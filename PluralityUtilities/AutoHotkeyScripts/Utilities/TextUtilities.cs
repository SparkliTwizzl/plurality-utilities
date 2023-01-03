namespace PluralityUtilities.AutoHotkeyScripts.Utilities
{
	public class TextUtilities
	{
		public static string GetFileNameWithoutExtension(string filePath)
		{
			var extensionStart = filePath.LastIndexOf('.');
			var pathEnd = Math.Max(filePath.LastIndexOf('/'), filePath.LastIndexOf('\\'));
			var fileName = (pathEnd < 0) ? filePath : filePath.Substring(0, filePath.Length - pathEnd);
			if (extensionStart < 0)
			{
				return fileName;
			}
			return fileName.Substring(0, fileName.Length - extensionStart);
		}

		public static string GetFileName(string filePath)
		{
			var pathEnd = Math.Max(filePath.LastIndexOf('/'), filePath.LastIndexOf('\\'));
			return filePath.Substring(pathEnd + 1, filePath.Length - pathEnd);

		}

		public static string GetDirectory(string filePath)
		{
			var pathEnd = Math.Max(filePath.LastIndexOf('/'), filePath.LastIndexOf('\\'));
			if (pathEnd < 0)
			{
				return string.Empty;
			}
			return filePath.Substring(0, filePath.Length - pathEnd);
		}

		public static string RemoveFileExtension(string filePath)
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