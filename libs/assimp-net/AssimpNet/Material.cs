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
using System.Collections.Generic;
using System.Linq;
using Assimp.Unmanaged;

namespace Assimp {
    /// <summary>
    /// A material contains all the information that describes how to render a mesh. E.g. textures, colors, and render states. Internally
    /// all this information is stored as key-value pair properties. The class contains many convienence methods and properties for
    /// accessing non-texture/texture properties without having to know the Assimp material key names. Not all properties may be present,
    /// and if they aren't a default value will be returned.
    /// </summary>
    public sealed class Material : IMarshalable<Material, AiMaterial> {
        private Dictionary<String, MaterialProperty> m_properties;

        /// <summary>
        /// Gets the number of properties contained in the material.
        /// </summary>
        public int PropertyCount {
            get {
                return m_properties.Count;
            }
        }

        #region Convienent non-texture properties

        /// <summary>
        /// Checks if the material has a name property.
        /// </summary>
        public bool HasName {
            get {
                return HasProperty(AiMatKeys.NAME);
            }
        }

        /// <summary>
        /// Gets the material name value, if any. Default value is an empty string.
        /// </summary>
        public String Name {
            get {
                MaterialProperty prop = GetProperty(AiMatKeys.NAME);
                if(prop != null)
                    return prop.GetStringValue();

                return String.Empty;
            }
            set {
                MaterialProperty prop = GetProperty(AiMatKeys.NAME);

                if(prop == null) {
                    prop = new MaterialProperty(AiMatKeys.NAME, value);
                    AddProperty(prop);
                }

                prop.SetStringValue(value);
            }
        }

        /// <summary>
        /// Checks if the material has a two-sided property.
        /// </summary>
        public bool HasTwoSided {
            get {
                return HasProperty(AiMatKeys.TWOSIDED);
            }
        }

        /// <summary>
        /// Gets if the material should be rendered as two-sided. Default value is false.
        /// </summary>
        public bool IsTwoSided {
            get {
                MaterialProperty prop = GetProperty(AiMatKeys.TWOSIDED);
                if(prop != null)
                    return prop.GetBooleanValue();

                return false;
            }
            set {
                MaterialProperty prop = GetProperty(AiMatKeys.TWOSIDED);

                if(prop == null) {
                    prop = new MaterialProperty(AiMatKeys.TWOSIDED, value);
                    AddProperty(prop);
                }

                prop.SetBooleanValue(value);
            }
        }

        /// <summary>
        /// Checks if the material has a shading-mode property.
        /// </summary>
        public bool HasShadingMode {
            get {
                return HasProperty(AiMatKeys.SHADING_MODEL);
            }
        }

        /// <summary>
        /// Gets the shading mode. Default value is <see cref="Assimp.ShadingMode.None"/>, meaning it is not defined.
        /// </summary>
        public ShadingMode ShadingMode {
            get {
                MaterialProperty prop = GetProperty(AiMatKeys.SHADING_MODEL);
                if(prop != null)
                    return (ShadingMode) prop.GetIntegerValue();

                return ShadingMode.None;
            }
            set {
                MaterialProperty prop = GetProperty(AiMatKeys.SHADING_MODEL);

                if(prop == null) {
                    prop = new MaterialProperty(AiMatKeys.SHADING_MODEL, (int) value);
                    AddProperty(prop);
                }

                prop.SetIntegerValue((int) value);
            }
        }

        /// <summary>
        /// Checks if the material has a wireframe property.
        /// </summary>
        public bool HasWireFrame {
            get {
                return HasProperty(AiMatKeys.ENABLE_WIREFRAME);
            }
        }

        /// <summary>
        /// Gets if wireframe should be enabled. Default value is false.
        /// </summary>
        public bool IsWireFrameEnabled {
            get {
                MaterialProperty prop = GetProperty(AiMatKeys.ENABLE_WIREFRAME);
                if(prop != null)
                    return prop.GetBooleanValue();

                return false;
            }
            set {
                MaterialProperty prop = GetProperty(AiMatKeys.ENABLE_WIREFRAME);

                if(prop == null) {
                    prop = new MaterialProperty(AiMatKeys.ENABLE_WIREFRAME, value);
                    AddProperty(prop);
                }

                prop.SetBooleanValue(value);
            }
        }

        /// <summary>
        /// Checks if the material has a blend mode property.
        /// </summary>
        public bool HasBlendMode {
            get {
                return HasProperty(AiMatKeys.BLEND_FUNC);
            }
        }

        /// <summary>
        /// Gets the blending mode. Default value is <see cref="Assimp.BlendMode.Default"/>.
        /// </summary>
        public BlendMode BlendMode {
            get {
                MaterialProperty prop = GetProperty(AiMatKeys.BLEND_FUNC);
                if(prop != null)
                    return (BlendMode) prop.GetIntegerValue();

                return BlendMode.Default;
            }
            set {
                MaterialProperty prop = GetProperty(AiMatKeys.BLEND_FUNC);

                if(prop == null) {
                    prop = new MaterialProperty(AiMatKeys.BLEND_FUNC, (int) value);
                    AddProperty(prop);
                }

                prop.SetIntegerValue((int) value);
            }
        }

        /// <summary>
        /// Checks if the material has an opacity property.
        /// </summary>
        public bool HasOpacity {
            get {
                return HasProperty(AiMatKeys.OPACITY);
            }
        }

