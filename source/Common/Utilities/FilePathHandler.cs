namespace Petrichor.Common.Utilities
{
	/// <summary>
	/// Handles file path management, including setting and storing file paths.
	/// </summary>
	public class FilePathHandler
	{
		/// <summary>
		/// The default directory for file storage.
		/// </summary>
		public string DefaultDirectory { get; private set; }
		
		/// <summary>
		/// The default file name for files in the directory.
		/// </summary>
		public string DefaultFileName { get; private set; }
		
		/// <summary>
		/// The current directory being managed.
		/// </summary>
		public string Directory { get; private set; }
		
		/// <summary>
		/// The current file name being managed.
		/// </summary>
		public string FileName { get; private set; }
		
		/// <summary>
		/// The full file path composed of the current directory and file name.
		/// </summary>
		public string FilePath { get; private set; } = string.Empty;


		/// <summary>
		/// Initializes a new instance of the <see cref="FilePathHandler"/> class
		/// with the specified default directory and file name.
		/// </summary>
		/// <param name="defaultDirectory">The default directory path.</param>
		/// <param name="defaultFileName">The default file name.</param>
		public FilePathHandler( string defaultDirectory, string defaultFileName )
		{
			DefaultDirectory = defaultDirectory;
			DefaultFileName = defaultFileName;
			Directory = defaultDirectory;
			FileName = defaultFileName;
		}


		/// <summary>
		/// Takes a partial or complete file path, converts it to a full path,
		/// and stores it in <see cref="FilePath"/>.
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
