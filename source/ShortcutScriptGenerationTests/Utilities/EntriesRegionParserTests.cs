using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Info;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.ShortcutScriptGeneration.Info;
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
				Entry,
				Entry,
			};
			public static ScriptEntry Entry => new(
					new List<ScriptIdentity>
					{
						new(EntryName, EntryTag),
					},
					EntryPronoun,
					EntryDecoration
				);
			public static string EntryDecoration => "decoration";
			public static int EntryLength => 5;
			public static string EntryName => "name";
			public static string EntryNameTokenValue => $"{EntryName} @{EntryTag}";
			public static string EntryPronoun => "pronoun";
			public static string EntryTag => "tag";
			public static string[] RegionData_DanglingCloseBracket => new[]
			{
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_DanglingOpenBracket => new[]
			{
				CommonSyntax.OpenBracketToken,
			};
			public static string[] RegionData_UnknownToken => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\tunknown{ CommonSyntax.TokenValueDivider } token",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_Valid => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{ CommonSyntax.LineCommentToken } line comment",
				$"\t{ ShortcutScriptGenerationSyntax.EntryRegionToken }",
				$"\t{ CommonSyntax.OpenBracketToken }",
				$"\t\t{ ShortcutScriptGenerationSyntax.EntryNameToken } { EntryNameTokenValue } { CommonSyntax.LineCommentToken } inline comment",
				$"\t\t{EntryPronoun}",
				$"\t\t{EntryDecoration}",
				$"\t{ CommonSyntax.CloseBracketToken }",
				"\t",
				$"\t{ CommonSyntax.LineCommentToken } line comment",
				$"\t{ ShortcutScriptGenerationSyntax.EntryRegionToken }",
				$"\t{ CommonSyntax.OpenBracketToken }",
				$"\t\t{ ShortcutScriptGenerationSyntax.EntryNameToken } { EntryNameTokenValue }",
				$"\t\t{EntryPronoun}",
				$"\t\t{EntryDecoration}",
				$"\t{ CommonSyntax.CloseBracketToken }",
				CommonSyntax.CloseBracketToken,
			};
		}



		public class EntryParserStub : IEntryRegionParser
		{
			public bool HasParsedMaxAllowedRegions { get; private set; } = false;
			public int LinesParsed { get; private set; } = 0;
			public int MaxRegionsAllowed { get; private set; } = 1;
			public int RegionsParsed { get; private set; } = 0;

			public ScriptEntry Parse( string[] regionData )
			{
				++RegionsParsed;
				LinesParsed = TestData.EntryLength;
				return TestData.Entry;
			}
		}


		public EntryParserStub? entryRegionParserStub;
		public EntriesRegionParser? parser;


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
			entryRegionParserStub = new();
			parser = new( entryRegionParserStub );
		}


		[TestMethod]
		[DynamicData( nameof( Parse_Test_Success_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Success( string[] regionData )
		{
			var expected = TestData.Entries;
			var actual = parser!.Parse( regionData );
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
		[ExpectedException( typeof( BracketMismatchException ) )]
		[DynamicData( nameof( Parse_Test_Throws_BracketMismatchException_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Throws_BracketMismatchException( string[] regionData ) => _ = parser!.Parse( regionData );

		public static IEnumerable<object[]> Parse_Test_Throws_BracketMismatchException_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_DanglingCloseBracket };
				yield return new object[] { TestData.RegionData_DanglingOpenBracket };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( FileRegionException ) )]
		public void Parse_Test_Throws_FileRegionException()
		{
			_ = parser!.Parse( TestData.RegionData_Valid );
			_ = parser!.Parse( TestData.RegionData_Valid );
		}

		[TestMethod]
		[ExpectedException( typeof( TokenException ) )]
		public void Parse_Test_Throws_TokenException() => _ = parser!.Parse( TestData.RegionData_UnknownToken );
	}
}
