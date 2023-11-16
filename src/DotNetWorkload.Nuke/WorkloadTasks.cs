using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet.Workload;
using Nuke.Common.Tools.DotNet.Workload.Records;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Nuke.Common.Tools.DotNet.Workload
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    [ExcludeFromCodeCoverage]
    public static class WorkloadTasks
    {
        /// <summary>
        ///
        /// </summary>
        private static string DotNetPath => ToolPathResolver.TryGetEnvironmentExecutable("DOTNET_EXE") ?? ToolPathResolver.GetPathExecutable("dotnet");

        /// <summary>
        ///
        /// </summary>
        /// <param name="toolSettings"></param>
        /// <returns></returns>
        private static IReadOnlyCollection<Output> DotNetWorkload(WorkloadSettings toolSettings = null)
        {
            toolSettings = toolSettings ?? new WorkloadSettings();
            var sb = new StringBuilder();
            sb.Append("workload ");
            switch (toolSettings.Command)
            {
                case Commands.Workload.Uninstall:
                    sb.Append($"uninstall {string.Join(" ", toolSettings.WorkloadIds)}");
                    break;
                case Commands.Workload.Install:
                    sb.Append($"install {string.Join(" ", toolSettings.WorkloadIds)}");
                    break;
                case Commands.Workload.Update:
                    sb.Append($"update");
                    break;
                case Commands.Workload.List:
                    sb.Append($"list");
                    break;
                case Commands.Workload.Search:
                    sb.Append($"search {toolSettings.SearchString}");
                    break;
                case Commands.Workload.Repair:
                    sb.Append($"repair");
                    break;
                case Commands.Workload.Restore:
                    sb.Append($"restore {toolSettings.Solution ?? toolSettings.Project}");
                    break;
                case Commands.Workload.Clean:
                    sb.Append($"clean");
                    break;
                default:
                    sb.Append($"info");
                    break;
            }
            return DotNetTasks.DotNet(sb.ToString());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="configurator"></param>
        /// <returns></returns>
        private static IReadOnlyCollection<Output> DotNetWorkload(Configure<WorkloadSettings> configurator)
        {
            return DotNetWorkload(configurator(new WorkloadSettings()));
        }

        /// <summary>
        /// Install one or more workloads.
        /// </summary>
        /// <param name="workloadIds"></param>
        /// <returns></returns>
        public static IReadOnlyCollection<Output> DotNetWorkloadInstall(params string[] workloadIds)
        {
            return DotNetWorkload(new WorkloadSettings(Commands.Workload.Install)
                .SetWorkloadId(workloadIds));
        }

        /// <summary>
        /// Update all installed workloads.
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyCollection<Output> DotNetWorkloadUpdate()
        {
            return DotNetWorkload(new WorkloadSettings(Commands.Workload.Update));
        }

        /// <summary>
        /// List workloads available.
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyCollection<Output> DotNetWorkloadList()
        {
            return DotNetWorkload(new WorkloadSettings(Commands.Workload.List));
        }

#if NET7_0
        //TODO: Migrate to Generated Regex
        [GeneratedRegex("(.*) {1,}(.*)\\/(.*) {1,}(SDK|VS)? (.*)(:?\\, VS)?(.*)")];
        private Regex WorkloadListItem();
#endif

        /// <summary>
        /// List workloads available.
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyList<WorkloadRecord> DotNetWorkloadListAsList()
        {
            var result = DotNetWorkloadList();
            var regex = new Regex("(.*) {1,}(.*)\\/(.*) {1,}(SDK|VS)? (.*)(:?\\, VS)?(.*)", RegexOptions.Compiled);
            return result.Skip(3).Where(p => regex.Match(p.Text).Success).Select(m =>
            {
                var match = regex.Match(m.Text);
                return new WorkloadRecord(match.Groups[1].Value.Trim(), string.Empty, match.Groups[2].Value.Trim(), match.Groups[4].Value.Trim(), match.Groups[5].Value.Trim());
            }).ToList().AsReadOnly();
        }

        /// <summary>
        /// Search for available workloads.
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyCollection<Output> DotNetWorkloadSearch(string searchString)
        {
            return DotNetWorkload(new WorkloadSettings(Commands.Workload.Search)
                .SetSearchString(searchString));
        }

        /// <summary>
        /// Search for available workloads.
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyCollection<WorkloadRecord> DotNetWorkloadSearchAsList(string searchString)
        {
            var result = DotNetWorkloadSearch(searchString);
            var regex = new Regex("([A-z-0-9]{1,}) {1,}(.*)", RegexOptions.Compiled);
            return result.Skip(3).Where(p => regex.Match(p.Text).Success).Select(m =>
            {
                var match = regex.Match(m.Text);
                return new WorkloadRecord(match.Groups[1].Value.Trim(), match.Groups[2].Value.Trim());
            }).ToList().AsReadOnly();
        }

        /// <summary>
        /// Uninstall one or more workloads.
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyCollection<Output> DotNetWorkloadUninstall(params string[] workloadIds)
        {
            return DotNetWorkload(new WorkloadSettings(Commands.Workload.Uninstall)
                .SetWorkloadId(workloadIds));
        }

        /// <summary>
        /// Repair workload installations.
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyCollection<Output> DotNetWorkloadRepair()
        {
            return DotNetWorkload(new WorkloadSettings(Commands.Workload.Uninstall));
        }

        /// <summary>
        /// Restore workloads required for a project.
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyCollection<Output> DotNetWorkloadRestore(Project project)
        {
            return DotNetWorkload(new WorkloadSettings(Commands.Workload.Restore)
                .SetProject(project));
        }

        /// <summary>
        /// Restore workloads required for a solution.
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyCollection<Output> DotNetWorkloadRestore(Solution solution)
        {
            return DotNetWorkload(new WorkloadSettings(Commands.Workload.Restore)
                .SetSolution(solution));
        }

        /// <summary>
        /// Restore workloads required for a solution/project.
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyCollection<Output> DotNetWorkloadRestore(string path)
        {
            return DotNetWorkload(new WorkloadSettings(Commands.Workload.Restore)
                .SetPath(path));
        }

        /// <summary>
        /// Removes workload components that may have been left behind from previous updates and uninstallations.
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyCollection<Output> DotNetWorkloadClean()
        {
            return DotNetWorkload(new WorkloadSettings(Commands.Workload.Clean));
        }

    }
}
