using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Common.Info;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.TestShared.Utilities;


namespace Petrichor.ShortcutScriptGeneration.Utilities.Tests
{
	[TestClass]
	public class ShortcutScriptEntryParserTests
	{
		public struct TestData
		{
			public static ShortcutScriptEntry[] Entries => new[]
			{
				new ShortcutScriptEntry(
					new List<ShortcutScriptIdentity>
					{
						new("name1", "tag1"),
						new("name2", "tag2"),
					},
					"pronoun1",
					"decoration1"
				),
				new(
					new List<ShortcutScriptIdentity>
					{
						new("name3", "tag3"),
						new("name4", "tag4"),
					},
					"pronoun3",
					"decoration3"
				),
			};
			public const string InputData_BlankDecorationField =
				CommonSyntax.OpenBracketToken + "|" +
				"\t{|" +
				"\t\t% #name# @tag|" +
				"\t\t&|" +
				"\t}|" +
				CommonSyntax.CloseBracketToken;
			public const string InputData_BlankNameField =
				CommonSyntax.OpenBracketToken + "|" +
				"\t{|" +
				"\t\t% ## @tag|" +
				"\t}|" +
				CommonSyntax.CloseBracketToken;
			public const string InputData_BlankPronounField =
				CommonSyntax.OpenBracketToken + "|" +
				"\t{|" +
				"\t\t% #name# @tag|" +
				"\t\t$|" +
				"\t}|" +
				CommonSyntax.CloseBracketToken;
			public const string InputData_BlankTagField =
				"{|" +
				"\t{|" +
				"\t\t% #name# @|" +
				"\t}|" +
				CommonSyntax.CloseBracketToken;
			public const string InputData_EntryNotClosed =
				CommonSyntax.OpenBracketToken + "|" +
				"\t{|" +
				"\t\t% #name# @tag|" +
				CommonSyntax.CloseBracketToken;
			public const string InputData_MissingIdentityField =
				CommonSyntax.OpenBracketToken + "|" +
				"\t{|" +
				"\t}|" +
				CommonSyntax.CloseBracketToken;
			public const string InputData_MissingNameField =
				CommonSyntax.OpenBracketToken + "|" +
				"\t{|" +
				"\t\t% @tag|" +
				"\t}|" +
				CommonSyntax.CloseBracketToken;
			public const string InputData_MissingTagField =
				CommonSyntax.OpenBracketToken + "|" +
				"\t{|" +
				"\t\t% #name#|" +
				"\t}|" +
				CommonSyntax.CloseBracketToken;
			public const string InputData_TagFieldContainsSpaces =
				CommonSyntax.OpenBracketToken + "|" +
				"\t{|" +
				"\t\t% #name# @t ag|" +
				"\t}|" +
				CommonSyntax.CloseBracketToken;
			public const string InputData_TooManyDecorationFields =
				CommonSyntax.OpenBracketToken + "|" +
				"\t{|" +
				"\t\t% #name# @tag|" +
				"\t\t$pronoun|" +
				"\t\t&decoration|" +
				"\t\t&decoration|" +
				"\t}|" +
				CommonSyntax.CloseBracketToken;
			public const string InputData_TooManyPronounFields =
				CommonSyntax.OpenBracketToken + "|" +
				"\t{|" +
				"\t\t% #name# @tag|" +
				"\t\t$pronoun|" +
				"\t\t$pronoun|" +
				"\t}|" +
				CommonSyntax.CloseBracketToken;
			public const string InputData_UnexpectedCharBetweenEntries =
				CommonSyntax.OpenBracketToken + "|" +
				"\ta|" +
				"\t{|" +
				"\t\t% #name# @tag|" +
				"\t\t$pronoun|" +
				"\t}|" +
				CommonSyntax.CloseBracketToken;
			public const string InputData_UnexpectedCharInEntry =
				CommonSyntax.OpenBracketToken + "|" +
				"\t{|" +
				"\t\ta|" +
				"\t\t% #name# @tag|" +
				"\t\t$pronoun|" +
				"\t}|" +
				CommonSyntax.CloseBracketToken;
			public const string InputData_Valid =
				CommonSyntax.OpenBracketToken + "|" +
				"\t{|" +
				"\t\t% #name1# @tag1|" +
				"\t\t% #name2# @tag2|" +
				"\t\t$pronoun1|" +
				"\t\t&decoration1|" +
				"\t}|" +
				"\t|" +
				"\t" + CommonSyntax.LineCommentToken + ": comment|" +
				"\t{|" +
				"\t\t% #name3# @tag3|" +
				"\t\t% #name4# @tag4|" +
				"\t\t$pronoun3|" +
				"\t\t&decoration3|" +
				"\t}|" +
				CommonSyntax.CloseBracketToken;
		}


