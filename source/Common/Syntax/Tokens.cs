using Petrichor.Common.Containers;


namespace Petrichor.Common.Syntax
{
	public readonly struct Tokens
	{
		public static DataToken BlankLine => new()
		{
			Key = string.Empty,
		};
		public static DataToken Command => new()
		{
			Key = "command",
			MaxAllowed = 1,
		};
		public static DataToken InputFile => new()
		{
			Key = "input-file",
			MaxAllowed = 1,
		};
		public static DataToken LineComment => new()
		{
			Key = "//",
		};
		public static DataToken LogFile => new()
		{
			Key = "log-file",
			MaxAllowed = 1,
		};
		public static DataToken LogMode => new()
		{
			Key = "log-mode",
			MaxAllowed = 1,
		};
		public static DataToken Metadata => new()
		{
			Key = "metadata",
			MaxAllowed = 1,
			MinRequired = 1,
		};
		public static DataToken MinimumVersion => new()
		{
			Key = "minimum-version",
			MaxAllowed = 1,
			MinRequired = 1,
		};
		public static DataToken OutputFile => new()
		{
			Key = "output-file",
			MaxAllowed = 1,
		};
		public static DataToken RegionClose => new()
		{
			Key = ControlSequences.RegionClose.ToString(),
		};
		public static DataToken RegionOpen => new()
		{
			Key = ControlSequences.RegionOpen.ToString(),
		};


		public static Dictionary<string, DataToken> LookUpTable => new()
		{
			{ BlankLine.Key, BlankLine },
			{ Command.Key, Command },
			{ LineComment.Key, LineComment },
			{ Metadata.Key, Metadata },
			{ MinimumVersion.Key, MinimumVersion },
			{ RegionClose.Key, RegionClose },
			{ RegionOpen.Key, RegionOpen },
		};
	}
}
