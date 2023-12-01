using System;
using System.Diagnostics;
using System.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using System.IO;
using System.Text.RegularExpressions;
using Serilog;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Pack);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")] public readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] public readonly Solution Solution;

    string _author = "Enner Pérez";
    Version _version = new("1.0.0.0");
    string _hash = string.Empty;
    string _tags = "build automation continuous-integration tools orchestration";
    string _projectUrl = "https://github.com/ennerperez/DotNetWorkload.Nuke";

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath PublishDirectory => RootDirectory / "publish";
    AbsolutePath ArtifactsDirectory => RootDirectory / "output";

    Target Prepare => d => d
        .Before(Compile)
        .Executes(() =>
        {
            #region Version

            var assemblyInfoVersionFile = Path.Combine(SourceDirectory, path2: ".files", path3: "AssemblyInfo.Version.cs");
            if (File.Exists(assemblyInfoVersionFile))
            {

                Log.Information(messageTemplate: "Patching: {File}", assemblyInfoVersionFile);

                using (var gitTag = new Process())
                {
                    gitTag.StartInfo = new ProcessStartInfo(fileName: "git", arguments: "tag --sort=-v:refname")
                    {
                        WorkingDirectory = SourceDirectory, RedirectStandardOutput = true, UseShellExecute = false
                    };
                    gitTag.Start();
                    var value = gitTag.StandardOutput.ReadToEnd().Trim();
                    value = new Regex(pattern: @"((?:[0-9]{1,}\.{0,}){1,})", RegexOptions.Compiled).Match(value).Captures.LastOrDefault()?.Value;
                    if (value != null)
                    {
                        _version = Version.Parse(value);
                    }

                    gitTag.WaitForExit();
                }

                using (var gitLog = new Process())
                {
                    gitLog.StartInfo = new ProcessStartInfo(fileName: "git", arguments: "rev-parse --verify HEAD")
                    {
                        WorkingDirectory = SourceDirectory, RedirectStandardOutput = true, UseShellExecute = false
                    };
                    gitLog.Start();
                    _hash = gitLog.StandardOutput.ReadLine()?.Trim().Split(separator: " ", StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                    gitLog.WaitForExit();
                }

                if (_version != null)
                {
                    var content = File.ReadAllText(assemblyInfoVersionFile);
                    var assemblyVersionRegEx = new Regex(pattern: @"\[assembly: AssemblyVersion\(.*\)\]", RegexOptions.Compiled);
                    var assemblyFileVersionRegEx = new Regex(pattern: @"\[assembly: AssemblyFileVersion\(.*\)\]", RegexOptions.Compiled);
                    var assemblyInformationalVersionRegEx = new Regex(pattern: @"\[assembly: AssemblyInformationalVersion\(.*\)\]", RegexOptions.Compiled);

                    content = assemblyVersionRegEx.Replace(content, $"[assembly: AssemblyVersion(\"{_version}\")]");
                    content = assemblyFileVersionRegEx.Replace(content, $"[assembly: AssemblyFileVersion(\"{_version}\")]");
                    content = assemblyInformationalVersionRegEx.Replace(content, $"[assembly: AssemblyInformationalVersion(\"{_version:3}+{_hash}\")]");

                    File.WriteAllText(assemblyInfoVersionFile, content);

                    Log.Information(messageTemplate: "Version: {Version}", _version);
                    Log.Information(messageTemplate: "Hash: {Hash}", _hash);

                }
                else
                {
                    Log.Warning("Version was not found");
                }
            }

            #endregion

        });

    Target Clean => d => d
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach((path) => path.DeleteDirectory());
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach((path) => path.DeleteDirectory());
            AbsolutePath.Create(PublishDirectory).CreateOrCleanDirectory();
            AbsolutePath.Create(ArtifactsDirectory).CreateOrCleanDirectory();
        });

    Target Restore => d => d
        .After(Prepare)
        .Executes(() =>
        {
            DotNetToolRestore();
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => d => d
        .DependsOn(Clean)
        .DependsOn(Restore)
        .DependsOn(Prepare)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target Pack => d => d
        .DependsOn(Compile)
        .Executes(() =>
        {
            var projectInfo = Solution.GetAllProjects("*.Nuke").Where(m => !m.Name.Contains("Tests")).FirstOrDefault();
            if (projectInfo != null)
            {
                var version = "1.0.0";
                var assemblyInfoVersionFile = Path.Combine(projectInfo.Directory, "Properties", "AssemblyInfo.cs");
                if (File.Exists(assemblyInfoVersionFile))
                {
                    var content = File.ReadAllText(assemblyInfoVersionFile);
                    var regex = Regex.Match(content, "AssemblyVersion\\(\"(.*)\"\\)", RegexOptions.Compiled);
                    if (regex.Success && regex.Groups.Count > 1) version = regex.Groups[1].Value;
                }

                DotNetPack(s => s
                    .EnableNoBuild()
                    .EnableNoRestore()
                    .SetProject(projectInfo)
                    .SetConfiguration(Configuration)
                    .AddProperty("Icon", "icon.png")
                    .SetPackageId($"{projectInfo.Name}")
                    .SetVersion(version)
                    .SetTitle($"{projectInfo.Name}")
                    .SetAuthors(_author)
                    .SetDescription($"{projectInfo.Name}")
                    .SetCopyright(_author)
                    .SetPackageProjectUrl(_projectUrl)
                    .SetPackageTags(_tags)
                    .SetOutputDirectory($"{ArtifactsDirectory}"));
            }
        });
}
