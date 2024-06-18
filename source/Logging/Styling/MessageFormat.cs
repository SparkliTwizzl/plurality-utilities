namespace Petrichor.Logging.Styling
{
	public struct MessageFormat
	{
		public string Background { get; set; } = "#000000";
		public string Foreground { get; set; } = "#aaaaaa";
		public string Label { get; set; } = string.Empty;
		public int? LineNumber { get; set; } = null;


		public MessageFormat() { }
	}
}