        /// <summary>
        /// Gets the opacity. Default value is 1.0f.
        /// </summary>
        public float Opacity {
            get {
                MaterialProperty prop = GetProperty(AiMatKeys.OPACITY);
                if(prop != null)
                    return prop.GetFloatValue();

                return 1.0f;
            }
            set {
                MaterialProperty prop = GetProperty(AiMatKeys.OPACITY);

                if(prop == null) {
                    prop = new MaterialProperty(AiMatKeys.OPACITY, value);
                    AddProperty(prop);
                }

                prop.SetFloatValue(value);
            }
        }

        /// <summary>
        /// Checks if the material has a bump scaling property.
        /// </summary>
        public bool HasBumpScaling {
            get {
                return HasProperty(AiMatKeys.BUMPSCALING);
            }
        }

        /// <summary>
        /// Gets the bump scaling. Default value is 0.0f;
        /// </summary>
        public float BumpScaling {
            get {
                MaterialProperty prop = GetProperty(AiMatKeys.BUMPSCALING);
                if(prop != null)
                    return prop.GetFloatValue();
  
                return 0.0f;
            }
            set {
                MaterialProperty prop = GetProperty(AiMatKeys.BUMPSCALING);

                if(prop == null) {
                    prop = new MaterialProperty(AiMatKeys.BUMPSCALING, value);
                    AddProperty(prop);
                }

                prop.SetFloatValue(value);
            }
        }

        /// <summary>
        /// Checks if the material has a shininess property.
        /// </summary>
        public bool HasShininess {
            get {
                return HasProperty(AiMatKeys.SHININESS);
            }
        }

        /// <summary>
        /// Gets the shininess. Default value is 0.0f;
        /// </summary>
        public float Shininess {
            get {
                MaterialProperty prop = GetProperty(AiMatKeys.SHININESS);
                if(prop != null)
                    return prop.GetFloatValue();

                return 0.0f;
            }
            set {
                MaterialProperty prop = GetProperty(AiMatKeys.SHININESS);

                if(prop == null) {
                    prop = new MaterialProperty(AiMatKeys.SHININESS, value);
                    AddProperty(prop);
                }

                prop.SetFloatValue(value);
            }
        }

        /// <summary>
        /// Checks if the material has a shininess strength property.
        /// </summary>
        public bool HasShininessStrength {
            get {
                return HasProperty(AiMatKeys.SHININESS_STRENGTH);
            }
        }

        /// <summary>
        /// Gets the shininess strength. Default vaulue is 1.0f.
        /// </summary>
        public float ShininessStrength {
            get {
                MaterialProperty prop = GetProperty(AiMatKeys.SHININESS_STRENGTH);
                if(prop != null)
                    return prop.GetFloatValue();

                return 1.0f;
            }
            set {
                MaterialProperty prop = GetProperty(AiMatKeys.SHININESS_STRENGTH);

                if(prop == null) {
                    prop = new MaterialProperty(AiMatKeys.SHININESS_STRENGTH, value);
                    AddProperty(prop);
                }

                prop.SetFloatValue(value);
            }
        }

        /// <summary>
        /// Checks if the material has a reflectivty property.
        /// </summary>
        public bool HasReflectivity {
            get {
                return HasProperty(AiMatKeys.REFLECTIVITY);
            }
        }


        /// <summary>
        /// Gets the reflectivity. Default value is 0.0f;
        /// </summary>
        public float Reflectivity {
            get {
                MaterialProperty prop = GetProperty(AiMatKeys.REFLECTIVITY);
                if(prop != null)
                    return prop.GetFloatValue();

                return 0.0f;
            }
            set {
                MaterialProperty prop = GetProperty(AiMatKeys.REFLECTIVITY);

                if(prop == null) {
                    prop = new MaterialProperty(AiMatKeys.REFLECTIVITY, value);
                    AddProperty(prop);
                }

                prop.SetFloatValue(value);
            }
        }

        /// <summary>
        /// Checks if the material has a color diffuse property.
        /// </summary>
        public bool HasColorDiffuse {
            get {
                return HasProperty(AiMatKeys.COLOR_DIFFUSE);
            }
        }

        /// <summary>
        /// Gets the color diffuse. Default value is white.
        /// </summary>
        public Color4D ColorDiffuse {
            get {
                MaterialProperty prop = GetProperty(AiMatKeys.COLOR_DIFFUSE);
                if(prop != null)
                    return prop.GetColor4DValue();

                return new Color4D(1.0f, 1.0f, 1.0f, 1.0f);
            }
            set {
                MaterialProperty prop = GetProperty(AiMatKeys.COLOR_DIFFUSE);

                if(prop == null) {
                    prop = new MaterialProperty(AiMatKeys.COLOR_DIFFUSE, value);
                    AddProperty(prop);
                }

                prop.SetColor4DValue(value);
            }
        }

        /// <summary>
        /// Checks if the material has a color ambient property.
        /// </summary>
        public bool HasColorAmbient {
            get {
                return HasProperty(AiMatKeys.COLOR_AMBIENT);
            }
        }

        /// <summary>
        /// Gets the color ambient. Default value is (.2f, .2f, .2f, 1.0f).
        /// </summary>
        public Color4D ColorAmbient {
            get {
                MaterialProperty prop = GetProperty(AiMatKeys.COLOR_AMBIENT);
                if(prop != null)
                    return prop.GetColor4DValue();

                return new Color4D(.2f, .2f, .2f, 1.0f);
            }
            set {
                MaterialProperty prop = GetProperty(AiMatKeys.COLOR_AMBIENT);

                if(prop == null) {
                    prop = new MaterialProperty(AiMatKeys.COLOR_AMBIENT, value);
                    AddProperty(prop);
                }

                prop.SetColor4DValue(value);
            }
        }

