using Microsoft.VisualStudio.TestTools.UnitTesting;
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
				"{|" +
				"	{|" +
				"		% #name# @tag|" +
				"		&|" +
				"	}|" +
				"}|";
			public const string InputData_BlankNameField =
				"{|" +
				"	{|" +
				"		% ## @tag|" +
				"	}|" +
				"}|";
			public const string InputData_BlankPronounField =
				"{|" +
				"	{|" +
				"		% #name# @tag|" +
				"		$|" +
				"	}|" +
				"}|";
			public const string InputData_BlankTagField =
				"{|" +
				"	{|" +
				"		% #name# @|" +
				"	}|" +
				"}|";
			public const string InputData_EntryNotClosed =
				"{|" +
				"	{|" +
				"		% #name# @tag|" +
				"}|";
			public const string InputData_MissingIdentityField =
				"{|" +
				"	{|" +
				"	}|" +
				"}|";
			public const string InputData_MissingNameField =
				"{|" +
				"	{|" +
				"		% @tag|" +
				"	}|" +
				"}|";
			public const string InputData_MissingTagField =
				"{|" +
				"	{|" +
				"		% #name#|" +
				"	}|" +
				"}|";
			public const string InputData_TagFieldContainsSpaces =
				"{|" +
				"	{|" +
				"		% #name# @t ag|" +
				"	}|" +
				"}|";
			public const string InputData_TooManyDecorationFields =
				"{|" +
				"	{|" +
				"		% #name# @tag|" +
				"		$pronoun|" +
				"		&decoration|" +
				"		&decoration|" +
				"	}|" +
				"}|";
			public const string InputData_TooManyPronounFields =
				"{|" +
				"	{|" +
				"		% #name# @tag|" +
				"		$pronoun|" +
				"		$pronoun|" +
				"	}|" +
				"}|";
			public const string InputData_UnexpectedCharBetweenEntries =
				"{|" +
				"	a|" +
				"	{|" +
				"		% #name# @tag|" +
				"		$pronoun|" +
				"	}|" +
				"}|";
			public const string InputData_UnexpectedCharInEntry =
				"{|" +
				"	{|" +
				"		a|" +
				"		% #name# @tag|" +
				"		$pronoun|" +
				"	}|" +
				"}|";
			public const string InputData_Valid =
				"{|" +
				"	{|" +
				"		% #name1# @tag1|" +
				"		% #name2# @tag2|" +
				"		$pronoun1|" +
				"		&decoration1|" +
				"	}|" +
				"	{|" +
				"		% #name3# @tag3|" +
				"		% #name4# @tag4|" +
				"		$pronoun3|" +
				"		&decoration3|" +
				"	}|" +
				"}|";
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
			var actual = entryParser!.ParseEntriesFromData( TestUtilities.SplitRegionDataString( regionDataString ), ref i );
			CollectionAssert.AreEqual( expected, actual );
		}

		[TestMethod]
		[ExpectedException( typeof( BlankInputFieldException ) )]
		[DataRow( TestData.InputData_BlankDecorationField )]
		[DataRow( TestData.InputData_BlankNameField )]
		[DataRow( TestData.InputData_BlankPronounField )]
		[DataRow( TestData.InputData_BlankTagField )]
		public void ParseEntriesFromDataTest_ThrowsBlankInputFieldException( string regionDataString )
			=> _ = entryParser!.ParseEntriesFromData( TestUtilities.SplitRegionDataString( regionDataString ), ref i );

		[TestMethod]
		[ExpectedException( typeof( DuplicateInputFieldException ) )]
		[DataRow( TestData.InputData_TooManyDecorationFields )]
		[DataRow( TestData.InputData_TooManyPronounFields )]
		public void ParseEntriesFromDataTest_ThrowsDuplicateInputFieldException( string regionDataString )
			=> _ = entryParser!.ParseEntriesFromData( TestUtilities.SplitRegionDataString( regionDataString ), ref i );

		[TestMethod]
		[ExpectedException( typeof( InputEntryNotClosedException ) )]
		[DataRow( TestData.InputData_EntryNotClosed )]
		public void ParseEntriesFromDataTest_ThrowsInputEntryNotClosedException( string regionDataString )
			=> _ = entryParser!.ParseEntriesFromData( TestUtilities.SplitRegionDataString( regionDataString ), ref i );

		[TestMethod]
		[ExpectedException( typeof( InvalidInputFieldException ) )]
		[DataRow( TestData.InputData_TagFieldContainsSpaces )]
		public void ParseEntriesFromDataTest_ThrowsInvalidInputFieldException( string regionDataString )
			=> _ = entryParser!.ParseEntriesFromData( TestUtilities.SplitRegionDataString( regionDataString ), ref i );

		[TestMethod]
		[ExpectedException( typeof( MissingInputFieldException ) )]
		[DataRow( TestData.InputData_MissingIdentityField )]
		[DataRow( TestData.InputData_MissingNameField )]
		[DataRow( TestData.InputData_MissingTagField )]
		public void ParseEntriesFromDataTest_ThrowsMissingInputFieldException( string regionDataString )
			=> _ = entryParser!.ParseEntriesFromData( TestUtilities.SplitRegionDataString( regionDataString ), ref i );

		[TestMethod]
		[ExpectedException( typeof( UnexpectedCharacterException ) )]
		[DataRow( TestData.InputData_UnexpectedCharBetweenEntries )]
		[DataRow( TestData.InputData_UnexpectedCharInEntry )]
		public void ParseEntriesFromDataTest_ThrowsUnexpectedCharacterException( string regionDataString )
			=> _ = entryParser!.ParseEntriesFromData( TestUtilities.SplitRegionDataString( regionDataString ), ref i );
	}
}
