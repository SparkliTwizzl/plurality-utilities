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
		private const int FormattedMessagePaddingAmount = 10;


		public MessageFormat? Format { get; set; } = null;
		public string Text { get; set; } = string.Empty;


		public Message() { }


		/// <summary>
		/// Get message text with formatting applied.
		/// Does not modify stored data.
		/// </summary>
		public string Formatted()
		{
			if ( Format is null )
			{
				return Text;
			}

			var labelString = string.Format( "{0," + FormattedMessagePaddingAmount + "}", $"{Format?.Label}" );
			var lineNumberString = Format?.LineNumber is not null ? $"<LINE {Format?.LineNumber}> " : string.Empty;
			var uncoloredText = $"{labelString} : {lineNumberString}{Text}";

			if ( Log.IsInTestMode )
			{
				return uncoloredText;
			}

			return uncoloredText.Pastel( Format?.Foreground ).PastelBg( Format?.Background );
		}

		public override string ToString() => Text;
	}
}
