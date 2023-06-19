using Microsoft.Extensions.Logging;
using MtconnectTranspiler.Sinks.ScribanTemplates;

namespace MtconnectTranspiler.Sinks.JsonSchema
{
    /// <summary>
    /// A base class for transpiling the MTConnect Standard SysML model into JSON-Schema files.
    /// </summary>
    public abstract class JsonSchemaTranspiler : ScribanTranspiler
    {
        /// <summary>
        /// Constructs a new instance of the transpiler that can transpile the model into JSON-Schema files.
        /// </summary>
        /// <param name="projectPath"><inheritdoc cref="ScribanTranspiler.ProjectPath" path="/summary"/></param>
        /// <param name="logger"><inheritdoc cref="ILogger"/></param>
        protected JsonSchemaTranspiler(string projectPath, ILogger<ITranspilerSink> logger = null) : base(projectPath, logger)
        {
        }
    }
}
