using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Syntax;
using Petrichor.TestShared.Utilities;


namespace Petrichor.Common.Utilities.Tests
{
	[TestClass]
	public class DataRegionParserTests
	{
		public readonly struct TestData
		{
			public static DataRegionParserDescriptor<IndexedString> ParserDescriptor => new()
			{
				RegionToken = RegionToken,
				TokenHandlers = new()
				{
					{ TestToken, IDataRegionParser<IndexedString>.InertHandler },
				},
			};
			public static IndexedString[] RegionData_MismatchedRegionClose => IndexedString.IndexStringArray( new[]
			{
				RegionToken.Qualify(),
				$"\t{ TestToken.Qualify() } { TestTokenValue }",
				Tokens.RegionClose.Key,
			} );
			public static IndexedString[] RegionData_MismatchedRegionOpen => IndexedString.IndexStringArray( new[]
			{
				RegionToken.Qualify(),
				Tokens.RegionOpen.Key,
				$"\t{ TestToken.Qualify() } { TestTokenValue }",
			} );
			public static IndexedString[] RegionData_TokenWithNoValue => IndexedString.IndexStringArray( new[]
			{
				RegionToken.Qualify(),
				Tokens.RegionOpen.Key,
				$"\t{ TestToken.Qualify() }",
				Tokens.RegionClose.Key,
			} );
			public static IndexedString[] RegionData_TooFewTokenInstances => IndexedString.IndexStringArray( new[]
			{
				RegionToken.Qualify(),
				Tokens.RegionOpen.Key,
				Tokens.RegionClose.Key,
			} );
			public static IndexedString[] RegionData_TooManyTokenInstances => IndexedString.IndexStringArray( new[]
			{
				RegionToken.Qualify(),
				Tokens.RegionOpen.Key,
				$"\t{ TestToken.Qualify() } { TestTokenValue }",
				$"\t{ TestToken.Qualify() } { TestTokenValue }",
				Tokens.RegionClose.Key,
			} );
			public static IndexedString[] RegionData_UnrecognizedToken => IndexedString.IndexStringArray( new[]
			{
				RegionToken.Qualify(),
				Tokens.RegionOpen.Key,
				$"\tunknown-token{ ControlSequences.TokenValueDivider } value",
				Tokens.RegionClose.Key,
			} );
			public static IndexedString[] RegionData_Valid => IndexedString.IndexStringArray( new[]
			{
				$"{RegionToken.Qualify()} region token value",
				$"{ Tokens.RegionOpen.Key } { Tokens.LineComment.Key } inline comment",
				$"\t{ TestToken.Qualify() } { TestTokenValue }",
				Tokens.RegionClose.Key,
			} );
			public static DataToken RegionToken => new()
			{
				Key = nameof( DataRegionParserTests ),
			};
			public static DataToken TestToken => new()
			{
				Key = "test-token",
				MaxAllowed = 1,
				MinRequired = 1,
			};
			public static string TestTokenValue => "Value";
		}


		public DataRegionParser<IndexedString>? parser;


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
			parser = new( TestData.ParserDescriptor );
		}


		[TestMethod]
		[DynamicData( nameof( Parse_Test_Success_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Success( IndexedString[] regionData )
		{
			var expectedResult = new IndexedString();
			var actualResult = parser!.Parse( regionData );
			Assert.AreEqual( expectedResult, actualResult );

			var expectedLinesParsed = regionData.Length;
			var actualLinesParsed = parser!.LinesParsed;
			Assert.AreEqual( expectedLinesParsed, actualLinesParsed );
		}

		public static IEnumerable<object[]> Parse_Test_Success_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_Valid };
				yield return new object[] { TestData.RegionData_TokenWithNoValue };
			}
		}


		[TestMethod]
		[ExpectedException( typeof( BracketException ) )]
		[DynamicData( nameof( Parse_Test_Throws_BracketException_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Throws_BracketException( IndexedString[] regionData ) => _ = parser!.Parse( regionData );

		public static IEnumerable<object[]> Parse_Test_Throws_BracketException_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_MismatchedRegionClose };
				yield return new object[] { TestData.RegionData_MismatchedRegionOpen };
			}
		}


		[TestMethod]
		[ExpectedException( typeof( TokenCountException ) )]
		[DynamicData( nameof( Parse_Test_Throws_TokenCountException_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Throws_TokenCountException( IndexedString[] regionData ) => _ = parser!.Parse( regionData );

		public static IEnumerable<object[]> Parse_Test_Throws_TokenCountException_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_TooFewTokenInstances };
				yield return new object[] { TestData.RegionData_TooManyTokenInstances };
			}
		}


		[TestMethod]
		[ExpectedException( typeof( TokenNameException ) )]
		[DynamicData( nameof( Parse_Test_Throws_TokenNameException_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Throws_TokenNameException( IndexedString[] regionData ) => _ = parser!.Parse( regionData );

		public static IEnumerable<object[]> Parse_Test_Throws_TokenNameException_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_UnrecognizedToken };
			}
		}
	}
}
