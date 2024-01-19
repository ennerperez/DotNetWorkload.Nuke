using System.IO;
namespace DotNetWorkload.Nuke.Tests
{
    internal static class Methods
    {
        internal static string GetSolutionDir()
        {
            return Path.Combine("..", "..", "..", "..", "..");
        }
    }
}
