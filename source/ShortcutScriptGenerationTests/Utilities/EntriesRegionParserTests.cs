using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Info;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.TestShared.Utilities;


namespace Petrichor.ShortcutScriptGeneration.Utilities.Tests
{
	[TestClass]
	public class EntriesRegionParserTests
	{
		public struct TestData
		{
			public static ScriptEntry[] Entries => new[]
			{
				new ScriptEntry(
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
			public static string[] RegionData_BlankDecorationField => new[]
			{
				CommonSyntax.OpenBracketToken,
				"\t{",
				"\t\t#name @tag",
				"\t\t&",
				"\t}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_BlankNameField => new[]
			{
				CommonSyntax.OpenBracketToken,
				"\t{",
				"\t\t# @tag",
				"\t}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_BlankPronounField => new[]
			{
				CommonSyntax.OpenBracketToken,
				"\t{",
				"\t\t#name @tag",
				"\t\t$",
				"\t}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_BlankTagField => new[]
			{
				"{",
				"\t{",
				"\t\t#name @",
				"\t}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_EntryNotClosed => new[]
			{
				CommonSyntax.OpenBracketToken,
				"\t{",
				"\t\t#name @tag",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_NoIdentityField => new[]
			{
				CommonSyntax.OpenBracketToken,
				"\t{",
				"\t}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_NoNameField => new[]
			{
				CommonSyntax.OpenBracketToken,
				"\t{",
				"\t\t@tag",
				"\t}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_NoTagField => new[]
			{
				CommonSyntax.OpenBracketToken,
				"\t{",
				"\t\t#name",
				"\t}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_TooManyDecorationFields => new[]
			{
				CommonSyntax.OpenBracketToken,
				"\t{",
				"\t\t#name @tag",
				"\t\t$pronoun",
				"\t\t&decoration",
				"\t\t&decoration",
				"\t}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_TooManyPronounFields => new[]
			{
				CommonSyntax.OpenBracketToken,
				"\t{",
				"\t\t#name @tag",
				"\t\t$pronoun",
				"\t\t$pronoun",
				"\t}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_UnexpectedCharBetweenEntries => new[]
			{
				CommonSyntax.OpenBracketToken,
				"\ta",
				"\t{",
				"\t\t#name @tag",
				"\t\t$pronoun",
				"\t}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_UnexpectedCharInEntry => new[]
			{
				CommonSyntax.OpenBracketToken,
				"\t{",
				"\t\tx",
				"\t\t#name @tag",
				"\t\t$pronoun",
				"\t}",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_Valid => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{ CommonSyntax.LineCommentToken } line comment",
				"\t{",
				$"\t\t#name1 @tag1 { CommonSyntax.LineCommentToken } inline comment",
				"\t\t#name2 @tag2",
				"\t\t$pronoun1",
				"\t\t&decoration1",
				"\t}",
				"\t",
				$"\t{ CommonSyntax.LineCommentToken } line comment",
				"\t{",
				"\t\t#name3 @tag3",
				"\t\t#name4 @tag4",
				"\t\t$pronoun3",
				"\t\t&decoration3",
				"\t}",
				CommonSyntax.CloseBracketToken,
			};
		}


		public EntriesRegionParser? entriesRegionParser;


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
			entriesRegionParser = new EntriesRegionParser();
		}


		[TestMethod]
		[DynamicData( nameof( Parse_Test_Success_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Success( string[] regionData )
		{
			var expected = TestData.Entries;
			var actual = entriesRegionParser!.Parse( regionData );
			CollectionAssert.AreEqual( expected, actual );
		}

		public static IEnumerable<object[]> Parse_Test_Success_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_Valid };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( BlankInputFieldException ) )]
		[DynamicData( nameof( Parse_Test_ThrowsBlankInputFieldException_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_ThrowsBlankInputFieldException( string[] regionData )
			=> _ = entriesRegionParser!.Parse( regionData );

		public static IEnumerable<object[]> Parse_Test_ThrowsBlankInputFieldException_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_BlankDecorationField };
				yield return new object[] { TestData.RegionData_BlankPronounField };
				yield return new object[] { TestData.RegionData_BlankTagField };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( DuplicateInputFieldException ) )]
		[DynamicData( nameof( Parse_Test_ThrowsDuplicateInputFieldException_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_ThrowsDuplicateInputFieldException( string[] regionData )
			=> _ = entriesRegionParser!.Parse( regionData );

		public static IEnumerable<object[]> Parse_Test_ThrowsDuplicateInputFieldException_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_TooManyDecorationFields };
				yield return new object[] { TestData.RegionData_TooManyPronounFields };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( InputEntryNotClosedException ) )]
		[DynamicData( nameof( Parse_Test_ThrowsInputEntryNotClosedException_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_ThrowsInputEntryNotClosedException( string[] regionData )
			=> _ = entriesRegionParser!.Parse( regionData );

		public static IEnumerable<object[]> Parse_Test_ThrowsInputEntryNotClosedException_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_EntryNotClosed };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( MissingInputFieldException ) )]
		[DynamicData( nameof( Parse_Test_ThrowsMissingInputFieldException_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_ThrowsMissingInputFieldException( string[] regionData )
			=> _ = entriesRegionParser!.Parse( regionData );

		public static IEnumerable<object[]> Parse_Test_ThrowsMissingInputFieldException_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_NoIdentityField };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( InvalidInputFieldException ) )]
		[DynamicData( nameof( Parse_Test_ThrowsInvalidInputFieldException_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_ThrowsInvalidInputFieldException( string[] regionData )
			=> _ = entriesRegionParser!.Parse( regionData );

		public static IEnumerable<object[]> Parse_Test_ThrowsInvalidInputFieldException_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_NoTagField };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( UnexpectedCharacterException ) )]
		[DynamicData( nameof( Parse_Test_ThrowsUnexpectedCharacterException_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_ThrowsUnexpectedCharacterException( string[] regionData )
			=> _ = entriesRegionParser!.Parse( regionData );

		public static IEnumerable<object[]> Parse_Test_ThrowsUnexpectedCharacterException_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_NoNameField };
				yield return new object[] { TestData.RegionData_UnexpectedCharBetweenEntries };
				yield return new object[] { TestData.RegionData_UnexpectedCharInEntry };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( FileRegionException ) )]
		public void Parse_Test_ThrowsFileRegionException()
		{
			_ = entriesRegionParser!.Parse( TestData.RegionData_Valid );
			_ = entriesRegionParser!.Parse( TestData.RegionData_Valid );
		}
	}
}
