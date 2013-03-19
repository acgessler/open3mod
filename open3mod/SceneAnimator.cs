///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [SceneAnimator.cs]
// (c) 2012-2013, Alexander C. Gessler
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using OpenTK;

namespace open3mod
{
    /// <summary>
    /// Encapsulates the animation state of a Scene. Every Scene has its own
    /// SceneAnimator, which is accessible via the Scene.SceneAnimator property
    /// </summary>
    public class SceneAnimator
    {
        /// <summary>
        /// Utility class to keep track of per-node animation state
        /// </summary>
        private sealed class NodeState
        {
            public Matrix4 LocalTransform;
            public Matrix4 GlobalTransform;
            public int ChannelIndex;

            public NodeState Parent;

            public NodeState[] Children;
        }


        private readonly Scene _scene;
        private readonly Assimp.Scene _raw;

        private int _activeAnim = -2;
        private double _animPlaybackSpeed = 1.0;
        private double _animCursor = 0.0;
        private bool _loop = true;
        private AnimEvaluator _evaluator;

        private readonly Dictionary<string, NodeState> _nodeStateByName;
        private NodeState _tree;

        private readonly Matrix4[] _boneMatrices;
        private bool _isInEndPosition;


        internal SceneAnimator(Scene scene)
        {
            _scene = scene;
            _raw = scene.Raw;
            Debug.Assert(_raw != null, "scene must already be loaded");
          
            _nodeStateByName = new Dictionary<string, NodeState>();

            int maxBoneCount = 0;
            for (int i = 0; i < _raw.MeshCount; ++i)
            {
                var bc = _raw.Meshes[i].BoneCount;
                if(bc > maxBoneCount)
                {
                    maxBoneCount = bc;
                }
            }

            _boneMatrices = new Matrix4[maxBoneCount];

            // initialize the node hierarchy
            ActiveAnimation = -1;
        }


        /// Zero-based index of the currently active animation or -1 to disable
        /// animations altogether (in which case the initial/non-animated pose
        /// is shown).
        public int ActiveAnimation
        {
            get { return _activeAnim; }

            set
            {
                Debug.Assert(value >= -1 && value < _raw.AnimationCount);

                if(value == _activeAnim)
                {
                    return;
                }

                _activeAnim = value;
                _tree = CreateNodeTree(_raw.RootNode, null);

                if (_activeAnim != -1)
                {
                    _evaluator = new AnimEvaluator(_raw.Animations[_activeAnim]);
                }
            }
        }

       

        /// <summary>
        /// Animation playback speed factor. A factor of 1 means that the current
        /// animation is played in its original speed. A value of 0
        /// animation playback.
        /// </summary>
        public double AnimationPlaybackSpeed
        {
            get { return _animPlaybackSpeed; }

            set
            {
                Debug.Assert(value >= 0);
                _animPlaybackSpeed = value;
            }
        }


        /// <summary>
        /// Current animation playback position (cursor), in seconds. Can be 
        /// assigned to. The animation cursor can exceed the duration of the
        /// animation, in which case the animation gets either looped (if 
        /// Looping is true) or it remains in its final position (otherwise).
        /// 
        /// The value need to be non-negative, though.
        /// </summary>
        public double AnimationCursor
        {
            get { return _animCursor; }

            set
            {
                Debug.Assert(value >= 0);
                _animCursor = value;

                if (!Loop && _animCursor > AnimationDuration)
                {
                    _animCursor = AnimationDuration;
                    _isInEndPosition = true;
                }
                else
                {
                    _isInEndPosition = false;
                }

                Recalculate();
            }
        }


        /// <summary>
        /// Getter for the duration of the current animation in seconds.
        /// Returns 0.0 if no animation is currently active.
        /// </summary>
        public double AnimationDuration
        {
            get
            {
                if (ActiveAnimation == -1)
                {
                    return 0.0;
                }
                var anim = _raw.Animations[ActiveAnimation];
                return anim.DurationInTicks / anim.TicksPerSecond;
            }
        }


        /// <summary>
        /// Specifies whether the animation loops (i.e. starts again as
        /// soon as its predefined duration is exceeded) or just stays
        /// in the final frame position.
        /// </summary>
        public bool Loop
        {
            get { return _loop; }
            set { 
                _loop = value;
                // necessary to update animations if needed
                AnimationCursor = _animCursor; 
            }
        }


        /// <summary>
        /// Check if a real animation is active. This is equivalent to 
        /// checking if the ActiveAnimation index is > -1.
        /// </summary>
        public bool IsAnimationActive
        {
            get { return ActiveAnimation > -1; }
        }


        /// <summary>
        /// Play animation given a time delta since the last Update()
        /// </summary>
        /// <param name="delta">Real-world time delta, in seconds</param>
        public void Update(double delta)
        {
            AnimationCursor += delta*AnimationPlaybackSpeed;
        }


        /// <summary>
        /// Obtain the current global transformation matrix for a node.
        /// </summary>
        /// <param name="name">Name of the node (must exist)</param>
        /// <param name="outTrafo">Receives the transformation matrix</param>
        public void GetGlobalTransform(string name, out Matrix4 outTrafo)
        {
            Debug.Assert(_nodeStateByName.ContainsKey(name));
            outTrafo = _nodeStateByName[name].GlobalTransform;
        }


