using Petrichor.Logging.Containers;


namespace Petrichor.Logging.Styling
{
	public static class StandardMessageFormats
	{
		public static MessageFormat Debug => new()
		{
			Foreground = "#40ffff",
			Background = "#004040",
			Label = nameof( Debug ).ToUpper(),
		};
		public static MessageFormat Error => new()
		{
			Foreground = "#ffffff",
			Background = "#c00000",
			Label = nameof( Error ).ToUpper(),
		};
		public static MessageFormat Finish => new()
		{
			Foreground = "#00c080",
			Background = "#202020",
			Label = nameof( Finish ).ToUpper(),
		};
		public static MessageFormat Important => new()
		{
			Foreground = "#ffffff",
			Background = "#007070",
			Label = nameof( Important ).ToUpper(),
		};
		public static MessageFormat Info => new()
		{
			Foreground = "#909090",
			Label = nameof( Info ).ToUpper(),
		};
		public static MessageFormat Start => new()
		{
			Foreground = "#d09000",
			Background = "#202020",
			Label = nameof( Start ).ToUpper(),
		};
		public static MessageFormat Warning => new()
		{
			Foreground = "#ffffff",
			Background = "#707000",
			Label = nameof( Warning ).ToUpper(),
		};
	}
}
