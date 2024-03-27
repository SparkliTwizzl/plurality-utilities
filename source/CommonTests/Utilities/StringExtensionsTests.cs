using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Petrichor.Common.Utilities.Tests
{
	[TestClass]
	public class StringExtensionsTests
	{
		public readonly struct TestData
		{
			public const string WrapInQuotes_Expected = "\"text\"";
			public const string WrapInQuotes_LeadingAndTrailngQuotes = "\"text\"";
			public const string WrapInQuotes_LeadingQuote = "\"text";
			public const string WrapInQuotes_NoQuotes = "text";
			public const string WrapInQuotes_TrailingQuote = "text\"";
		}


		[TestMethod]
		[DataRow( TestData.WrapInQuotes_NoQuotes )]
		[DataRow( TestData.WrapInQuotes_LeadingQuote )]
		[DataRow( TestData.WrapInQuotes_TrailingQuote )]
		[DataRow( TestData.WrapInQuotes_LeadingAndTrailngQuotes )]
		public void WrapInQuotes_Test_Success( string input )
		{
			var expected = TestData.WrapInQuotes_Expected;
			var actual = input.WrapInQuotes();
			Assert.AreEqual( expected, actual );
		}
	}
}