		public int i;
		public ShortcutScriptEntryParser? entryParser;


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
			i = 0;
			entryParser = new ShortcutScriptEntryParser();
		}


		[TestMethod]
		[DataRow( TestData.InputData_Valid )]
		public void ParseEntriesFromDataTest_Success( string regionDataString )
		{
			var expected = TestData.Entries;
			var input = TestUtilities.SplitRegionDataString( regionDataString );
			var actual = entryParser!.ParseEntriesFromData( input, ref i );
			CollectionAssert.AreEqual( expected, actual );
		}

		[TestMethod]
		[ExpectedException( typeof( BlankInputFieldException ) )]
		[DataRow( TestData.InputData_BlankDecorationField )]
		[DataRow( TestData.InputData_BlankNameField )]
		[DataRow( TestData.InputData_BlankPronounField )]
		[DataRow( TestData.InputData_BlankTagField )]
		public void ParseEntriesFromDataTest_ThrowsBlankInputFieldException( string regionDataString )
		{
			var input = TestUtilities.SplitRegionDataString( regionDataString );
			_ = entryParser!.ParseEntriesFromData( input, ref i );
		}

		[TestMethod]
		[ExpectedException( typeof( DuplicateInputFieldException ) )]
		[DataRow( TestData.InputData_TooManyDecorationFields )]
		[DataRow( TestData.InputData_TooManyPronounFields )]
		public void ParseEntriesFromDataTest_ThrowsDuplicateInputFieldException( string regionDataString )
		{
			var input = TestUtilities.SplitRegionDataString( regionDataString );
			_ = entryParser!.ParseEntriesFromData( input, ref i );
		}

		[TestMethod]
		[ExpectedException( typeof( InputEntryNotClosedException ) )]
		[DataRow( TestData.InputData_EntryNotClosed )]
		public void ParseEntriesFromDataTest_ThrowsInputEntryNotClosedException( string regionDataString )
		{
			var input = TestUtilities.SplitRegionDataString( regionDataString );
			_ = entryParser!.ParseEntriesFromData( input, ref i );
		}

		[TestMethod]
		[ExpectedException( typeof( InvalidInputFieldException ) )]
		[DataRow( TestData.InputData_TagFieldContainsSpaces )]
		public void ParseEntriesFromDataTest_ThrowsInvalidInputFieldException( string regionDataString )
		{
			var input = TestUtilities.SplitRegionDataString( regionDataString );
			_ = entryParser!.ParseEntriesFromData( input, ref i );
		}

		[TestMethod]
		[ExpectedException( typeof( MissingInputFieldException ) )]
		[DataRow( TestData.InputData_MissingIdentityField )]
		[DataRow( TestData.InputData_MissingNameField )]
		[DataRow( TestData.InputData_MissingTagField )]
		public void ParseEntriesFromDataTest_ThrowsMissingInputFieldException( string regionDataString )
		{
			var input = TestUtilities.SplitRegionDataString( regionDataString );
			_ = entryParser!.ParseEntriesFromData( input, ref i );
		}

		[TestMethod]
		[ExpectedException( typeof( UnexpectedCharacterException ) )]
		[DataRow( TestData.InputData_UnexpectedCharBetweenEntries )]
		[DataRow( TestData.InputData_UnexpectedCharInEntry )]
		public void ParseEntriesFromDataTest_ThrowsUnexpectedCharacterException( string regionDataString )
		{
			var input = TestUtilities.SplitRegionDataString( regionDataString );
			_ = entryParser!.ParseEntriesFromData( input, ref i );
		}
	}
}
