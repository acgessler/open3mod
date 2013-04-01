using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace open3mod
{
    /// <summary>
    /// Shared utility class for SceneRendererClassicGl and SceneRendererModernGl
    /// </summary>
    public class SceneRendererShared
    {
        protected readonly Scene Owner;
        protected readonly Vector3 InitposeMin;
        protected readonly Vector3 InitposeMax;
        protected readonly CpuSkinningEvaluator Skinner;
        protected readonly bool[] IsAlphaMaterial;


        /// <summary>
        /// Constructs an instance given a scene with its bounds (AABB)
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="initposeMin"></param>
        /// <param name="initposeMax"></param>
        protected SceneRendererShared(Scene owner, Vector3 initposeMin, Vector3 initposeMax)
        {
            Owner = owner;
            InitposeMin = initposeMin;
            InitposeMax = initposeMax;

            Debug.Assert(Owner.Raw != null);    
            Skinner = new CpuSkinningEvaluator(owner);

            IsAlphaMaterial = new bool[owner.Raw.MaterialCount];
            for (int i = 0; i < IsAlphaMaterial.Length; ++i)
            {
                IsAlphaMaterial[i] = Owner.MaterialMapper.IsAlphaMaterial(owner.Raw.Materials[i]);
            }
        }


        /// <summary>
        /// Make sure all textures required for the materials in the scene are uploaded to VRAM.
        /// </summary>
        public void UploadTextures()
        {
            if (Owner.Raw.Materials == null)
            {
                return;
            }
            var i = 0;
            foreach (var mat in Owner.Raw.Materials)
            {
                if (Owner.MaterialMapper.UploadTextures(mat))
                {
                    IsAlphaMaterial[i] = Owner.MaterialMapper.IsAlphaMaterial(mat);
                }
                ++i;
            }
        }
    }
}