        /// <summary>
        /// Obtain the current global transformation matrix for a node
        /// </summary>
        /// <param name="node">Node</param>
        /// <param name="outTrafo">Receives the transformation matrix</param>
        public void GetGlobalTransform(Node node, out Matrix4 outTrafo)
        {
            GetGlobalTransform(node.Name, out outTrafo);
        }


        /// <summary>
        /// Obtain the current local transformation matrix for a node
        /// </summary>
        /// <param name="name">Name of the node (must exist)</param>
        /// <param name="outTrafo">Receives the transformation matrix</param>
        public void GetLocalTransform(string name, out Matrix4 outTrafo)
        {
            Debug.Assert(_nodeStateByName.ContainsKey(name));
            outTrafo = _nodeStateByName[name].LocalTransform;
        }


        /// <summary>
        /// Obtain the current local transformation matrix for a node
        /// </summary>
        /// <param name="node">Node</param>
        /// <param name="outTrafo">Receives the transformation matrix</param>
        public void GetLocalTransform(Node node, out Matrix4 outTrafo)
        {
            GetLocalTransform(node.Name, out outTrafo);
        }


        /// <summary>
        /// Obtain the bone matrices for a given node mesh index at the
        /// current time. Calling this is costly, redundant invocations
        /// should thus be avoided.
        /// </summary>
        /// <param name="node">Node for which to query bone matrices</param>
        /// <param name="meshIndex">Index of the mesh in the mesh list of
        ///    the node (_not_ the scene-wide index)</param>
        /// <returns>For each bone of the mesh the bone transformation
        /// matrix. The returned array is only valid for the rest of
        /// the frame or till the next call to GetBoneMatricesForMesh(). 
        /// It may contain more entries than the mesh has bones, the extra entries 
        /// should be ignored in this case.</returns>
        public Matrix4[] GetBoneMatricesForMesh(Node node, int meshIndex)
        {
            Debug.Assert(node != null);
            Debug.Assert(meshIndex < node.MeshCount);

            var globalMeshIndex = node.MeshIndices[meshIndex];
            var mesh = _raw.Meshes[globalMeshIndex];

            // calculate the mesh's inverse global transform
            Matrix4 globalInverseMeshTransform;
            GetGlobalTransform( node, out globalInverseMeshTransform );
            globalInverseMeshTransform.Invert();

            // Bone matrices transform from mesh coordinates in bind pose to mesh coordinates in skinned pose
            // Therefore the formula is offsetMatrix * currentGlobalTransform * inverseCurrentMeshTransform
            for( int a = 0; a < mesh.BoneCount; ++a)
            {
                var bone = mesh.Bones[a];

                Matrix4 currentGlobalTransform;
                GetGlobalTransform( bone.Name, out currentGlobalTransform );
                _boneMatrices[a] = globalInverseMeshTransform * currentGlobalTransform * AssimpToOpenTk.FromMatrix(bone.OffsetMatrix);
                // TODO for some reason, all OpenTk matrices need a ^T - clarify our conventions somewhere
                _boneMatrices[a].Transpose();
            }
            return _boneMatrices;
        }


        private void Recalculate()
        {
            if (IsAnimationActive)
            {
                Debug.Assert(_evaluator != null);

                _evaluator.Evaluate(_animCursor, _isInEndPosition);
                CalculateTransforms(_tree, _evaluator.CurrentTransforms);
            }         
        }


        private void CalculateTransforms(NodeState node, Matrix4[] perChannelLocalTransformation)
        {
            // grab the latest local transformation from the animation channel corr. to this node
            if (node.ChannelIndex != -1)
            {
                Debug.Assert(node.ChannelIndex < perChannelLocalTransformation.Length);
                node.LocalTransform = perChannelLocalTransformation[node.ChannelIndex];
            }

            node.GlobalTransform = node.Parent != null ? node.Parent.GlobalTransform*node.LocalTransform : node.LocalTransform;

            // recursively update children
            foreach (NodeState t in node.Children)
            {
                CalculateTransforms(t, perChannelLocalTransformation);
            }
        }


        private NodeState CreateNodeTree(Node rootNode, NodeState parent)
        {
            var outNode = new NodeState {LocalTransform = AssimpToOpenTk.FromMatrix(rootNode.Transform)};
            outNode.Parent = parent;

            // calculate transforms
            outNode.GlobalTransform = parent != null ? parent.GlobalTransform * outNode.LocalTransform : outNode.LocalTransform;

            // populate by-name map to quickly map nodes to their state
            _nodeStateByName[rootNode.Name] = outNode;

            // find the index of the animation track affecting this node, if any
            outNode.ChannelIndex = -1;
            if (ActiveAnimation != -1)
            {
                var channels = _raw.Animations[ActiveAnimation].NodeAnimationChannels;
                for (int i = 0; i < channels.Length; ++i)
                {
                    if (channels[i].NodeName != rootNode.Name)
                    {
                        continue;
                    }
                    outNode.ChannelIndex = i;
                    break;
                }
            }

            outNode.Children = new NodeState[rootNode.ChildCount];

            // recursively add up children
            for (int i = 0; i < rootNode.ChildCount; ++i)
            {
                outNode.Children[i] = CreateNodeTree(rootNode.Children[i], outNode);
            }

            return outNode;
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 