        /// <summary>
        /// Checks if the material has a color specular property.
        /// </summary>
        public bool HasColorSpecular {
            get {
                return HasProperty(AiMatKeys.COLOR_SPECULAR);
            }
        }

        /// <summary>
        /// Gets the color specular. Default value is black.
        /// </summary>
        public Color4D ColorSpecular {
            get {
                MaterialProperty prop = GetProperty(AiMatKeys.COLOR_SPECULAR);
                if(prop != null)
                    return prop.GetColor4DValue();

                return new Color4D(0, 0, 0, 1.0f);
            }
            set {
                MaterialProperty prop = GetProperty(AiMatKeys.COLOR_SPECULAR);

                if(prop == null) {
                    prop = new MaterialProperty(AiMatKeys.COLOR_SPECULAR, value);
                    AddProperty(prop);
                }

                prop.SetColor4DValue(value);
            }
        }

        /// <summary>
        /// Checks if the material has a color emissive property.
        /// </summary>
        public bool HasColorEmissive {
            get {
                return HasProperty(AiMatKeys.COLOR_EMISSIVE);
            }
        }

        /// <summary>
        /// Gets the color emissive. Default value is black.
        /// </summary>
        public Color4D ColorEmissive {
            get {
                MaterialProperty prop = GetProperty(AiMatKeys.COLOR_EMISSIVE);
                if(prop != null)
                    return prop.GetColor4DValue();

                return new Color4D(0, 0, 0, 1.0f);
            }
            set {
                MaterialProperty prop = GetProperty(AiMatKeys.COLOR_EMISSIVE);

                if(prop == null) {
                    prop = new MaterialProperty(AiMatKeys.COLOR_EMISSIVE, value);
                    AddProperty(prop);
                }

                prop.SetColor4DValue(value);
            }
        }

        /// <summary>
        /// Checks if the material has a color transparent property.
        /// </summary>
        public bool HasColorTransparent {
            get {
                return HasProperty(AiMatKeys.COLOR_TRANSPARENT);
            }
        }

        /// <summary>
        /// Gets the color transparent. Default value is black.
        /// </summary>
        public Color4D ColorTransparent {
            get {
                MaterialProperty prop = GetProperty(AiMatKeys.COLOR_TRANSPARENT);
                if(prop != null)
                    return prop.GetColor4DValue();

                return new Color4D(0, 0, 0, 1.0f);
            }
            set {
                MaterialProperty prop = GetProperty(AiMatKeys.COLOR_TRANSPARENT);

                if(prop == null) {
                    prop = new MaterialProperty(AiMatKeys.COLOR_TRANSPARENT, value);
                    AddProperty(prop);
                }

                prop.SetColor4DValue(value);
            }
        }

        /// <summary>
        /// Checks if the material has a color reflective property.
        /// </summary>
        public bool HasColorReflective {
            get {
                return HasProperty(AiMatKeys.COLOR_REFLECTIVE);
            }
        }

        /// <summary>
        /// Gets the color reflective. Default value is black.
        /// </summary>
        public Color4D ColorReflective {
            get {
                MaterialProperty prop = GetProperty(AiMatKeys.COLOR_REFLECTIVE);
                if(prop != null)
                    return prop.GetColor4DValue();

                return new Color4D(0, 0, 0, 1.0f);
            }
            set {
                MaterialProperty prop = GetProperty(AiMatKeys.COLOR_REFLECTIVE);

                if(prop == null) {
                    prop = new MaterialProperty(AiMatKeys.COLOR_REFLECTIVE, value);
                    AddProperty(prop);
                }

                prop.SetColor4DValue(value);
            }
        }

        #endregion

        #region Convienent texture properties

        /// <summary>
        /// Gets if the material has a diffuse texture in the first texture index.
        /// </summary>
        public bool HasTextureDiffuse {
            get {
                return HasProperty(AiMatKeys.TEXTURE_BASE, TextureType.Diffuse, 0);
            }
        }

        /// <summary>
        /// Gets or sets diffuse texture properties in the first texture index.
        /// </summary>
        public TextureSlot TextureDiffuse {
            get {
                TextureSlot tex;
                GetMaterialTexture(TextureType.Diffuse, 0, out tex);

                return tex;
            }
            set {
                if(value.TextureIndex == 0 && value.TextureType == TextureType.Diffuse)
                    AddMaterialTexture(ref value);
            }
        }

        /// <summary>
        /// Gets if the material has a specular texture in the first texture index.
        /// </summary>
        public bool HasTextureSpecular {
            get {
                return HasProperty(AiMatKeys.TEXTURE_BASE, TextureType.Specular, 0);
            }
        }

        /// <summary>
        /// Gets or sets specular texture properties in the first texture index.
        /// </summary>
        public TextureSlot TextureSpecular {
            get {
                TextureSlot tex;
                GetMaterialTexture(TextureType.Specular, 0, out tex);

                return tex;
            }
            set {
                if(value.TextureIndex == 0 && value.TextureType == TextureType.Specular)
                    AddMaterialTexture(ref value);
            }
        }

        /// <summary>
        /// Gets if the material has a ambient texture in the first texture index.
        /// </summary>
        public bool HasTextureAmbient {
            get {
                return HasProperty(AiMatKeys.TEXTURE_BASE, TextureType.Ambient, 0);
            }
        }

