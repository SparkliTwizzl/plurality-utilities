namespace Petrichor.Logging.Styling
{
	public static class StandardMessageFormats
	{
		public static MessageFormat Debug => new()
		{
			ForegroundColor = "#40ffff",
			BackgroundColor = "#004040",
			Label = nameof( Debug ).ToUpper(),
		};
		public static MessageFormat Error => new()
		{
			ForegroundColor = "#ffffff",
			BackgroundColor = "#c00000",
			Label = nameof( Error ).ToUpper(),
		};
		public static MessageFormat Finish => new()
		{
			ForegroundColor = "#00c080",
			BackgroundColor = "#202020",
			Label = nameof( Finish ).ToUpper(),
		};
		public static MessageFormat Important => new()
		{
			ForegroundColor = "#ffffff",
			BackgroundColor = "#007070",
			Label = nameof( Important ).ToUpper(),
		};
		public static MessageFormat Info => new()
		{
			ForegroundColor = "#ffffff",
			Label = nameof( Info ).ToUpper(),
		};
		public static MessageFormat Start => new()
		{
			ForegroundColor = "#d09000",
			BackgroundColor = "#202020",
			Label = nameof( Start ).ToUpper(),
		};
		public static MessageFormat Warning => new()
		{
			ForegroundColor = "#ffffff",
			BackgroundColor = "#707000",
			Label = nameof( Warning ).ToUpper(),
		};
	}
}
