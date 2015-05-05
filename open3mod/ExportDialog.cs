///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [ExportDialog.cs]
// (c) 2012-2015, Open3Mod Contributors
//
// Licensed under the terms and conditions of the 3-clause BSD license. See
// the LICENSE file in the root folder of the repository for the details.
//
// HIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND 
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS 
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Assimp;

namespace open3mod
{
    public partial class ExportDialog : Form
    {
        private readonly MainWindow _main;
        private readonly ExportFormatDescription[] _formats;

        private bool _changedText = false;


        public string SelectedFormatId
        {
            get
            {
                return _formats[comboBoxExportFormats.SelectedIndex].FormatId;
            }
        }

        public ExportFormatDescription SelectedFormat
        {
            get
            {
                return _formats[comboBoxExportFormats.SelectedIndex];
            }
        }

        public ExportDialog(MainWindow main)
        {
            _main = main;
            InitializeComponent();

            using (var v = new AssimpContext())
            {
                _formats = v.GetSupportedExportFormats();
                foreach (var format in _formats)
                {
                    comboBoxExportFormats.Items.Add(format.Description + "  (" + format.FileExtension + ")");
                }
                comboBoxExportFormats.SelectedIndex = ExportSettings.Default.ExportFormatIndex;
                comboBoxExportFormats.SelectedIndexChanged += (object s, EventArgs e) =>
                {
                    ExportSettings.Default.ExportFormatIndex = comboBoxExportFormats.SelectedIndex;
                    UpdateFileName(true);
                };
            }

            textBoxFileName.KeyPress += (object s, KeyPressEventArgs e) =>
            {
                _changedText = true;
            };

            // Respond to updates in the main window - the export dialog is non-modal and
            // always takes the currently selected file at the time the export button
            // is pressed.
            _main.SelectedTabChanged += (Tab tab) =>
            {
                UpdateFileName();
                UpdateCaption();
            };

            UpdateFileName();
            UpdateCaption();
        }

        private void UpdateCaption()
        {
            string str = "Export ";
            var scene = _main.UiState.ActiveTab.ActiveScene;
            if (scene != null)
            {
                str += Path.GetFileName(scene.File);
                buttonExportRun.Enabled = true;
            }
            else
            {
                buttonExportRun.Enabled = false;
                str += "<no scene currently selected>";
            }

            Text = str;
        }

        private void UpdateFileName(bool extensionOnly = false)
        {
            string str = textBoxFileName.Text;

            if (!extensionOnly && !_changedText)
            {
                var scene = _main.UiState.ActiveTab.ActiveScene;
                if (scene != null)
                {
                    str = Path.GetFileName(scene.File);
                }
            }
            textBoxFileName.Text = Path.ChangeExtension(str, SelectedFormat.FileExtension);
        }

        private void buttonSelectFolder_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.ShowDialog(this);
            textBoxPath.Text = folderBrowserDialog.SelectedPath;
        }

        private void buttonExport(object sender, EventArgs e)
        {
            var scene = _main.UiState.ActiveTab.ActiveScene;
            if (scene == null)
            {
                MessageBox.Show("No exportable scene selected");
                return;
            }
            DoExport(scene, SelectedFormatId);
        }

