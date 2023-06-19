using MtconnectTranspiler.Sinks.ScribanTemplates;
using MtconnectTranspiler.Xmi.UML;
using MtconnectTranspiler.Xmi;

namespace MtconnectTranspiler.Sinks.JsonSchema.Example.Models
{
    [ScribanTemplate("JsonSchema.Enum.scriban")]
    public class JsonEnum : JsonSchema.Models.Enum
    {
        // NOTE: Only used for CATEGORY types that have subTypes.
        public Dictionary<string, string> SubTypes { get; set; } = new Dictionary<string, string>();

        // NOTE: Only used for CATEGORY types that have value enums.
        public Dictionary<string, string> ValueTypes { get; set; } = new Dictionary<string, string>();

        public JsonEnum(XmiDocument model, XmiElement source, string name) : base(model, source, name) { }

        public JsonEnum(XmiDocument model, UmlEnumeration source) : base(model, source) { }

        public JsonEnum(XmiDocument model, UmlPackage source) : base(model, source) { }
    }
}
