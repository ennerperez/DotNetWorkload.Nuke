using System.IO;
using System.Linq;
using Xunit;
using static Nuke.Common.Tools.DotNet.WorkloadTasks;
namespace DotNetWorkload.Nuke.Tests
{

    public class WorkloadTasks
    {
        [Fact]
        public void Search()
        {
            var result = DotNetWorkloadSearch("a");
            Assert.True(result.Any());
        }

        [Fact]
        public void SearchAsList()
        {
            var result = DotNetWorkloadSearchAsList("a");
            Assert.True(result.Any());
        }

        [Fact]
        public void List()
        {
            var result = DotNetWorkloadList();
            Assert.True(result.Any());
        }

        [Fact]
        public void ListAsList()
        {
            var result = DotNetWorkloadListAsList();
            Assert.True(result.Any());
        }

        [Fact]
        public void RestoreSolution()
        {
            var path = Path.GetFullPath(@"..\..\..\..\..\DotNetWorkload.Nuke.sln");
            var result = DotNetWorkloadRestore(path);
            Assert.Contains(result, m => m.Text.Contains("Successfully installed workload(s)"));
        }

        [Fact]
        public void RestoreProject()
        {
            var path = Path.GetFullPath(@"..\..\..\..\..\src\DotNetWorkload.Nuke\DotNetWorkload.Nuke.csproj");
            var result = DotNetWorkloadRestore(path);
            Assert.Contains(result, m => m.Text.Contains("Successfully installed workload(s)"));
        }
    }

}