        private void DoExport(Scene scene, string id) 
        {          
            var overwriteWithoutConfirmation = checkBoxNoOverwriteConfirm.Checked;

            var path = textBoxPath.Text.Trim();
            path = (path.Length > 0 ? path : scene.BaseDir);
            var name = textBoxFileName.Text.Trim();
            var fullPath = Path.Combine(path, name);
            if (!overwriteWithoutConfirmation && Path.GetFullPath(fullPath) == Path.GetFullPath(scene.File))
            {
                if (MessageBox.Show("This will overwrite the current scene's source file. Continue?", "Warning", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    PushLog("Canceled");
                    return;
                }
            }

            var copyTextures = checkBoxCopyTexturesToSubfolder.Checked;
            var relativeTexturePaths = checkBoxUseRelativeTexturePaths.Checked;
            var includeAnimations = checkBoxIncludeAnimations.Checked;
            var includeSceneHierarchy = checkBoxIncludeSceneHierarchy.Checked;

            var textureCopyJobs = new Dictionary<string, string>();
            var textureDestinationFolder = textBoxCopyTexturesToFolder.Text;

            PushLog("*** Export: " + scene.File);

            if(copyTextures)
            {
                try
                {
                    Directory.CreateDirectory(Path.Combine(path, textureDestinationFolder));
                }
                catch (Exception)
                {
                    PushLog("Failed to create texture destination directory " + Path.Combine(path, textureDestinationFolder));
                    return;
                }
            }

            progressBarExport.Style = ProgressBarStyle.Marquee;
            progressBarExport.MarqueeAnimationSpeed = 5;

            // Create a shallow copy of the original scene that replaces all the texture paths with their
            // corresponding output paths, and omits animations if requested.
            var sourceScene = new Assimp.Scene
                              {
                                  Textures = scene.Raw.Textures,
                                  SceneFlags = scene.Raw.SceneFlags,
                                  RootNode = scene.Raw.RootNode,
                                  Meshes = scene.Raw.Meshes,
                                  Lights = scene.Raw.Lights,
                                  Cameras = scene.Raw.Cameras
                              };

            if (includeAnimations)
            {
                sourceScene.Animations = scene.Raw.Animations;
            }

            var uniques = new HashSet<string>();
            var textureMapping = new Dictionary<string, string>();

            PushLog("Locating all textures");
            foreach (var texId in scene.TextureSet.GetTextureIds())
            {
                // TODO(acgessler): Verify if this handles texture replacements and GUID-IDs correctly.
                var destName = texId;

                // Broadly skip over embedded (in-memory) textures
                if (destName.StartsWith("*"))
                {
                    PushLog("Ignoring embedded texture: " + destName);
                    continue;
                }

                // Locate the texture on-disk
                string diskLocation;
                try
                {
                    TextureLoader.ObtainStream(texId, scene.BaseDir, out diskLocation).Close();
                } catch(IOException)
                {
                    PushLog("Failed to locate texture " + texId);
                    continue;
                }
             
                if (copyTextures)
                {
                    destName = GeUniqueTextureExportPath(path, textureDestinationFolder, diskLocation, uniques);
                }

                if (relativeTexturePaths)
                {
                    textureMapping[texId] = GetRelativePath(path + "\\", destName);
                }
                else
                {
                    textureMapping[texId] = destName;
                }
                textureCopyJobs[diskLocation] = destName;

                PushLog("Texture " + texId + " maps to " + textureMapping[texId]);              
            }

            foreach (var mat in scene.Raw.Materials)
            {
                sourceScene.Materials.Add(CloneMaterial(mat, textureMapping));
            }

            var t = new Thread(() =>
            {
                using (var v = new AssimpContext())
                {
                    PushLog("Exporting using Assimp to " + fullPath + ", using format id: " + id);
                    var result = v.ExportFile(sourceScene, fullPath, id, includeSceneHierarchy 
                        ? PostProcessSteps.None 
                        : PostProcessSteps.PreTransformVertices);
                    _main.BeginInvoke(new MethodInvoker(() =>
                    {
                        progressBarExport.Style = ProgressBarStyle.Continuous;
                        progressBarExport.MarqueeAnimationSpeed = 0;

                        if (!result)
                        {
                            // TODO: get native error message
                            PushLog("Export failure");
                            MessageBox.Show("Failed to export to " + fullPath, "Export error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            if (copyTextures)
                            {
                                PushLog("Copying textures");
                                foreach (var kv in textureCopyJobs)
                                {
                                    PushLog(" ... " + kv.Key + " -> " + kv.Value);
                                    try
                                    {
                                        File.Copy(kv.Key, kv.Value, false);
                                    }                               
                                    catch (IOException)
                                    {
                                        if (!File.Exists(kv.Value))
                                        {
                                            throw;
                                        }
                                        if (!overwriteWithoutConfirmation && MessageBox.Show("Texture " + kv.Value +
                                                " already exists. Overwrite?", "Overwrite Warning",
                                                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                                        {
                                            PushLog("Exists already, skipping");
                                            continue;
                                        }
                                        PushLog("Exists already, overwriting");
                                        File.Copy(kv.Key, kv.Value, true);
                                    }
                                    catch (Exception ex)
                                    {
                                        PushLog(ex.Message);
                                    }
                                }
                            }
                            if (checkBoxOpenExportedFile.Checked)
                            {
                                _main.AddTab(fullPath, true, false);
                            }
                            PushLog("Export completed");
                        }
                    }));
                }
            });

            t.Start();
        }

        private static string GeUniqueTextureExportPath(string exportBasePath,
            string textureDestinationFolder,
            string diskLocation,
            HashSet<string> uniques)
        {
            string baseFileName = Path.GetFileNameWithoutExtension(diskLocation);     
            string destName;
            var count = 1;
            // Enforce unique output names
            do
            {
                destName = Path.Combine(exportBasePath, Path.Combine(textureDestinationFolder,
                    baseFileName +
                    (count == 1
                        ? ""
                        : (count.ToString(CultureInfo.InvariantCulture) + "_")) +
                    Path.GetExtension(diskLocation)
                    ));
                ++count;
            } while (uniques.Contains(destName));
            uniques.Add(destName);
            return destName;
        }

        private void PushLog(string message)
        {
            _main.BeginInvoke(new MethodInvoker(() =>
            {
                textBoxExportLog.Text += message + Environment.NewLine;
                textBoxExportLog.SelectionStart = textBoxExportLog.TextLength;
                textBoxExportLog.ScrollToCaret();
            }));
        }

        /// <summary>
        /// Clone a given material subject to a texture path remapping
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="textureMapping"></param>
        /// <returns></returns>
        private static Material CloneMaterial(Material mat, Dictionary<string, string> textureMapping)
        {
            Debug.Assert(mat != null);
            Debug.Assert(textureMapping != null);
            var matOut = new Material();
            foreach (var prop in mat.GetAllProperties())
            {
                var propOut = prop;
                if (prop.PropertyType == PropertyType.String && textureMapping.ContainsKey(prop.GetStringValue()))
                {
                    propOut = new MaterialProperty {PropertyType = PropertyType.String, Name = prop.Name};
                    propOut.TextureIndex = prop.TextureIndex;
                    propOut.TextureType = prop.TextureType;
                    propOut.SetStringValue(textureMapping[prop.GetStringValue()]);
                }
                matOut.AddProperty(propOut);
            }
            return matOut;
        }


        // From http://stackoverflow.com/questions/9042861/how-to-make-an-absolute-path-relative-to-a-particular-folder
        public static string GetRelativePath(string fromPath, string toPath)
        {
            var fromUri = new Uri(Path.GetFullPath(fromPath), UriKind.Absolute);
            var toUri = new Uri(Path.GetFullPath(toPath), UriKind.Absolute);
            return fromUri.MakeRelativeUri(toUri).ToString();
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 