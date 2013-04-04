/*
* Copyright (c) 2012-2013 AssimpNet - Nicholas Woodfield
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

using System;
using Assimp.Unmanaged;

namespace Assimp {
    /// <summary>
    /// Describes a light source in the scene. Assimp supports multiple light sources
    /// including spot, point, and directional lights. All are defined by a single structure
    /// and distinguished by their parameters. Lights have corresponding nodes in the scenegraph.
    /// <para>Some file formats such as 3DS and ASE export a "target point", e.g. the point
    /// a spot light is looking at (it can even be animated). Assimp writes the target point as a subnode
    /// of a spotlight's main node called "spotName.Target". However, this is just additional information
    /// then, the transform tracks of the main node make the spot light already point in the right direction.</para>
    /// </summary>
    public sealed class Light : IMarshalable<Light, AiLight> {
        private String m_name;
        private LightSourceType m_lightType;
        private float m_angleInnerCone;
        private float m_angleOuterCone;
        private float m_attConstant;
        private float m_attLinear;
        private float m_attQuadratic;
        private Vector3D m_position;
        private Vector3D m_direction;
        private Color3D m_diffuse;
        private Color3D m_specular;
        private Color3D m_ambient;

        /// <summary>
        /// Gets or sets the name of the light source. This corresponds to a node present in the scenegraph.
        /// </summary>
        public String Name {
            get {
                return m_name;
            }
            set {
                m_name = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of light source. This should never be undefined.
        /// </summary>
        public LightSourceType LightType {
            get {
                return m_lightType;
            }
            set {
                m_lightType = value;
            }
        }

        /// <summary>
        /// Gets or sets the inner angle of a spot light's light cone. The spot light has
        /// maximum influence on objects inside this angle. The angle is given in radians, it
        /// is 2PI for point lights and defined for directional lights.
        /// </summary>
        public float AngleInnerCone {
            get {
                return m_angleInnerCone;
            }
            set {
                m_angleInnerCone = value;
            }
        }

        /// <summary>
        /// Gets or sets the outer angle of a spot light's light cone. The spot light does not affect objects outside
        /// this angle. The angle is given in radians. It is 2PI for point lights and undefined for
        /// directional lights. The outer angle must be greater than or equal to the inner angle.
        /// </summary>
        public float AngleOuterCone {
            get {
                return m_angleOuterCone;
            }
            set {
                m_angleOuterCone = value;
            }
        }

        /// <summary>
        /// Gets or sets the constant light attenuation factor. The intensity of the light source
        /// at a given distance 'd' from the light position is <code>Atten = 1 / (att0 + att1 * d + att2 * d*d)</code>.
        /// <para>This member corresponds to the att0 variable in the equation and is undefined for directional lights.</para>
        /// </summary>
        public float AttenuationConstant {
            get {
                return m_attConstant;
            }
            set {
                m_attConstant = value;
            }
        }

        /// <summary>
        /// Gets or sets the linear light attenuation factor. The intensity of the light source
        /// at a given distance 'd' from the light position is <code>Atten = 1 / (att0 + att1 * d + att2 * d*d)</code>
        /// <para>This member corresponds to the att1 variable in the equation and is undefined for directional lights.</para>
        /// </summary>
        public float AttenuationLinear {
            get {
                return m_attLinear;
            }
            set {
                m_attLinear = value;
            }
        }

        /// <summary>
        /// Gets or sets the quadratic light attenuation factor. The intensity of the light source
        /// at a given distance 'd' from the light position is <code>Atten = 1 / (att0 + att1 * d + att2 * d*d)</code>.
        /// <para>This member corresponds to the att2 variable in the equation and is undefined for directional lights.</para>
        /// </summary>
        public float AttenuationQuadratic {
            get {
                return m_attQuadratic;
            }
            set {
                m_attQuadratic = value;
            }
        }

        /// <summary>
        /// Gets or sets the position of the light source in space, relative to the
        /// transformation of the node corresponding to the light. This is undefined for
        /// directional lights.
        /// </summary>
        public Vector3D Position {
            get {
                return m_position;
            }
            set {
                m_position = value;
            }
        }

        /// <summary>
        /// Gets or sets the direction of the light source in space, relative to the transformation
        /// of the node corresponding to the light. This is undefined for point lights.
        /// </summary>
        public Vector3D Direction {
            get {
                return m_direction;
            }
            set {
                m_direction = value;
            }
        }

        /// <summary>
        /// Gets or sets the diffuse color of the light source.  The diffuse light color is multiplied with
        /// the diffuse material color to obtain the final color that contributes to the diffuse shading term.
        /// </summary>
        public Color3D ColorDiffuse {
            get {
                return m_diffuse;
            }
            set {
                m_diffuse = value;
            }
        }

        /// <summary>
        /// Gets or sets the specular color of the light source. The specular light color is multiplied with the
        /// specular material color to obtain the final color that contributes to the specular shading term.
        /// </summary>
        public Color3D ColorSpecular {
            get {
                return m_specular;
            }
            set {
                m_specular = value;
            }
        }

        /// <summary>
        /// Gets or sets the ambient color of the light source. The ambient light color is multiplied with the ambient
        /// material color to obtain the final color that contributes to the ambient shading term.
        /// </summary>
        public Color3D ColorAmbient {
            get {
                return m_ambient;
            }
            set {
                m_ambient = value;
            }
        }

        /// <summary>
        /// Constructs a new Light.
        /// </summary>
        /// <param name="light">Unmanaged AiLight struct</param>
        internal Light(ref AiLight light) {
            m_name = light.Name.GetString();
            m_lightType = light.Type;
            m_angleInnerCone = light.AngleInnerCone;
            m_angleOuterCone = light.AngleOuterCone;
            m_attConstant = light.AttenuationConstant;
            m_attLinear = light.AttenuationLinear;
            m_attQuadratic = light.AttenuationQuadratic;
            m_position = light.Position;
            m_direction = light.Direction;
            m_diffuse = light.ColorDiffuse;
            m_specular = light.ColorSpecular;
            m_ambient = light.ColorAmbient;
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="Light"/> class.
        /// </summary>
        public Light() {
            m_lightType = LightSourceType.Undefined;
        }

        #region IMarshalable Implementation

        /// <summary>
        /// Gets if the native value type is blittable (that is, does not require marshaling by the runtime, e.g. has MarshalAs attributes).
        /// </summary>
        bool IMarshalable<Light, AiLight>.IsNativeBlittable {
            get { return true; }
        }

        /// <summary>
        /// Writes the managed data to the native value.
        /// </summary>
        /// <param name="thisPtr">Optional pointer to the memory that will hold the native value.</param>
        /// <param name="nativeValue">Output native value</param>
        void IMarshalable<Light, AiLight>.ToNative(IntPtr thisPtr, out AiLight nativeValue) {
            nativeValue.Name = new AiString(m_name);
            nativeValue.Type = m_lightType;
            nativeValue.AngleInnerCone = m_angleInnerCone;
            nativeValue.AngleOuterCone = m_angleOuterCone;
            nativeValue.AttenuationConstant = m_attConstant;
            nativeValue.AttenuationLinear = m_attLinear;
            nativeValue.AttenuationQuadratic = m_attQuadratic;
            nativeValue.ColorAmbient = m_ambient;
            nativeValue.ColorDiffuse = m_diffuse;
            nativeValue.ColorSpecular = m_specular;
            nativeValue.Direction = m_direction;
            nativeValue.Position = m_position;
        }

        /// <summary>
        /// Reads the unmanaged data from the native value.
        /// </summary>
        /// <param name="nativeValue">Input native value</param>
        void IMarshalable<Light, AiLight>.FromNative(ref AiLight nativeValue) {
            m_name = nativeValue.Name.GetString();
            m_lightType = nativeValue.Type;
            m_angleInnerCone = nativeValue.AngleInnerCone;
            m_angleOuterCone = nativeValue.AngleOuterCone;
            m_attConstant = nativeValue.AttenuationConstant;
            m_attLinear = nativeValue.AttenuationLinear;
            m_attQuadratic = nativeValue.AttenuationQuadratic;
            m_position = nativeValue.Position;
            m_direction = nativeValue.Direction;
            m_diffuse = nativeValue.ColorDiffuse;
            m_specular = nativeValue.ColorSpecular;
            m_ambient = nativeValue.ColorAmbient;
        }

        /// <summary>
        /// Frees unmanaged memory created by <see cref="ToNative"/>.
        /// </summary>
        /// <param name="nativeValue">Native value to free</param>
        /// <param name="freeNative">True if the unmanaged memory should be freed, false otherwise.</param>
        public static void FreeNative(IntPtr nativeValue, bool freeNative) {
            if(nativeValue != IntPtr.Zero && freeNative)
                MemoryHelper.FreeMemory(nativeValue);
        }

        #endregion
    }
}
