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
			var uncoloredText = ApplyStringFormatting();
			if ( Format is null || Log.IsInTestMode )
			{
				return uncoloredText;
			}
			return uncoloredText.Pastel( Format?.Foreground ).PastelBg( Format?.Background );
		}

		/// <summary>
		/// Get message text with formatting applied, sans color scheme.
		/// Does not modify stored data.
		/// </summary>
		public string FormattedWithoutColor() => ApplyStringFormatting();

		public override string ToString() => Text;


		private string ApplyStringFormatting()
		{
			if ( Format is null )
			{
				return Text;
			}

			var labelString = string.Format( "{0," + FormattedMessagePaddingAmount + "}", $"{Format?.Label}" );
			var lineNumberString = Format?.LineNumber is not null ? $"<LINE {Format?.LineNumber}> " : string.Empty;
			var uncoloredText = $"{labelString} : {lineNumberString}{Text}";

			return uncoloredText;
		}
	}
}