        /// <summary>
        /// Gets or sets ambient texture properties in the first texture index.
        /// </summary>
        public TextureSlot TextureAmbient {
            get {
                TextureSlot tex;
                GetMaterialTexture(TextureType.Ambient, 0, out tex);

                return tex;
            }
            set {
                if(value.TextureIndex == 0 && value.TextureType == TextureType.Ambient)
                    AddMaterialTexture(ref value);
            }
        }

        /// <summary>
        /// Gets if the material has a emissive texture in the first texture index.
        /// </summary>
        public bool HasTextureEmissive {
            get {
                return HasProperty(AiMatKeys.TEXTURE_BASE, TextureType.Emissive, 0);
            }
        }

        /// <summary>
        /// Gets or sets emissive texture properties in the first texture index.
        /// </summary>
        public TextureSlot TextureEmissive {
            get {
                TextureSlot tex;
                GetMaterialTexture(TextureType.Emissive, 0, out tex);

                return tex;
            }
            set {
                if(value.TextureIndex == 0 && value.TextureType == TextureType.Emissive)
                    AddMaterialTexture(ref value);
            }
        }

        /// <summary>
        /// Gets if the material has a height texture in the first texture index.
        /// </summary>
        public bool HasTextureHeight {
            get {
                return HasProperty(AiMatKeys.TEXTURE_BASE, TextureType.Height, 0);
            }
        }

        /// <summary>
        /// Gets or sets height texture properties in the first texture index.
        /// </summary>
        public TextureSlot TextureHeight {
            get {
                TextureSlot tex;
                GetMaterialTexture(TextureType.Height, 0, out tex);

                return tex;
            }
            set {
                if(value.TextureIndex == 0 && value.TextureType == TextureType.Height)
                    AddMaterialTexture(ref value);
            }
        }

        /// <summary>
        /// Gets if the material has a normal texture in the first texture index.
        /// </summary>
        public bool HasTextureNormal {
            get {
                return HasProperty(AiMatKeys.TEXTURE_BASE, TextureType.Normals, 0);
            }
        }

        /// <summary>
        /// Gets or sets normal texture properties in the first texture index.
        /// </summary>
        public TextureSlot TextureNormal {
            get {
                TextureSlot tex;
                GetMaterialTexture(TextureType.Normals, 0, out tex);

                return tex;
            }
            set {
                if(value.TextureIndex == 0 && value.TextureType == TextureType.Normals)
                    AddMaterialTexture(ref value);
            }
        }

        /// <summary>
        /// Gets if the material has an opacity texture in the first texture index.
        /// </summary>
        public bool HasTextureOpacity {
            get {
                return HasProperty(AiMatKeys.TEXTURE_BASE, TextureType.Opacity, 0);
            }
        }

        /// <summary>
        /// Gets or sets opacity texture properties in the first texture index.
        /// </summary>
        public TextureSlot TextureOpacity {
            get {
                TextureSlot tex;
                GetMaterialTexture(TextureType.Opacity, 0, out tex);

                return tex;
            }
            set {
                if(value.TextureIndex == 0 && value.TextureType == TextureType.Opacity)
                    AddMaterialTexture(ref value);
            }
        }

        /// <summary>
        /// Gets if the material has a displacement texture in the first texture index.
        /// </summary>
        public bool HasTextureDisplacement {
            get {
                return HasProperty(AiMatKeys.TEXTURE_BASE, TextureType.Displacement, 0);
            }
        }

        /// <summary>
        /// Gets or sets displacement texture properties in the first texture index.
        /// </summary>
        public TextureSlot TextureDisplacement {
            get {
                TextureSlot tex;
                GetMaterialTexture(TextureType.Displacement, 0, out tex);

                return tex;
            }
            set {
                if(value.TextureIndex == 0 && value.TextureType == TextureType.Displacement)
                    AddMaterialTexture(ref value);
            }
        }

        /// <summary>
        /// Gets if the material has a light map texture in the first texture index.
        /// </summary>
        public bool HasTextureLightMap {
            get {
                return HasProperty(AiMatKeys.TEXTURE_BASE, TextureType.Lightmap, 0);
            }
        }

        /// <summary>
        /// Gets or sets light map texture properties in the first texture index.
        /// </summary>
        public TextureSlot TextureLightMap {
            get {
                TextureSlot tex;
                GetMaterialTexture(TextureType.Lightmap, 0, out tex);

                return tex;
            }
            set {
                if(value.TextureIndex == 0 && value.TextureType == TextureType.Lightmap)
                    AddMaterialTexture(ref value);
            }
        }

        /// <summary>
        /// Gets if the material has a reflection texture in the first texture index.
        /// </summary>
        public bool HasTextureReflection {
            get {
                return HasProperty(AiMatKeys.TEXTURE_BASE, TextureType.Reflection, 0);
            }
        }

        /// <summary>
        /// Gets or sets reflection texture properties in the first texture index.
        /// </summary>
        public TextureSlot TextureReflection {
            get {
                TextureSlot tex;
                GetMaterialTexture(TextureType.Reflection, 0, out tex);

                return tex;
            }
            set {
                if(value.TextureIndex == 0 && value.TextureType == TextureType.Reflection)
                    AddMaterialTexture(ref value);
            }
        }

        #endregion

