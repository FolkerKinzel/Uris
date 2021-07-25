using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Serilog;

namespace MimeResourceCompiler.Classes
{
    public abstract class ResourceParser : IResourceParser
    {
        private readonly StreamReader _reader;
        private readonly ILogger _log;
        private bool _disposedValue;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="resourceLoader">IResourceLoader</param>
        /// <param name="log">ILogger</param>
        public ResourceParser(IResourceLoader resourceLoader, ILogger log)
        {
            this._log = log;

            _log.Debug("Open the resource {0} for reading.", FileName);
            _reader = new StreamReader(resourceLoader.GetResourceStream(FileName));
        }

        public abstract string FileName { get; }


        public Entry? GetNextLine()
        {
            string? line;
            while ((line = _reader.ReadLine()) is not null)
            {
                line = line.Trim();
                if (line.StartsWith('#') || line.Length == 0)
                {
                    continue;
                }

                string[] parts = Regex.Split(line, @"\s+");

                if(parts.Length < 2)
                {
                    throw new InvalidDataException(
                        string.Format("The resource {0} contains invalid data: {1}", FileName, line));
                }
                else
                {
                    try
                    { 
                        return new Entry(parts[0], parts[1]); 
                    } 
                    catch (ArgumentException e) 
                    {
                        throw new InvalidDataException(
                            string.Format("The resource {0} contains invalid data: {1}", FileName, line), e);
                    };
                }
            }

            return null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    this._reader.Close();
                }

                // TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
                // TODO: Große Felder auf NULL setzen
                _disposedValue = true;

                _log.Debug("Resource {0} closed.", FileName);
            }
        }

        // // TODO: Finalizer nur überschreiben, wenn "Dispose(bool disposing)" Code für die Freigabe nicht verwalteter Ressourcen enthält
        // ~Addendum()
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

    }
}
