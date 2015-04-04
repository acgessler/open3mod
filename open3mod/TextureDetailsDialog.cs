///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [TextureDetailsDialog.cs]
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
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using Assimp;

namespace open3mod
{
    public partial class TextureDetailsDialog : Form
    {
        private TextureThumbnailControl _tex;
        public TextureDetailsDialog()
        {
            InitializeComponent();
        }


        public TextureThumbnailControl GetTexture()
        {
            return _tex;
        }


        public void SetTexture(TextureThumbnailControl tex)
        {
            Debug.Assert(tex != null && tex.Texture != null);

            _tex = tex;
            var img = tex.Texture.Image;

            Text = Path.GetFileName(tex.FilePath) + " - Details";

            pictureBox1.Image = img;

            if (img != null)
            {
                labelInfo.Text = string.Format("Size: {0} x {1} px", img.Width, img.Height);
            }
            checkBoxHasAlpha.Checked = tex.Texture.HasAlpha == Texture.AlphaState.HasAlpha;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 