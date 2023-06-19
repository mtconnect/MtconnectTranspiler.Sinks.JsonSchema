using MtconnectTranspiler.Sinks.ScribanTemplates;
using MtconnectTranspiler.Xmi;

namespace MtconnectTranspiler.Sinks.JsonSchema.Models
{
    /// <summary>
    /// Represents content for a code documentation block <c>&lt;summary /&gt;</c>
    /// </summary>
    public class SummaryItem
    {
        private OwnedComment _source;

        /// <summary>
        /// Constructs the content for <c>&lt;summary /&gt;</c>
        /// </summary>
        /// <param name="source"><inheritdoc cref="OwnedComment" path="/summary"/></param>
        public SummaryItem(OwnedComment source)
        {
            _source = source;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{ScribanHelperMethods.ToSummary(_source.Body)}";
        }
    }
}
