using System;
using System.IO;
using System.Text;

namespace ObjectExporter.Exporting.Formatting
{
    /// <summary>
    /// A facade to write text to an IO Stream.
    /// </summary>
    public sealed class TextStream : IDisposable
    {
        #region Data
        private readonly Stream _stream;
        private readonly Encoding _encoder;
        #endregion

        #region Constructor
        public TextStream(Stream stream)
        {
            _stream = stream;
            _encoder = Encoding.ASCII;
        }
        public TextStream(Stream stream, Encoding encoder)
        {
            _stream = stream;
            _encoder = encoder;
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Writes encoded text to the stream.
        /// </summary>
        public void Write(string text)
        {
            _stream.Write(_encoder.GetBytes(text));
        }

        /// <summary>
        /// Flushes the stream buffer and actually writes the data.
        /// </summary>
        public void Flush() => _stream.Flush();
        /// <summary>
        /// Disposes of the stream.
        /// </summary>
        public void Dispose() => _stream.Dispose();
        #endregion
    }
}
