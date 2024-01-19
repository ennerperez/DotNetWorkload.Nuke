using System.IO;
using System.Linq;
using Xunit;
using Xunit.Extensions.Ordering;
using static Nuke.Common.Tools.DotNet.Workload.Tasks;
namespace DotNetWorkload.Nuke.Tests
{

    [Order(1)]
    public class WorkloadTest
    {
        [Fact, Order(1)]
        public void Search()
        {
            var result = DotNetWorkloadSearch("a");
            Assert.True(result.Any());
        }

        [Fact, Order(2)]
        public void SearchAsList()
        {
            var result = DotNetWorkloadSearchAsList("a");
            Assert.True(result.Any());
        }

        [Fact, Order(3)]
        public void List()
        {
            var result = DotNetWorkloadList();
            Assert.True(result.Any());
        }

        [Fact, Order(4)]
        public void ListAsList()
        {
            var result = DotNetWorkloadListAsList();
            Assert.True(result.Any());
        }

        [Fact, Order(5)]
        public void RestoreSolution()
        {
            var path = Path.GetFullPath(Path.Combine(Methods.GetSolutionDir(), "DotNetWorkload.Nuke.sln"));
            var result = DotNetWorkloadRestore(path);
            Assert.Contains(result, m => m.Text.Contains("Successfully installed workload(s)"));
        }

        [Fact, Order(6)]
        public void RestoreProject()
        {
            var path = Path.GetFullPath(Path.Combine(Methods.GetSolutionDir(), "src", "DotNetWorkload.Nuke", "DotNetWorkload.Nuke.csproj"));
            var result = DotNetWorkloadRestore(path);
            Assert.Contains(result, m => m.Text.Contains("Successfully installed workload(s)"));
        }
    }

}
