using MtconnectTranspiler.Sinks.ScribanTemplates;
using MtconnectTranspiler.Xmi;
using MtconnectTranspiler.Xmi.UML;
using System.Collections.Generic;
using System.Linq;

namespace MtconnectTranspiler.Sinks.JsonSchema.Models
{
    public class Enum : SchemaType, IFileSource
    {
        /// <summary>
        /// Reference to the xmi:id
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// Reference to any Comments written in the SysML model to be converted into a C# format <c>&lt;summary /&gt;</c>
        /// </summary>
        public Summary Summary { get; protected set; }

        /// <summary>
        /// Internal list of <see cref="EnumItem"/>, used by <see cref="Items"/>.
        /// </summary>
        protected List<EnumItem> _items { get; set; } = new List<EnumItem>();
        /// <summary>
        /// Collection of Enum values
        /// </summary>
        public IEnumerable<EnumItem> Items => _items;

        /// <summary>
        /// Internal string, used by <see cref="Filename"/>.
        /// </summary>
        protected string _filename { get; set; }
        /// <inheritdoc />
        public string Filename
        {
            get
            {
                if (string.IsNullOrEmpty(_filename))
                    _filename = $"{Title}.json";
                return _filename;
            }
            set { _filename = value; }
        }

        /// <summary>
        /// Constructs an <see cref="Enum"/> more generically. <b>NOTE</b>: You'll need to add items manually from here.
        /// </summary>
        /// <param name="model"><inheritdoc cref="XmiDocument" path="/summary"/></param>
        /// <param name="source"><inheritdoc cref="XmiElement" path="/summary"/></param>
        /// <param name="title"><inheritdoc cref="CsharpType.Name" path="/summary"/></param>
        public Enum(XmiDocument model, XmiElement source, string title) : base(model, source)
        {
            Title = title;
            ReferenceId = source.Id;
        }

        /// <summary>
        /// <inheritdoc cref="Enum(XmiDocument, XmiElement, string)"/>
        /// </summary>
        /// <param name="model"><inheritdoc cref="Enum(XmiDocument, XmiElement, string)" path="/param[@name='model']"/></param>
        /// <param name="source">The source <see cref="UmlEnumeration"/> to convert into an <see cref="Enum"/></param>
        public Enum(XmiDocument model, UmlEnumeration source) : this(model, source, source.Name)
        {
            AddRange(model, source.Items);
        }

        /// <summary>
        /// <inheritdoc cref="Enum(XmiDocument, XmiElement, string)"/>
        /// </summary>
        /// <param name="model"><inheritdoc cref="Enum(XmiDocument, XmiElement, string)" path="/param[@name='model']"/></param>
        /// <param name="source">The source <see cref="UmlPackage"/> to convert into an <see cref="Enum"/></param>
        public Enum(XmiDocument model, UmlPackage source) : this(model, source, source.Name)
        {
            AddRange(model, source.Classes.ToList());

            if (source.Comments?.Length > 0)
                Summary = new Summary(source.Comments);
        }

        /// <summary>
        /// <inheritdoc cref="Enum(XmiDocument, XmiElement, string)"/>
        /// </summary>
        /// <param name="model"><inheritdoc cref="Enum(XmiDocument, XmiElement, string)" path="/param[@name='model']"/></param>
        /// <param name="source">The source <see cref="UmlClass"/> to convert into an <see cref="Enum"/></param>
        public Enum(XmiDocument model, UmlClass source) : this(model, source, source.Name)
        {
            AddRange(model, source.Properties.ToList());

            if (source.Comments?.Length > 0)
                Summary = new Summary(source.Comments);
        }

        /// <summary>
        /// Adds a <see cref="EnumItem"/>
        /// </summary>
        /// <param name="item">Reference to <see cref="EnumItem"/> to add to the internal list</param>
        public void Add(EnumItem item)
            => _items.Add(item);

        /// <summary>
        /// Adds a new <see cref="EnumItem"/>
        /// </summary>
        /// <param name="model">Reference to the source <see cref="XmiDocument"/></param>
        /// <param name="item">Reference to the source to convert into an <see cref="EnumItem"/></param>
        public void Add(XmiDocument model, XmiElement item)
            => Add(new EnumItem(model, item));

        /// <inheritdoc cref="Add(XmiDocument, XmiElement)"/>
        public void Add(XmiDocument model, UmlClass item)
            => Add(new EnumItem(model, item));

        /// <inheritdoc cref="Add(XmiDocument, XmiElement)"/>
        public void Add(XmiDocument model, UmlEnumerationLiteral item)
            => Add(new EnumItem(model, item));

        /// <summary>
        /// Adds a collection of <see cref="EnumItem"/>s
        /// </summary>
        /// <param name="model">Reference to the <see cref="XmiDocument"/></param>
        /// <param name="items">Collection of source items to convert into <see cref="EnumItem"/>s</param>
        public void AddRange(XmiDocument model, IEnumerable<XmiElement> items)
        {
            if (items == null) return;

            var arr = items.ToArray();
            if (arr.Length <= 0) return;

            foreach (var item in arr)
            {
                Add(model, item);
            }
        }

        /// <inheritdoc cref="AddRange(XmiDocument, IEnumerable{XmiElement})"/>
        public void AddRange(XmiDocument model, IEnumerable<UmlClass> items)
        {
            if (items == null) return;

            var arr = items.ToArray();
            if (arr.Length <= 0) return;

            foreach (var item in arr)
            {
                Add(model, item);
            }
        }

        /// <inheritdoc cref="AddRange(XmiDocument, IEnumerable{XmiElement})"/>
        public void AddRange(XmiDocument model, IEnumerable<UmlEnumerationLiteral> items)
        {
            if (items == null) return;

            var arr = items.ToArray();
            if (arr.Length <= 0) return;

            foreach (var item in arr)
            {
                Add(model, item);
            }
        }
    }
}
