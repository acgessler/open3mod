///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [UberVertexShader.glsl]
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


uniform mat4x4 WorldViewProjection;
uniform mat4x4 WorldView;

varying vec3 position;
varying vec3 normal; 


// use custom varyings to pass to the fragment shader to simplify porting to HLSL
#ifdef HAS_COLOR_MAP
varying vec2 texCoord; 
#endif
 
#ifdef HAS_VERTEX_COLOR
varying vec3 vertexColor; 
#endif

 
void main(void) 
{
  gl_Position		= WorldViewProjection * gl_Vertex;
 
#ifdef HAS_VERTEX_COLOR
  vertexColor = gl_Color;
#endif

#ifdef HAS_COLOR_MAP
  texCoord = gl_MultiTexCoord0; 
#endif

  normal		= normalize(((mat3x4)WorldView) * gl_Normal);
  position		= vec3(WorldView * gl_Vertex);
}

