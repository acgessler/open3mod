///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [FileAssociations.cs]
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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace open3mod
{
    public static class FileAssociations
    {
        /// <summary>
        /// Set file-system associations for the given list of file formats
        /// </summary>
        /// <param name="extensionList"></param>
        public static bool SetAssociations(string[] extensionList)
        {
            // based on the old assimp viewer code (<assimp-repo>/tools/assimp_cmd) and
            // http://stackoverflow.com/questions/2681878/associate-file-extension-with-application
            foreach(var extension in extensionList)
            {
                using (var baseKey = Registry.CurrentUser.CreateSubKey(@"Software\Classes\" + extension))
                {
                    if (baseKey != null)
                    {
                        baseKey.SetValue("", "OPEN3MOD_CLASS");
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            using (var currentUser = Registry.CurrentUser.CreateSubKey(@"Software\Classes\OPEN3MOD_CLASS"))
            {
                using (var openKey = Registry.CurrentUser.CreateSubKey(@"Software\Classes\OPEN3MOD_CLASS\shell\open\command"))
                {
                    if (openKey != null)
                    {
                        openKey.SetValue("", Application.ExecutablePath + " \"%1\"");
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            // Tell explorer the file association has been changed
            SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
            return true;
        }


        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 