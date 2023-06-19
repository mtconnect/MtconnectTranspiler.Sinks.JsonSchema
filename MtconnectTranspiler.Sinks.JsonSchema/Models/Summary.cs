using MtconnectTranspiler.Sinks.ScribanTemplates;
using MtconnectTranspiler.Xmi;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MtconnectTranspiler.Sinks.JsonSchema.Models
{
    /// <summary>
    /// Represents a code documentation block <c>&lt;summary /&gt;</c>
    /// </summary>
    [ScribanTemplate("UmlCommentsSummaryContent.scriban")]
    public class Summary
    {
        /// <summary>
        /// Collection of separate <c>&lt;summary /&gt;</c> contents
        /// </summary>
        public SummaryItem[] Items { get; }

        /// <summary>
        /// Constructs a <c>&lt;summary /&gt;</c>
        /// </summary>
        /// <param name="comments"><inheritdoc cref="OwnedComment" path="/summary"/></param>
        public Summary(OwnedComment[] comments)
        {
            Items = comments?.Select(o => new SummaryItem(o))?.ToArray();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            Regex removeLines = new Regex(@"\r\n|\n|\r", RegexOptions.Compiled);
            StringBuilder sb = new StringBuilder();
            foreach (var item in Items)
            {
                sb.Append(removeLines.Replace(item.ToString(), " "));
            }
            return sb.ToString();
        }
    }
}
