using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Serilog;

namespace MimeResourceCompiler.Classes
{
    /// <summary>
    /// Abstract base class for compiled files.
    /// </summary>
    public abstract class CompiledFile : ICompiledFile
    {
        protected readonly StreamWriter _writer;

        private const char SEPARATOR = ' ';
        private const string NEW_LINE = "\n";
        private readonly ILogger _log;
        private bool _disposedValue;

        protected CompiledFile(IStreamFactory streamFactory, ILogger log)
        {
            this._log = log;

            Stream stream = streamFactory.CreateWriteStream(FileName);
            _writer = new StreamWriter(stream)
            {
                NewLine = NEW_LINE
            };
        }

        /// <summary>
        /// The filename of the compiled file. (Without path information.)
        /// </summary>
        public abstract string FileName { get; }


        /// <summary>
        /// Writes a collection of entries to the compiled file.
        /// </summary>
        /// <param name="entries">The data to be written.</param>
        public void WriteEntries(IEnumerable<Entry> entries)
        {
            foreach (Entry entry in entries)
            {
                WriteEntry(entry);
            }
        }

        /// <summary>
        /// Writes an <see cref="Entry"/> to the compiled file.
        /// </summary>
        /// <param name="entries">The data to be written.</param>
        private void WriteEntry(Entry entry)
        {
            _writer.Write(entry.MimeType);
            _writer.Write(SEPARATOR);
            _writer.WriteLine(entry.Extension);
        }



        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: Verwalteten Zustand (verwaltete Objekte) bereinigen
                    TruncateLastEmptyRow();

                    _writer?.Close();
                    _log.Debug("{0} closed.", FileName);
                }

                // TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
                // TODO: Große Felder auf NULL setzen
                _disposedValue = true;
            }
        }

        // // TODO: Finalizer nur überschreiben, wenn "Dispose(bool disposing)" Code für die Freigabe nicht verwalteter Ressourcen enthält
        // ~ResourceFile()
        // {
        //     // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void TruncateLastEmptyRow()
        {
            _writer.Flush();

            Stream mimeFileStream = _writer.BaseStream;
            long newLength = mimeFileStream.Length - NEW_LINE.Length;
            mimeFileStream.SetLength(newLength > 0 ? newLength : 0);

            _log.Debug("Last empty row in {compiledFile} successfully truncated.", FileName);
        }
    }
}
