using System.IO;
using System.Reflection;

namespace HockeyScoresVS
{
    internal class Utilities
    {
        private static string executingAssemblyDirectory = string.Empty;
        public static string ExecutingAssemblyDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(executingAssemblyDirectory))
                {
                    executingAssemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                }

                return executingAssemblyDirectory;
            }
        }
    }
}
