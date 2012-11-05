using System;
using Assimp.Unmanaged;

namespace Assimp {
    /// <summary>
    /// Describes a blob of exported scene data. Blobs can be nested - each blob may reference another blob, which in
    /// turn can reference another and so on. This is used to allow exporters to write more than one output for a given
    /// scene, such as material files. Existence of such files depends on the format.
    /// </summary>
    public sealed class ExportDataBlob {
        private String m_name;
        private byte[] m_data;
        private ExportDataBlob m_next;

        /// <summary>
        /// Gets the name of the blob. The first and primary blob always has an empty string for a name. Auxillary files
        /// that are nested will have names.
        /// </summary>
        public String Name {
            get {
                return m_name;
            }
        }

        /// <summary>
        /// Get the blob data.
        /// </summary>
        public byte[] Data {
            get {
                return m_data;
            }
        }

        /// <summary>
        /// Gets the next data blob.
        /// </summary>
        public ExportDataBlob NextBlob {
            get {
                return m_next;
            }
        }

        /// <summary>
        /// Creates a new ExportDataBlob.
        /// </summary>
        /// <param name="dataBlob">Unmanaged structure.</param>
        internal ExportDataBlob(ref AiExportDataBlob dataBlob) {
            m_name = dataBlob.Name.GetString();
            m_data = MemoryHelper.MarshalArray<byte>(dataBlob.Data, dataBlob.Size.ToInt32());
            m_next = null;

            if(dataBlob.NextBlob != IntPtr.Zero) {
                AiExportDataBlob nextBlob = MemoryHelper.MarshalStructure<AiExportDataBlob>(dataBlob.NextBlob);
                m_next = new ExportDataBlob(ref nextBlob);
            }
        }
    }
}
