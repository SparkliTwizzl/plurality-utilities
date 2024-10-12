namespace Petrichor.Logging.Styling
{
	/// <summary>
	/// Contains styling data to apply to a message.
	/// </summary>
	public struct MessageFormat
	{
		/// <summary>
		/// The background color for a message sent in a terminal.
		/// </summary>
		public string BackgroundColor { get; set; } = "#000000";

		/// <summary>
		/// The foreground color for a message sent in a terminal.
		/// </summary>
		public string ForegroundColor { get; set; } = "#aaaaaa";

		/// <summary>
		/// The label text to prepend to a message.
		/// </summary>
		public string Label { get; set; } = string.Empty;

		/// <summary>
		/// Optional line number to prepend to a message.
		/// </summary>
		public int? LineNumber { get; set; } = null;


		/// <summary>
		/// Initializes a new instance of the <see cref="MessageFormat"/> struct 
		/// </summary> 
		public MessageFormat() { }
	}
}
