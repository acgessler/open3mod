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
    public class ManagedImage {
        private MipMapChainCollection m_faces;
        private AnimationChainCollection m_animChain;

        //May hold a single face representing a 2D image or faces of a cubemap
        public MipMapChainCollection Faces {
            get {
                return m_faces;
            }
        }

        public AnimationChainCollection AnimationChain {
            get {
                return m_animChain;
            }
        }

        public ManagedImage(Image image) {
            m_faces = new MipMapChainCollection();
            m_animChain = new AnimationChainCollection();

            if(!image.IsValid) {
                return;
            }

            ImageID imageID = image.ImageID;
            LoadFaces(imageID, 0);
            LoadAnimationChain(imageID);
        }

        private ManagedImage(ImageID imageID, int imageNum) {
            m_faces = new MipMapChainCollection();
            m_animChain = new AnimationChainCollection();

            LoadFaces(imageID, imageNum);
        }

        private void LoadAnimationChain(ImageID imageID) {
            IL.BindImage(imageID);

            //Get total number of images in array (including first)
            int imageCount = IL.ilGetInteger(ILDefines.IL_NUM_IMAGES) + 1;

            //If just one image, we aren't really an animation chain
            if(imageCount > 1) {
                m_animChain.Add(this);
                for(int i = 1; i < imageCount; i++) {
                    ManagedImage image = new ManagedImage(imageID, i);
                    //If the image wasn't valid, don't add it
                    if(image.Faces.Count != 0)
                        m_animChain.Add(image);
                }
            }
        }

        private void LoadFaces(ImageID imageID, int imageNum) {
            IL.BindImage(imageID);
            if(!IL.ActiveImage(imageNum))
                return;

            //Get total number of faces (including base face)
            int faceCount = IL.ilGetInteger(ILDefines.IL_NUM_FACES) + 1;

            //Get the first face and every other as a mip map chain, when we hit a null, we break
            for(int i = 0; i < faceCount; i++) {
                MipMapChain mipMapChain = CreateMipMapChain(imageID, imageNum, i);
                if(mipMapChain == null)
                    break;
                m_faces.Add(mipMapChain);
            }
        }

        private MipMapChain CreateMipMapChain(ImageID imageID, int imageNum, int faceNum) {
            IL.BindImage(imageID);
            if(!IL.ActiveImage(imageNum))
                return null;
            if(!IL.ActiveFace(faceNum))
                return null;

            //Get total number of mipmaps (including base face)
            int mipMapCount = IL.ilGetInteger(ILDefines.IL_NUM_MIPMAPS) + 1;
            MipMapChain mipMapChain = new MipMapChain();

            //Get the first mipmap and every other, when we hit a null, we break
            for(int i = 0; i < mipMapCount; i++) {
                ImageData data = ImageData.Load(new Subimage(imageID, imageNum, faceNum, 0, i));
                if(data == null)
                    break;
                mipMapChain.Add(data);
            }
            mipMapChain.TrimExcess();
            return mipMapChain;
        }
    }
}
