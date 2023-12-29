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
			public static string[] InputData_BlankDecorationField => new[]
			{
				CommonSyntax.OpenBracketToken,
				"\t{",
				"\t\t% #name# @tag",
				"\t\t&",
				"\t}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] InputData_BlankNameField => new[]
			{
				CommonSyntax.OpenBracketToken,
				"\t{",
				"\t\t% ## @tag",
				"\t}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] InputData_BlankPronounField => new[]
			{
				CommonSyntax.OpenBracketToken,
				"\t{",
				"\t\t% #name# @tag",
				"\t\t$",
				"\t}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] InputData_BlankTagField => new[]
			{
				"{",
				"\t{",
				"\t\t% #name# @",
				"\t}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] InputData_EntryNotClosed => new[]
			{
				CommonSyntax.OpenBracketToken,
				"\t{",
				"\t\t% #name# @tag",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] InputData_MissingIdentityField => new[]
			{
				CommonSyntax.OpenBracketToken,
				"\t{",
				"\t}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] InputData_MissingNameField => new[]
			{
				CommonSyntax.OpenBracketToken,
				"\t{",
				"\t\t% @tag",
				"\t}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] InputData_MissingTagField => new[]
			{
				CommonSyntax.OpenBracketToken,
				"\t{",
				"\t\t% #name#",
				"\t}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] InputData_TagFieldContainsSpaces => new[]
			{
				CommonSyntax.OpenBracketToken,
				"\t{",
				"\t\t% #name# @t ag",
				"\t}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] InputData_TooManyDecorationFields => new[]
			{
				CommonSyntax.OpenBracketToken,
				"\t{",
				"\t\t% #name# @tag",
				"\t\t$pronoun",
				"\t\t&decoration",
				"\t\t&decoration",
				"\t}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] InputData_TooManyPronounFields => new[]
			{
				CommonSyntax.OpenBracketToken,
				"\t{",
				"\t\t% #name# @tag",
				"\t\t$pronoun",
				"\t\t$pronoun",
				"\t}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] InputData_UnexpectedCharBetweenEntries => new[]
			{
				CommonSyntax.OpenBracketToken,
				"\ta",
				"\t{",
				"\t\t% #name# @tag",
				"\t\t$pronoun",
				"\t}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] InputData_UnexpectedCharInEntry => new[]
			{
				CommonSyntax.OpenBracketToken,
				"\t{",
				"\t\ta",
				"\t\t% #name# @tag",
				"\t\t$pronoun",
				"\t}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] InputData_Valid => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{ CommonSyntax.LineCommentToken } line comment",
				"\t{",
				$"\t\t% #name1# @tag1{ CommonSyntax.LineCommentToken } inline comment",
				"\t\t% #name2# @tag2",
				"\t\t$pronoun1",
				"\t\t&decoration1",
				"\t}",
				"\t",
				$"\t{ CommonSyntax.LineCommentToken } line comment",
				"\t{",
				"\t\t% #name3# @tag3",
				"\t\t% #name4# @tag4",
				"\t\t$pronoun3",
				"\t\t&decoration3",
				"\t}",
				CommonSyntax.CloseBracketToken,
			};
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
		[DynamicData( nameof( ParseEntriesFromDataTest_Success_Data ), DynamicDataSourceType.Property )]
		public void ParseEntriesFromDataTest_Success( string[] data )
		{
			var expected = TestData.Entries;
			var actual = entryParser!.ParseEntriesFromData( data, ref i );
			CollectionAssert.AreEqual( expected, actual );
		}

		public static IEnumerable<object[]> ParseEntriesFromDataTest_Success_Data
		{
			get
			{
				yield return new object[] { TestData.InputData_Valid };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( BlankInputFieldException ) )]
		[DynamicData( nameof( ParseEntriesFromDataTest_ThrowsBlankInputFieldException_Data ), DynamicDataSourceType.Property )]
		public void ParseEntriesFromDataTest_ThrowsBlankInputFieldException( string[] data )
			=> _ = entryParser!.ParseEntriesFromData( data, ref i );

		public static IEnumerable<object[]> ParseEntriesFromDataTest_ThrowsBlankInputFieldException_Data
		{
			get
			{
				yield return new object[] { TestData.InputData_BlankDecorationField };
				yield return new object[] { TestData.InputData_BlankNameField };
				yield return new object[] { TestData.InputData_BlankPronounField };
				yield return new object[] { TestData.InputData_BlankTagField };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( DuplicateInputFieldException ) )]
		[DynamicData( nameof( ParseEntriesFromDataTest_ThrowsDuplicateInputFieldException_Data ), DynamicDataSourceType.Property )]
		public void ParseEntriesFromDataTest_ThrowsDuplicateInputFieldException( string[] data )
			=> _ = entryParser!.ParseEntriesFromData( data, ref i );

		public static IEnumerable<object[]> ParseEntriesFromDataTest_ThrowsDuplicateInputFieldException_Data
		{
			get
			{
				yield return new object[] { TestData.InputData_TooManyDecorationFields };
				yield return new object[] { TestData.InputData_TooManyPronounFields };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( InputEntryNotClosedException ) )]
		[DynamicData( nameof( ParseEntriesFromDataTest_ThrowsInputEntryNotClosedException_Data ), DynamicDataSourceType.Property )]
		public void ParseEntriesFromDataTest_ThrowsInputEntryNotClosedException( string[] data )
			=> _ = entryParser!.ParseEntriesFromData( data, ref i );

		public static IEnumerable<object[]> ParseEntriesFromDataTest_ThrowsInputEntryNotClosedException_Data
		{
			get
			{
				yield return new object[] { TestData.InputData_EntryNotClosed };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( InvalidInputFieldException ) )]
		[DynamicData( nameof( ParseEntriesFromDataTest_ThrowsInvalidInputFieldException_Data ), DynamicDataSourceType.Property )]
		public void ParseEntriesFromDataTest_ThrowsInvalidInputFieldException( string[] data )
			=> _ = entryParser!.ParseEntriesFromData( data, ref i );

		public static IEnumerable<object[]> ParseEntriesFromDataTest_ThrowsInvalidInputFieldException_Data
		{
			get
			{
				yield return new object[] { TestData.InputData_TagFieldContainsSpaces };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( MissingInputFieldException ) )]
		[DynamicData( nameof( ParseEntriesFromDataTest_ThrowsMissingInputFieldException_Data ), DynamicDataSourceType.Property )]
		public void ParseEntriesFromDataTest_ThrowsMissingInputFieldException( string[] data )
			=> _ = entryParser!.ParseEntriesFromData( data, ref i );

		public static IEnumerable<object[]> ParseEntriesFromDataTest_ThrowsMissingInputFieldException_Data
		{
			get
			{
				yield return new object[] { TestData.InputData_MissingIdentityField };
				yield return new object[] { TestData.InputData_MissingNameField };
				yield return new object[] { TestData.InputData_MissingTagField };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( UnexpectedCharacterException ) )]
		[DynamicData( nameof( ParseEntriesFromDataTest_ThrowsUnexpectedCharacterException_Data ), DynamicDataSourceType.Property )]
		public void ParseEntriesFromDataTest_ThrowsUnexpectedCharacterException( string[] data )
			=> _ = entryParser!.ParseEntriesFromData( data, ref i );

		public static IEnumerable<object[]> ParseEntriesFromDataTest_ThrowsUnexpectedCharacterException_Data
		{
			get
			{
				yield return new object[] { TestData.InputData_UnexpectedCharBetweenEntries };
				yield return new object[] { TestData.InputData_UnexpectedCharInEntry };
			}
		}
	}
}
