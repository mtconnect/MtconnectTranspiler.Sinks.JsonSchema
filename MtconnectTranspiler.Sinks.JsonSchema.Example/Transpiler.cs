using Microsoft.Extensions.Logging;
using MtconnectTranspiler.Contracts;
using MtconnectTranspiler.Sinks.ScribanTemplates;
using MtconnectTranspiler.Xmi.UML;
using MtconnectTranspiler.Xmi;
using MtconnectTranspiler.Sinks.JsonSchema.Example.Models;
using MtconnectTranspiler.Sinks.JsonSchema.Models;
using Scriban.Runtime;

namespace MtconnectTranspiler.Sinks.JsonSchema.Example
{
    public class CategoryFunctions : ScriptObject
    {
        public static bool CategoryContainsType(JsonEnum @enum, EnumItem item) => @enum.SubTypes.ContainsKey(item.Name);
        public static bool CategoryContainsValue(JsonEnum @enum, EnumItem item) => @enum.ValueTypes.ContainsKey(item.Name);
        public static bool EnumHasValues(JsonEnum @enum) => @enum.ValueTypes.Any();
    }
    internal class Transpiler : JsonSchemaTranspiler
    {
        public Transpiler(string projectPath, ILogger<ITranspilerSink> logger = null) : base(projectPath, logger)
        {
        }

        public override void Transpile(XmiDocument model, CancellationToken cancellationToken = default)
        {
            _logger?.LogInformation("Received MTConnectModel, beginning transpilation");

            Model.SetValue("model", model, true);

            base.TemplateContext.PushGlobal(new CategoryFunctions());

            const string DataItemNamespace = "http://json-schema.org/draft-07/schema#";
            const string DataItemValueNamespace = "http://json-schema.org/draft-07/schema#";

            // Process DataItem Types/Sub-Types
            var dataItemTypeEnums = new List<JsonEnum>();
            var valueEnums = new List<JsonEnum>();
            string[] categories = new string[] { "Sample", "Event", "Condition" };

            foreach (var category in categories)
            {
                // Get the UmlPackage for the category (ie. Samples, Events, Conditions).
                var typesPackage = MTConnectHelper.JumpToPackage(
                    model!,
                    MTConnectHelper.PackageNavigationTree.ObservationInformationModel.ObservationTypes
                    )?
                    .Packages
                    .FirstOrDefault(o => o.Name == $"{category} Types");
                if (typesPackage == null)
                {
                    _logger?.LogTrace("Couldn't find {Category} Types", category);
                    continue;
                }

                // Get all DataItem Type and SubType references
                var allTypes = typesPackage
                    .Classes;
                // Filter to get just the Type references
                var types = allTypes
                    ?.Where(o => !o!.Name!.Contains('.'));
                if (types == null)
                {
                    _logger?.LogTrace("Couldn't find type classes for {Category} Types", category);
                    continue;
                }

                // Filter and group each SubType by the relevant Type reference
                var subTypes = allTypes
                    ?.Where(o => o!.Name!.Contains('.'))
                    ?.GroupBy(o => o!.Name![..o.Name!.IndexOf(".")], o => o)
                    ?.Where(o => o.Any())
                    ?.ToDictionary(o => o.Key, o => o?.ToList());

                var categoryEnum = new JsonEnum(model!, typesPackage)
                {
                    Title = $"{category}Types",
                    Namespace = DataItemNamespace
                };

                foreach (var type in types)
                {
                    // Add type to CATEGORY enum
                    categoryEnum.Add(model, type);

                    // Find value
                    var typeResult = type!.Properties?.FirstOrDefault(o => o.Name == "result");
                    if (typeResult != null)
                    {
                        var typeValuesSysEnum = MTConnectHelper.JumpToPackage(model!, MTConnectHelper.PackageNavigationTree.Profile.DataTypes)?
                            .Enumerations
                            .GetById(typeResult.PropertyType);
                        if (typeValuesSysEnum != null && typeValuesSysEnum is UmlEnumeration)
                        {
                            var typeValuesEnum = new JsonEnum(model!, typeValuesSysEnum!)
                            {
                                Namespace = DataItemValueNamespace,
                                Title = $"{type.Name}Values"
                            };
                            foreach (var value in typeValuesEnum.Items)
                            {
                                value.Name = value.SysML_Name;
                            }
                            if (!categoryEnum.ValueTypes.ContainsKey(type.Name!)) categoryEnum.ValueTypes.Add(ScribanHelperMethods.ToUpperSnakeCode(type.Name), $"{type.Name}Values");
                            valueEnums.Add(typeValuesEnum);
                        }
                    }

                    // Add subType as enum
                    if (subTypes != null && subTypes.ContainsKey(type.Name!))
                    {
                        // Register type as having a subType in the CATEGORY enum
                        if (!categoryEnum.SubTypes.ContainsKey(type.Name!)) categoryEnum.SubTypes.Add(ScribanHelperMethods.ToUpperSnakeCode(type.Name), $"{type.Name}SubTypes");

                        var subTypeEnum = new JsonEnum(model!, type, $"{type.Name}SubTypes") { Namespace = DataItemNamespace };

                        var typeSubTypes = subTypes[type.Name!];
                        subTypeEnum.AddRange(model, typeSubTypes);

                        // Cleanup Enum names
                        foreach (var item in subTypeEnum.Items)
                        {
                            if (!item.Name.Contains('.')) continue;
                            item.Name = ScribanHelperMethods.ToUpperSnakeCode(item.Name[(item.Name.IndexOf(".") + 1)..]);
                        }

                        // Register the DataItem SubType Enum
                        dataItemTypeEnums.Add(subTypeEnum);
                    }
                }

                // Cleanup Enum names
                foreach (var item in categoryEnum.Items)
                {
                    item.Name = ScribanHelperMethods.ToUpperSnakeCode(item.Name);
                }

                // Register the DataItem Category Enum (ie. Samples, Events, Conditions)
                dataItemTypeEnums.Add(categoryEnum);
            }

            _logger?.LogInformation("Processing {Count} DataItem types/subTypes", dataItemTypeEnums.Count);

            // Process the template into enum files
            ProcessTemplate(dataItemTypeEnums, Path.Combine(ProjectPath, "Enums", "Devices", "DataItemTypes"), true);
            ProcessTemplate(valueEnums, Path.Combine(ProjectPath, "Enums", "Streams"), true);
        }
    }
}
