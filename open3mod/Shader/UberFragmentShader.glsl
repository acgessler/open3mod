///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [UberFragmentShader.glsl]
// (c) 2012-2013, Open3Mod Contributors
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

// note: all lighting calculations done in modelview space
uniform sampler2D Texture0;

uniform vec3 LightDiffuse_SceneBrightness;
uniform vec3 LightSpecular;
uniform vec3 LightDirection; // modelview space, norm.

uniform vec4 MaterialDiffuse_Alpha;
uniform vec4 MaterialSpecular_Shininess;
uniform vec3 MaterialAmbient;
uniform vec3 MaterialEmissive;

varying vec3 position;
varying vec3 normal; 

#ifdef HAS_COLOR_MAP
varying vec2 texCoord; 
#endif
 
#ifdef HAS_VERTEX_COLOR
varying vec3 vertexColor; 
#endif


void main(void) 
{
  vec3 diffuse = MaterialDiffuse_Alpha.xyz;

#ifdef HAS_COLOR_MAP
  diffuse *= texture2D(Texture0, texCoord.xy);
#endif

#ifdef HAS_VERTEX_COLOR
  diffuse *= vertexColor;
#endif

  diffuse = (dot(normal, LightDirection) * diffuse + MaterialAmbient) * LightDiffuse_SceneBrightness.xyz;

  vec3 specular = vec3(0,0,0);

#ifdef HAS_PHONG_SPECULAR_SHADING
  vec3 eyeDir = normalize(-position);
  vec3 r = normalize(reflect(-LightDirection, normal));
  specular = LightSpecular * pow(max(dot(r, eyeDir), 0.0), MaterialSpecular_Shininess.w);	
#endif

  gl_FragColor = (diffuse + specular + MaterialEmissive) * LightDiffuse_SceneBrightness.w;
}

