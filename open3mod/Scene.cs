using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Assimp;
using Assimp.Configs;

namespace open3mod
{
    /// <summary>
    /// Represents a 3D scene/asset loaded through assimp.
    /// 
    /// Basically, this class contains the aiScene plus some auxiliary structures
    /// for drawing. Since assimp is the only source for model data and this is
    /// only a viewer, we ignore the recommendation of the assimp docs and use
    /// its data structures (almost) directly for rendering.
    /// </summary>
    public class Scene
    {
        private readonly Assimp.Scene _raw;

        /// <summary>
        /// Obtain the "raw" scene data as imported by Assimp
        /// </summary>
        public Assimp.Scene Raw
        {
            get { return _raw; }
        }


        /// <summary>
        /// Construct a scene given a file name, throw if loading fails
        /// </summary>
        /// <param name="file">File name to be loaded</param>
        public Scene(string file) 
        {
            var imp = new AssimpImporter();

            // Assimp configuration:
            
            //  - if no normals are present, generate them using a threshold
            //    angle of 66 degrees.
            imp.SetConfig(new NormalSmoothingAngleConfig(66.0f));


            //  - request lots of post processing steps, the details of which
            //    can be found in the TargetRealTimeMaximumQuality docs.
            _raw = imp.ImportFile(file, PostProcessPreset.TargetRealTimeMaximumQuality);
        }      
    }
}