        /// <summary>
        /// Constructs a new Material.
        /// </summary>
        /// <param name="material">Unmanaged AiMaterial struct.</param>
        internal Material(ref AiMaterial material) {
            m_properties = new Dictionary<String, MaterialProperty>();

            if(material.NumProperties > 0 && material.Properties != IntPtr.Zero) {
                AiMaterialProperty[] properties = MemoryHelper.MarshalArray<AiMaterialProperty>(material.Properties, (int) material.NumProperties, true);
                for(int i = 0; i < properties.Length; i++) {
                    MaterialProperty prop = new MaterialProperty(ref material, ref properties[i]);
                    m_properties.Add(prop.FullyQualifiedName, prop);
                }
            }
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="Material"/> class.
        /// </summary>
        public Material() {
            m_properties = new Dictionary<String, MaterialProperty>();
        }

        /// <summary>
        /// Helper method to construct a fully qualified name from the input parameters. All the input parameters are combined into the fully qualified name: {baseName},{texType},{texIndex}. E.g.
        /// "$clr.diffuse,0,0" or "$tex.file,1,0". This is the name that is used as the material dictionary key.
        /// </summary>
        /// <param name="baseName">Key basename, this must not be null or empty</param>
        /// <param name="texType">Texture type; non-texture properties should leave this <see cref="TextureType.None"/></param>
        /// <param name="texIndex">Texture index; non-texture properties should leave this zero.</param>
        /// <returns>The fully qualified name</returns>
        public static String CreateFullyQualifiedName(String baseName, TextureType texType, int texIndex) {
            if(String.IsNullOrEmpty(baseName))
                return String.Empty;

            return String.Format("{0},{1},{2}", baseName, (int) texType, texIndex);
        }

        /// <summary>
        /// Gets the non-texture properties contained in this Material. The name should be
        /// the "base name", as in it should not contain texture type/texture index information. E.g. "$clr.diffuse" rather than "$clr.diffuse,0,0". The extra
        /// data will be filled in automatically.
        /// </summary>
        /// <param name="baseName">Key basename</param>
        /// <returns>The material property, if it exists</returns>
        public MaterialProperty GetNonTextureProperty(String baseName) {
            if(String.IsNullOrEmpty(baseName)) {
                return null;
            }
            String fullyQualifiedName = CreateFullyQualifiedName(baseName, TextureType.None, 0);
            return GetProperty(fullyQualifiedName);
        }

        /// <summary>
        /// Gets the material property. All the input parameters are combined into the fully qualified name: {baseName},{texType},{texIndex}. E.g.
        /// "$clr.diffuse,0,0" or "$tex.file,1,0".
        /// </summary>
        /// <param name="baseName">Key basename</param>
        /// <param name="texType">Texture type; non-texture properties should leave this <see cref="TextureType.None"/></param>
        /// <param name="texIndex">Texture index; non-texture properties should leave this zero.</param>
        /// <returns>The material property, if it exists</returns>
        public MaterialProperty GetProperty(String baseName, TextureType texType, int texIndex) {
            if(String.IsNullOrEmpty(baseName)) {
                return null;
            }
            String fullyQualifiedName = CreateFullyQualifiedName(baseName, texType, texIndex);
            return GetProperty(fullyQualifiedName);
        }

        /// <summary>
        /// Gets the material property by its fully qualified name. The format is: {baseName},{texType},{texIndex}. E.g.
        /// "$clr.diffuse,0,0" or "$tex.file,1,0".
        /// </summary>
        /// <param name="fullyQualifiedName">Fully qualified name of the property</param>
        /// <returns>The material property, if it exists</returns>
        public MaterialProperty GetProperty(String fullyQualifiedName) {
            if(String.IsNullOrEmpty(fullyQualifiedName)) {
                return null;
            }
            MaterialProperty prop;
            if(!m_properties.TryGetValue(fullyQualifiedName, out prop)) {
                return null;
            }
            return prop;
        }

        /// <summary>
        /// Checks if the material has the specified non-texture property. The name should be
        /// the "base name", as in it should not contain texture type/texture index information. E.g. "$clr.diffuse" rather than "$clr.diffuse,0,0". The extra
        /// data will be filled in automatically.
        /// </summary>
        /// <param name="baseName"></param>
        /// <returns></returns>
        public bool HasNonTextureProperty(String baseName) {
            if(String.IsNullOrEmpty(baseName)) {
                return false;
            }
            String fullyQualifiedName = CreateFullyQualifiedName(baseName, TextureType.None, 0);
            return HasProperty(fullyQualifiedName);
        }

        /// <summary>
        /// Checks if the material has the specified property. All the input parameters are combined into the fully qualified name: {baseName},{texType},{texIndex}. E.g.
        /// "$clr.diffuse,0,0" or "$tex.file,1,0".
        /// </summary>
        /// <param name="baseName">Key basename</param>
        /// <param name="texType">Texture type; non-texture properties should leave this <see cref="TextureType.None"/></param>
        /// <param name="texIndex">Texture index; non-texture properties should leave this zero.</param>
        /// <returns>True if the property exists, false otherwise.</returns>
        public bool HasProperty(String baseName, TextureType texType, int texIndex) {
            if(String.IsNullOrEmpty(baseName)) {
                return false;
            }

            String fullyQualifiedName = CreateFullyQualifiedName(baseName, texType, texIndex);
            return HasProperty(fullyQualifiedName);
        }

        /// <summary>
        /// Checks if the material has the specified property by looking up its fully qualified name. The format is: {baseName},{texType},{texIndex}. E.g.
        /// "$clr.diffuse,0,0" or "$tex.file,1,0".
        /// </summary>
        /// <param name="fullyQualifiedName">Fully qualified name of the property</param>
        /// <returns>True if the property exists, false otherwise.</returns>
        public bool HasProperty(String fullyQualifiedName) {
            if(String.IsNullOrEmpty(fullyQualifiedName)) {
                return false;
            }
            return m_properties.ContainsKey(fullyQualifiedName);
        }

        /// <summary>
        /// Adds a property to this material.
        /// </summary>
        /// <param name="matProp">Material property</param>
        /// <returns>True if the property was successfully added, false otherwise (e.g. null or key already present).</returns>
        public bool AddProperty(MaterialProperty matProp) {
            if(matProp == null)
                return false;

            if(m_properties.ContainsKey(matProp.FullyQualifiedName))
                return false;

            m_properties.Add(matProp.FullyQualifiedName, matProp);

            return true;
        }

        /// <summary>
        /// Removes a non-texture property from the material.
        /// </summary>
        /// <param name="baseName">Property name</param>
        /// <returns>True if the property was removed, false otherwise</returns>
        public bool RemoveNonTextureProperty(String baseName) {
            if(String.IsNullOrEmpty(baseName))
                return false;

            return RemoveProperty(CreateFullyQualifiedName(baseName, TextureType.None, 0));
        }

        /// <summary>
        /// Removes a property from the material.
        /// </summary>
        /// <param name="baseName">Name of the property</param>
        /// <param name="texType">Property texture type</param>
        /// <param name="texIndex">Property texture index</param>
        /// <returns>True if the property was removed, false otherwise</returns>
        public bool RemoveProperty(String baseName, TextureType texType, int texIndex) {
            if(String.IsNullOrEmpty(baseName))
                return false;

            return RemoveProperty(CreateFullyQualifiedName(baseName, texType, texIndex));
        }

        /// <summary>
        /// Removes a property from the material.
        /// </summary>
        /// <param name="fullyQualifiedName">Fully qualified name of the property ({basename},{texType},{texIndex})</param>
        /// <returns></returns>
        public bool RemoveProperty(String fullyQualifiedName) {
            if(String.IsNullOrEmpty(fullyQualifiedName))
                return false;

            return m_properties.Remove(fullyQualifiedName);
        }

        /// <summary>
        /// Removes all properties from the material;
        /// </summary>
        public void Clear() {
            m_properties.Clear();
        }

        /// <summary>
        /// Gets -all- properties contained in the Material.
        /// </summary>
        /// <returns>All properties in the material property map.</returns>
        public MaterialProperty[] GetAllProperties() {
            return m_properties.Values.ToArray<MaterialProperty>();
        }

        /// <summary>
        /// Gets all the number of textures that are of the specified texture type.
        /// </summary>
        /// <param name="texType">Texture type</param>
        /// <returns>Texture count</returns>
        public int GetMaterialTextureCount(TextureType texType) {
            int count = 0;
            foreach(KeyValuePair<String, MaterialProperty> kv in m_properties) {
                MaterialProperty matProp = kv.Value;

                if(matProp.Name.StartsWith(AiMatKeys.TEXTURE_BASE) && matProp.TextureType == texType) {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Adds a texture to the material - this bulk creates a property for each field. This will
        /// either create properties or overwrite existing properties. If the texture has no
        /// file path, nothing is added.
        /// </summary>
        /// <param name="texture">Texture to add</param>
        /// <returns>True if the texture properties were added or modified</returns>
        public bool AddMaterialTexture(ref TextureSlot texture) {
            return AddMaterialTexture(ref texture, false);
        }

        /// <summary>
        /// Adds a texture to the material - this bulk creates a property for each field. This will
        /// either create properties or overwrite existing properties. If the texture has no
        /// file path, nothing is added.
        /// </summary>
        /// <param name="texture">Texture to add</param>
        /// <param name="onlySetFilePath">True to only set the texture's file path, false otherwise</param>
        /// <returns>True if the texture properties were added or modified</returns>
        public bool AddMaterialTexture(ref TextureSlot texture, bool onlySetFilePath) {
            if(String.IsNullOrEmpty(texture.FilePath))
                return false;

            TextureType texType = texture.TextureType;
            int texIndex = texture.TextureIndex;
            
            String texName = CreateFullyQualifiedName(AiMatKeys.TEXTURE_BASE, texType, texIndex);

            MaterialProperty texNameProp = GetProperty(texName);

            if(texNameProp == null)
                AddProperty(new MaterialProperty(texName, texture.FilePath));
            else
                texNameProp.SetStringValue(texture.FilePath);

            if(onlySetFilePath)
                return true;

            String mappingName = CreateFullyQualifiedName(AiMatKeys.MAPPING_BASE, texType, texIndex);
            String uvIndexName = CreateFullyQualifiedName(AiMatKeys.UVWSRC_BASE, texType, texIndex);
            String blendFactorName = CreateFullyQualifiedName(AiMatKeys.TEXBLEND_BASE, texType, texIndex);
            String texOpName = CreateFullyQualifiedName(AiMatKeys.TEXOP_BASE, texType, texIndex);
            String uMapModeName = CreateFullyQualifiedName(AiMatKeys.MAPPINGMODE_U_BASE, texType, texIndex);
            String vMapModeName = CreateFullyQualifiedName(AiMatKeys.MAPPINGMODE_V_BASE, texType, texIndex);
            String texFlagsName = CreateFullyQualifiedName(AiMatKeys.TEXFLAGS_BASE, texType, texIndex);

            MaterialProperty mappingNameProp = GetProperty(mappingName);
            MaterialProperty uvIndexNameProp = GetProperty(uvIndexName);
            MaterialProperty blendFactorNameProp = GetProperty(blendFactorName);
            MaterialProperty texOpNameProp = GetProperty(texOpName);
            MaterialProperty uMapModeNameProp = GetProperty(uMapModeName);
            MaterialProperty vMapModeNameProp = GetProperty(vMapModeName);
            MaterialProperty texFlagsNameProp = GetProperty(texFlagsName);

            if(mappingNameProp == null)
                AddProperty(new MaterialProperty(mappingName, (int) texture.Mapping));
            else
                mappingNameProp.SetIntegerValue((int) texture.Mapping);

            if(uvIndexNameProp == null)
                AddProperty(new MaterialProperty(uvIndexName, texture.UVIndex));
            else
                uvIndexNameProp.SetIntegerValue(texture.UVIndex);

            if(blendFactorNameProp == null)
                AddProperty(new MaterialProperty(blendFactorName, texture.BlendFactor));
            else
                blendFactorNameProp.SetFloatValue(texture.BlendFactor);

            if(texOpNameProp == null)
                AddProperty(new MaterialProperty(texOpName, (int) texture.Operation));
            else
                texOpNameProp.SetIntegerValue((int) texture.Operation);

            if(uMapModeNameProp == null)
                AddProperty(new MaterialProperty(uMapModeName, (int) texture.WrapModeU));
            else
                uMapModeNameProp.SetIntegerValue((int) texture.WrapModeU);

            if(vMapModeNameProp == null)
                AddProperty(new MaterialProperty(vMapModeName, (int) texture.WrapModeV));
            else
                vMapModeNameProp.SetIntegerValue((int) texture.WrapModeV);

            if(texFlagsNameProp == null)
                AddProperty(new MaterialProperty(texFlagsName, texture.Flags));
            else
                texFlagsNameProp.SetIntegerValue(texture.Flags);

            return true;
        }

        /// <summary>
        /// Removes a texture from the material - this bulk removes a property for each field.
        /// If the texture has no file path, nothing is removed
        /// </summary>
        /// <param name="texture">Texture to remove</param>
        public bool RemoveMaterialTexture(ref TextureSlot texture) {
            if(String.IsNullOrEmpty(texture.FilePath))
                return false;

            TextureType texType = texture.TextureType;
            int texIndex = texture.TextureIndex;

            String texName = CreateFullyQualifiedName(AiMatKeys.TEXTURE_BASE, texType, texIndex);
            String mappingName = CreateFullyQualifiedName(AiMatKeys.MAPPING_BASE, texType, texIndex);
            String uvIndexName = CreateFullyQualifiedName(AiMatKeys.UVWSRC_BASE, texType, texIndex);
            String blendFactorName = CreateFullyQualifiedName(AiMatKeys.TEXBLEND_BASE, texType, texIndex);
            String texOpName = CreateFullyQualifiedName(AiMatKeys.TEXOP_BASE, texType, texIndex);
            String uMapModeName = CreateFullyQualifiedName(AiMatKeys.MAPPINGMODE_U_BASE, texType, texIndex);
            String vMapModeName = CreateFullyQualifiedName(AiMatKeys.MAPPINGMODE_V_BASE, texType, texIndex);
            String texFlagsName = CreateFullyQualifiedName(AiMatKeys.TEXFLAGS_BASE, texType, texIndex);

            RemoveProperty(texName);
            RemoveProperty(mappingName);
            RemoveProperty(uvIndexName);
            RemoveProperty(blendFactorName);
            RemoveProperty(texOpName);
            RemoveProperty(uMapModeName);
            RemoveProperty(vMapModeName);
            RemoveProperty(texFlagsName);

            return true;
        }

        /// <summary>
        /// Gets a texture that corresponds to the type/index.
        /// </summary>
        /// <param name="texType">Texture type</param>
        /// <param name="texIndex">Texture index</param>
        /// <param name="texture">Texture description</param>
        /// <returns>True if the texture was found in the material</returns>
        public bool GetMaterialTexture(TextureType texType, int texIndex, out TextureSlot texture) {
            texture = new TextureSlot();

            String texName = CreateFullyQualifiedName(AiMatKeys.TEXTURE_BASE, texType, texIndex);

            MaterialProperty texNameProp = GetProperty(texName);

            //This one is necessary, the rest are optional
            if(texNameProp == null)
                return false;

            String mappingName = CreateFullyQualifiedName(AiMatKeys.MAPPING_BASE, texType, texIndex);
            String uvIndexName = CreateFullyQualifiedName(AiMatKeys.UVWSRC_BASE, texType, texIndex);
            String blendFactorName = CreateFullyQualifiedName(AiMatKeys.TEXBLEND_BASE, texType, texIndex);
            String texOpName = CreateFullyQualifiedName(AiMatKeys.TEXOP_BASE, texType, texIndex);
            String uMapModeName = CreateFullyQualifiedName(AiMatKeys.MAPPINGMODE_U_BASE, texType, texIndex);
            String vMapModeName = CreateFullyQualifiedName(AiMatKeys.MAPPINGMODE_V_BASE, texType, texIndex);
            String texFlagsName = CreateFullyQualifiedName(AiMatKeys.TEXFLAGS_BASE, texType, texIndex);

            MaterialProperty mappingNameProp = GetProperty(mappingName);
            MaterialProperty uvIndexNameProp = GetProperty(uvIndexName);
            MaterialProperty blendFactorNameProp = GetProperty(blendFactorName);
            MaterialProperty texOpNameProp = GetProperty(texOpName);
            MaterialProperty uMapModeNameProp = GetProperty(uMapModeName);
            MaterialProperty vMapModeNameProp = GetProperty(vMapModeName);
            MaterialProperty texFlagsNameProp = GetProperty(texFlagsName);

            texture.FilePath = texNameProp.GetStringValue();
            texture.TextureType = texType;
            texture.TextureIndex = texIndex;
            texture.Mapping = (mappingNameProp != null) ? (TextureMapping) mappingNameProp.GetIntegerValue() : TextureMapping.FromUV;
            texture.UVIndex = (uvIndexNameProp != null) ? uvIndexNameProp.GetIntegerValue() : 0;
            texture.BlendFactor = (blendFactorNameProp != null) ? blendFactorNameProp.GetFloatValue() : 0.0f;
            texture.Operation = (texOpNameProp != null) ? (TextureOperation) texOpNameProp.GetIntegerValue() : 0;
            texture.WrapModeU = (uMapModeNameProp != null) ? (TextureWrapMode) uMapModeNameProp.GetIntegerValue() : TextureWrapMode.Wrap;
            texture.WrapModeV = (vMapModeNameProp != null) ? (TextureWrapMode) vMapModeNameProp.GetIntegerValue() : TextureWrapMode.Wrap;
            texture.Flags = (texFlagsNameProp != null) ? texFlagsNameProp.GetIntegerValue() : 0;

            return true;
        }

        /// <summary>
        /// Gets all textures that correspond to the type.
        /// </summary>
        /// <param name="type">Texture type</param>
        /// <returns>The array of textures</returns>
        public TextureSlot[] GetMaterialTextures(TextureType type) {
            int count = GetMaterialTextureCount(type);

            if(count == 0)
                return new TextureSlot[0];

            TextureSlot[] textures = new TextureSlot[count];

            for(int i = 0; i < count; i++) {
                TextureSlot tex;
                GetMaterialTexture(type, i, out tex);
                textures[i] = tex;
            }

            return textures;
        }

        /// <summary>
        /// Gets all textures in the material.
        /// </summary>
        /// <returns>The array of textures</returns>
        public TextureSlot[] GetAllMaterialTextures() {
            List<TextureSlot> textures = new List<TextureSlot>();
            TextureType[] types = Enum.GetValues(typeof(TextureType)) as TextureType[];

            foreach(TextureType texType in types) {
                textures.AddRange(GetMaterialTextures(texType));
            }

            return textures.ToArray();
        }

        #region IMarshalable Implementation

        /// <summary>
        /// Gets if the native value type is blittable (that is, does not require marshaling by the runtime, e.g. has MarshalAs attributes).
        /// </summary>
        bool IMarshalable<Material, AiMaterial>.IsNativeBlittable {
            get { return true; }
        }

        /// <summary>
        /// Writes the managed data to the native value.
        /// </summary>
        /// <param name="thisPtr">Optional pointer to the memory that will hold the native value.</param>
        /// <param name="nativeValue">Output native value</param>
        void IMarshalable<Material, AiMaterial>.ToNative(IntPtr thisPtr, out AiMaterial nativeValue) {
            nativeValue.NumAllocated = nativeValue.NumProperties = (uint) m_properties.Count;
            nativeValue.Properties = IntPtr.Zero;

            if(m_properties.Count > 0)
                nativeValue.Properties = MemoryHelper.ToNativeArray<MaterialProperty, AiMaterialProperty>(m_properties.Values.ToArray<MaterialProperty>(), true);
        }

        /// <summary>
        /// Reads the unmanaged data from the native value.
        /// </summary>
        /// <param name="nativeValue">Input native value</param>
        void IMarshalable<Material, AiMaterial>.FromNative(ref AiMaterial nativeValue) {
            Clear();

            if(nativeValue.NumProperties > 0 && nativeValue.Properties != IntPtr.Zero) {
                MaterialProperty[] matProps = MemoryHelper.FromNativeArray<MaterialProperty, AiMaterialProperty>(nativeValue.Properties, (int) nativeValue.NumProperties, true);

                foreach(MaterialProperty matProp in matProps) {
                    AddProperty(matProp);
                }
            }
        }

        /// <summary>
        /// Frees unmanaged memory created by <see cref="ToNative"/>.
        /// </summary>
        /// <param name="nativeValue">Native value to free</param>
        /// <param name="freeNative">True if the unmanaged memory should be freed, false otherwise.</param>
        public static void FreeNative(IntPtr nativeValue, bool freeNative) {
            if(nativeValue == IntPtr.Zero)
                return;

            AiMaterial aiMaterial = MemoryHelper.Read<AiMaterial>(nativeValue);

            if(aiMaterial.NumAllocated > 0 && aiMaterial.Properties != IntPtr.Zero) 
                MemoryHelper.FreeNativeArray<AiMaterialProperty>(aiMaterial.Properties, (int) aiMaterial.NumProperties, MaterialProperty.FreeNative, true);

            if(freeNative)
                MemoryHelper.FreeMemory(nativeValue);
        }

        #endregion
    }
}
