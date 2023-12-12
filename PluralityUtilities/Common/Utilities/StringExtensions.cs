namespace PluralityUtilities.Common.Utilities
{
	public static class StringExtensions
	{
		public static string GetDirectory( this string filePath )
		{
			var pathEnd = Math.Max( filePath.LastIndexOf( '/' ), filePath.LastIndexOf( '\\' ) );
			if ( pathEnd < 0 )
			{
				return string.Empty;
			}
			var directoryLength = filePath.Length - ( ( filePath.Length - pathEnd ) - 1 );
			return filePath.Substring( 0, directoryLength );
		}

		public static string GetFileName( this string filePath )
		{
			var pathEnd = Math.Max( filePath.LastIndexOf( '/' ), filePath.LastIndexOf( '\\' ) );
			if ( pathEnd < 0 )
			{
				return filePath;
			}
			var fileNameStart = pathEnd + 1;
			return filePath.Substring( fileNameStart, filePath.Length - fileNameStart );
		}

		public static string RemoveFileExtension( this string filePath )
		{
			var extensionStart = filePath.LastIndexOf( '.' );
			if ( extensionStart < 0 )
			{
				return filePath;
			}
			var length = filePath.Length - ( filePath.Length - extensionStart );
			return filePath.Substring( 0, length );
		}
	}
}