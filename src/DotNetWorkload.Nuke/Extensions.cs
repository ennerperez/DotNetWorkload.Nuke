using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;

// ReSharper disable PossibleNullReferenceException

namespace Nuke.Common.Tools.DotNet.Workload
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    [ExcludeFromCodeCoverage]
    public static class Extensions
    {

        /// <summary>
        ///
        /// </summary>
        /// <param name="toolSettings"></param>
        /// <param name="command"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Pure]
        internal static T SetCommand<T>(this T toolSettings, Commands.Workload command) where T : WorkloadSettings
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.Command = command;
            return toolSettings;
        }

        [Pure]
        public static T SetWorkloadId<T>(this T toolSettings, params string[] workloadIds) where T : WorkloadSettings
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.WorkloadIds = workloadIds;
            return toolSettings;
        }


        [Pure]
        public static T SetSearchString<T>(this T toolSettings, string searchString) where T : WorkloadSettings
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.SearchString = searchString;
            return toolSettings;
        }

        [Pure]
        public static T SetProject<T>(this T toolSettings, Project project) where T : WorkloadSettings
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.Project = project.Path;
            return toolSettings;
        }

        [Pure]
        public static T SetSolution<T>(this T toolSettings, Solution solution) where T : WorkloadSettings
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.Solution = solution.Path;
            return toolSettings;
        }

        [Pure]
        public static T SetPath<T>(this T toolSettings, string path) where T : WorkloadSettings
        {
            toolSettings = toolSettings.NewInstance();
            if (path.EndsWith(".sln")) toolSettings.Solution = path;
            else toolSettings.Project = path;
            return toolSettings;
        }

    }
}
