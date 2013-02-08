/*
* Copyright (c) 2012 Nicholas Woodfield
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using DevIL.Unmanaged;

namespace DevIL {
    public class TransformEngine {

        public Placement ImagePlacement {
            get {
                return ILU.GetImagePlacement();
            }
            set {
                ILU.SetImagePlacement(value);
            }
        }

        public bool Crop(Image image, int offsetX, int offsetY, int offsetZ, int width, int height, int depth) {
            if(image == null || !image.IsValid) {
                return false;
            }

            IL.BindImage(image.ImageID);
            return ILU.Crop(offsetX, offsetY, offsetZ, width, height, depth);
        }

        public bool EnlargeCanvas(Image image, int width, int height, int depth) {
            if(image == null || !image.IsValid) {
                return false;
            }

            IL.BindImage(image.ImageID);
            return ILU.EnlargeCanvas( width,  height,  depth);
        }

        public bool EnlargeImage(Image image, int xDim, int yDim, int zDim) {
            if(image == null || !image.IsValid) {
                return false;
            }

            IL.BindImage(image.ImageID);
            return ILU.EnlargeImage( xDim,  yDim,  zDim);
        }

        public bool Scale(Image image, int width, int height, int depth) {
            if(image == null || !image.IsValid) {
                return false;
            }

            IL.BindImage(image.ImageID);
            return ILU.Scale(width, height, depth);
        }

        public bool ScaleAlpha(Image image, float scale) {
            if(image == null || !image.IsValid) {
                return false;
            }

            IL.BindImage(image.ImageID);
            return ILU.ScaleAlpha(scale);
        }

        public bool ScaleColors(Image image, float red, float green, float blue) {
            if(image == null || !image.IsValid) {
                return false;
            }

            IL.BindImage(image.ImageID);
            return ILU.ScaleColors(red, green, blue);
        }

        public bool ReplaceColor(Image image, byte red, byte green, byte blue, float tolerance) {
            if(image == null || !image.IsValid) {
                return false;
            }

            IL.BindImage(image.ImageID);
            return ILU.ReplaceColor(red, green, blue, tolerance);
        }

        public bool SwapColors(Image image) {
            if(image == null || !image.IsValid) {
                return false;
            }

            IL.BindImage(image.ImageID);
            return ILU.SwapColors();
        }

        public bool FlipImage(Image image) {
            if(image == null || !image.IsValid) {
                return false;
            }

            IL.BindImage(image.ImageID);
            return ILU.FlipImage();
        }

        public bool Mirror(Image image) {
            if(image == null || !image.IsValid) {
                return false;
            }

            IL.BindImage(image.ImageID);
            return ILU.Mirror();
        }

        public bool Rotate(Image image, float angle) {
            if(image == null || !image.IsValid) {
                return false;
            }

            IL.BindImage(image.ImageID);
            return ILU.Rotate(angle);
        }

        public bool Rotate3D(Image image, float x, float y, float z, float angle) {
            if(image == null || !image.IsValid) {
                return false;
            }

            IL.BindImage(image.ImageID);
            return ILU.Rotate3D(x, y, z, angle);
        }


    }
}
