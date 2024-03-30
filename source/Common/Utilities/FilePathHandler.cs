namespace Petrichor.Common.Utilities
{
	public class FilePathHandler
	{
		public string DefaultDirectory { get; private set; }
		public string DefaultFileName { get; private set; }
		public string Directory { get; private set; }
		public string FileName { get; private set; }
		public string FilePath { get; private set; } = string.Empty;


		public FilePathHandler( string defaultDirectory, string defaultFileName )
		{
			DefaultDirectory = defaultDirectory;
			DefaultFileName = defaultFileName;
			Directory = defaultDirectory;
			FileName = defaultFileName;
		}


		/// <summary>
		/// Takes a partial or complete file path, converts it to a full path, and stores it in <see cref="FilePath"/>
		/// </summary>
		/// <param name="file">Directory path, file name, or file path to set.</param>
		public void SetFile( string file )
		{
			file = file.TrimQuotes();
			var directory = Path.GetDirectoryName( file ) ?? string.Empty;
			if ( directory != string.Empty )
			{
				Directory = directory;
			}

			var fileName = Path.GetFileName( file );
			if ( fileName != string.Empty )
			{
				FileName = fileName;
			}

			FilePath = Path.Combine( Directory, FileName );
		}
	}
}
