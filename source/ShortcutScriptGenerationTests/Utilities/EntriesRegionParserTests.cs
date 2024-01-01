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
				new ScriptEntry(),
				new ScriptEntry(),
			};
			public static int EntryRegionLength => 3;
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
				$"\t{ ShortcutScriptGenerationSyntax.EntryRegionToken } { CommonSyntax.LineCommentToken } inline comment",
				$"\t{ CommonSyntax.OpenBracketToken }",
				$"\t\t{ CommonSyntax.LineCommentToken } entry region body",
				$"\t{ CommonSyntax.CloseBracketToken }",
				"\t",
				$"\t{ CommonSyntax.LineCommentToken } line comment",
				$"\t{ ShortcutScriptGenerationSyntax.EntryRegionToken }",
				$"\t{ CommonSyntax.OpenBracketToken }",
				$"\t\t{ CommonSyntax.LineCommentToken } entry region body",
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
				LinesParsed = TestData.EntryRegionLength;
				return new ScriptEntry();
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
