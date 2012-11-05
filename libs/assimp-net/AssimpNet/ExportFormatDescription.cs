using System;
using System.Runtime.InteropServices;
using Assimp.Unmanaged;

namespace Assimp {
    /// <summary>
    /// Describes a file format which Assimp can export to.
    /// </summary>
    public sealed class ExportFormatDescription {
        private String m_formatId;
        private String m_description;
        private String m_fileExtension;

        /// <summary>
        /// Gets a short string ID to uniquely identify the export format. E.g. "dae" or "obj".
        /// </summary>
        public String FormatId {
            get {
                return m_formatId;
            }
        }

        /// <summary>
        /// Gets a short description of the file format to present to users.
        /// </summary>
        public String Description {
            get {
                return m_description;
            }
        }

        /// <summary>
        /// Gets the recommended file extension for the exported file in lower case.
        /// </summary>
        public String FileExtension {
            get {
                return m_fileExtension;
            }
        }

        /// <summary>
        /// Constructs a new ExportFormatDescription.
        /// </summary>
        /// <param name="formatDesc">Unmanaged structure</param>
        internal ExportFormatDescription(ref AiExportFormatDesc formatDesc) {
            m_formatId = Marshal.PtrToStringAnsi(formatDesc.FormatId);
            m_description = Marshal.PtrToStringAnsi(formatDesc.Description);
            m_fileExtension = Marshal.PtrToStringAnsi(formatDesc.FileExtension);
        }
    }
}
