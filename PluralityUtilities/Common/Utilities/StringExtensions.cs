namespace PluralityUtilities.AutoHotkeyScripts.Utilities
{
	public static class StringExtensions
	{
		public static string GetDirectory(this string filePath)
		{
			var pathEnd = Math.Max(filePath.LastIndexOf('/'), filePath.LastIndexOf('\\'));
			if (pathEnd < 0)
			{
				return string.Empty;
			}
			return filePath.Substring(0, filePath.Length - pathEnd);
		}

		public static string GetFileName(this string filePath)
		{
			var pathEnd = Math.Max(filePath.LastIndexOf('/'), filePath.LastIndexOf('\\'));
			if (pathEnd < 0)
			{
				return filePath;
			}
			return filePath.Substring(pathEnd + 1, filePath.Length - pathEnd);
		}

		public static string RemoveFileExtension(this string filePath)
		{
			var extensionStart = filePath.LastIndexOf('.');
			if (extensionStart < 0)
			{
				return filePath;
			}
			return filePath.Substring(0, filePath.Length - extensionStart);
		}
	}
}