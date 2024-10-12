using Pastel;
using Petrichor.Logging.Styling;


namespace Petrichor.Logging.Containers
{
	/// <summary>
	/// Contains a string and optional formatting.
	/// Can be printed like a standard string using implicit ToString() method.
	/// </summary>
	public struct Message
	{
		private const int PaddingAmountForFormattedMessage = 10;

		/// <summary>
		/// Gets or sets the format of the message.
		/// </summary>
		public MessageFormat? Format { get; set; } = null;

		/// <summary>
		/// Gets or sets the text of the message.
		/// </summary>
		public string Text { get; set; } = string.Empty;


		/// <summary>
		/// Initializes a new instance of the <see cref="Message"/> struct.
		/// </summary>
		public Message() { }


		/// <summary>
		/// Gets the message text with formatting applied.
		/// Does not modify stored data.
		/// </summary>
		/// <returns>The formatted message text.</returns>
		public string GetFormattedMessage()
		{
			var uncoloredText = ApplyFormattingToString();
			if (Format is null || Logger.IsInTestMode)
			{
				return uncoloredText;
			}
			return uncoloredText.Pastel(Format?.ForegroundColor).PastelBg(Format?.BackgroundColor);
		}

		/// <summary>
		/// Gets the message text with formatting applied, sans color scheme.
		/// Does not modify stored data.
		/// </summary>
		/// <returns>The formatted message text without color.</returns>
		public string GetFormattedMessageWithoutColor() => ApplyFormattingToString();

		/// <summary>
		/// Returns the message text.
		/// </summary>
		/// <returns>The message text.</returns>
		public override string ToString() => Text;


		private string ApplyFormattingToString()
		{
			if (Format is null)
			{
				return Text;
			}

			var labelString = string.Format("{0," + PaddingAmountForFormattedMessage + "}", $"{Format?.Label}");
			var lineNumberString = Format?.LineNumber is not null ? $"<LINE {Format?.LineNumber}> " : string.Empty;
			var uncoloredText = $"{labelString} : {lineNumberString}{Text}";

			return uncoloredText;
		}
	}
}
