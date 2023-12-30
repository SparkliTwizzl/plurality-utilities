using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Petrichor.Common.Utilities.Tests
{
	[TestClass]
	public class StringExtensionsTests
	{
		public struct TestData
		{
			public const string WrapInQuotes_Expected = "\"text\"";
		}

		[TestMethod]
		[DataRow( "text" )]
		[DataRow( "\"text" )]
		[DataRow( "text\"" )]
		[DataRow( "\"text\"" )]
		public void WrapInQuotes_Test_Success( string input )
		{
			var expected = TestData.WrapInQuotes_Expected;
			var actual = input.WrapInQuotes();
			Assert.AreEqual( expected, actual );
		}
	}
}
