using System;

namespace Ruzzie.Mtg.Core.IO
{
    /// <summary>
    /// Metadata for a remote file.
    /// </summary>
    public interface IRemoteFileMetaData
    {
        /// <summary>
        /// Gets or sets the last modified time UTC.
        /// </summary>
        /// <value>
        /// The last modified time UTC.
        /// </value>
        DateTime? LastModifiedTimeUtc { get; set; }
    }
}