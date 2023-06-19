using MtconnectTranspiler.Sinks.ScribanTemplates;
using MtconnectTranspiler.Xmi;
using System;
using System.Collections.Generic;
using System.Text;

namespace MtconnectTranspiler.Sinks.JsonSchema.Models
{
    public abstract class SchemaType : MtconnectVersionedObject
    {
        /// <summary>
        /// The intended namespace for the C# type.
        /// </summary>
        public virtual string Namespace { get; set; } = "http://json-schema.org/draft-07/schema#";

        /// <summary>
        /// Internal string, used by <see cref="Title"/>.
        /// </summary>
        protected string _title { get; set; }
        /// <summary>
        /// The intended name for the C# type.
        /// </summary>
        public virtual string Title
        {
            get
            {
                if (string.IsNullOrEmpty(_title))
                    _title = ScribanHelperMethods.ToPascalCase(base.SysML_Name);
                return _title;
            }
            set { _title = value; }
        }

        /// <summary>
        /// The value type for the schema type.
        /// </summary>
        public string ValueType { get; set; } = "string";


        /// <summary>
        /// Constructs an <see cref="SchemaType"/> more generically. <b>NOTE</b>: You'll need to add items manually from here.
        /// </summary>
        /// <param name="model"><inheritdoc cref="XmiDocument" path="/summary"/></param>
        /// <param name="source"><inheritdoc cref="XmiElement" path="/summary"/></param>
        protected SchemaType(XmiDocument model, XmiElement source) : base(model, source) { }
    }
}
