namespace Nuke.Common.Tools.DotNet.Workload.Commands
{
    /// <summary>
    ///
    /// </summary>
    public enum Workload : short
    {
        /// <summary>
        /// Display information about installed workloads.
        /// </summary>
        Info = -2,
        /// <summary>
        /// Uninstall one or more workloads.
        /// </summary>
        Uninstall = -1,
        /// <summary>
        ///  Install one or more workloads.
        /// </summary>
        Install = 1,
        /// <summary>
        /// Update all installed workloads.
        /// </summary>
        Update = 2,
        /// <summary>
        /// List workloads available.
        /// </summary>
        List = 3,
        /// <summary>
        /// Search for available workloads.
        /// </summary>
        Search = 4,
        /// <summary>
        /// Repair workload installations.
        /// </summary>
        Repair = 5,
        /// <summary>
        /// Restore workloads required for a project.
        /// </summary>
        Restore = 6,
        /// <summary>
        /// Removes workload components that may have been left behind from previous updates and uninstallations.
        /// </summary>
        Clean = 7
    }
}
