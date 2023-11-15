namespace Nuke.Common.Tools.DotNet.Workload.Records
{
    public record WorkloadRecord(string WorkloadId, string Description, string ManifestVersion ="", string Source = "", string SourceVersion = "");
}
