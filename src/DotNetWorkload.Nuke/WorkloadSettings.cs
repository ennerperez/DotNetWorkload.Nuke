using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Nuke.Common.Tooling;

namespace Nuke.Common.Tools.DotNet.Workload
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class WorkloadSettings : ToolSettings
    {

        /// <summary>
        ///
        /// </summary>
        public WorkloadSettings()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="command"></param>
        public WorkloadSettings(Commands.Workload command) : this()
        {
            Command = command;
        }

        /// <summary>
        ///
        /// </summary>
        public override string ProcessToolPath => base.ProcessToolPath ?? DotNetTasks.DotNetPath;

        /// <summary>
        ///
        /// </summary>
        public Action<OutputType, string> ProcessCustomLogger => DotNetTasks.DotNetLogger;


        /// <summary>
        ///
        /// </summary>
        public virtual IReadOnlyDictionary<string, object> Properties => PropertiesInternal.AsReadOnly();
        internal Dictionary<string, object> PropertiesInternal { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        ///
        /// </summary>
        public Commands.Workload Command { get; internal set; }

        /// <summary>
        ///
        /// </summary>
        public string[] WorkloadIds { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string SearchString { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Project { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Solution { get; set; }

    }
}
