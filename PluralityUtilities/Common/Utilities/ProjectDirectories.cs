using System.Reflection;


namespace PluralityUtilities.Common.Utilities
{
	public class ProjectDirectories
	{
		protected const string buildDirectoryName = "_build";
		protected const string inputDirectoryName = "_input";
		protected const string logDirectoryName = "_log";
		protected const string outputDirectoryName = "_output";


		public static string BaseDirectory => $"{ AppContext.BaseDirectory }";
		public static string BuildDirectory => $@"{ BaseDirectory }\{ buildDirectoryName }";
		public static string ExecutingDirectory => $"{ Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) }";
		public static string LogDirectory => $@"{ BaseDirectory }\{ logDirectoryName }";
		public static string OutputDirectory => $@"{ BaseDirectory }\{ outputDirectoryName }";
	}
}
