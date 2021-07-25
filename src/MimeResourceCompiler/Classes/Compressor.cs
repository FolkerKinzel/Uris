using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace MimeResourceCompiler.Classes
{
    public class Compressor : ICompressor
    {
        private readonly ILogger _log;

        public Compressor(ILogger log) => this._log = log;

        public void RemoveUnreachableEntries(List<Entry> list)
        {
            for (int i = list.Count - 1; i >= 1; i--)
            {
                bool equalsMimeType = false;
                bool equalsExtension = false;
                Entry currentEntry = list[i];

                for (int j = i - 1; j >= 0; j--)
                {
                    Entry comp = list[j];

                    if (comp.MimeType.Equals(currentEntry.MimeType, StringComparison.Ordinal))
                    {
                        equalsMimeType = true;
                    }

                    if (comp.Extension.Equals(currentEntry.Extension, StringComparison.Ordinal))
                    {
                        equalsExtension = true;
                    }

                    if (equalsMimeType && equalsExtension)
                    {
                        list.RemoveAt(i);
                        _log.Debug("  {0} removed.", currentEntry);
                        break;
                    }
                }
            }
        }
    }
}
