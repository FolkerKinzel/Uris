using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MimeResourceCompiler
{
    /// <summary>
    /// Represents the data of one row of the addendum.
    /// </summary>
    public record AddendumRecord(string MimeType, string Extension);
}
