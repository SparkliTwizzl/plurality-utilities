using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Syntax;
using Petrichor.TestShared.Utilities;


namespace Petrichor.Common.Utilities.Tests
{
	[TestClass]
	public class TokenBodyParserTests
	{
		public readonly struct TestData
		{
			public static TokenParseDescriptor<IndexedString> ParserDescriptor => new()
			{
				TokenPrototype = RegionToken,
				SubTokenHandlers = new()
				{
					{ TestToken, ITokenBodyParser<IndexedString>.InertHandler },
				},
			};
			public static IndexedString[] RegionData_MismatchedRegionClose => IndexedString.IndexRawStrings(
			[
				RegionToken.QualifyKey(),
				$"\t{ TestToken.QualifyKey() } { TestTokenValue }",
				TokenPrototypes.TokenBodyClose.Key,
			] );
			public static IndexedString[] RegionData_MismatchedRegionOpen => IndexedString.IndexRawStrings(
			[
				RegionToken.QualifyKey(),
				TokenPrototypes.TokenBodyOpen.Key,
				$"\t{ TestToken.QualifyKey() } { TestTokenValue }",
			] );
			public static IndexedString[] RegionData_TokenWithNoValue => IndexedString.IndexRawStrings(
			[
				RegionToken.QualifyKey(),
				TokenPrototypes.TokenBodyOpen.Key,
				$"\t{ TestToken.QualifyKey() }",
				TokenPrototypes.TokenBodyClose.Key,
			] );
			public static IndexedString[] RegionData_TooFewTokenInstances => IndexedString.IndexRawStrings(
			[
				RegionToken.QualifyKey(),
				TokenPrototypes.TokenBodyOpen.Key,
				TokenPrototypes.TokenBodyClose.Key,
			] );
			public static IndexedString[] RegionData_TooManyTokenInstances => IndexedString.IndexRawStrings(
			[
				RegionToken.QualifyKey(),
				TokenPrototypes.TokenBodyOpen.Key,
				$"\t{ TestToken.QualifyKey() } { TestTokenValue }",
				$"\t{ TestToken.QualifyKey() } { TestTokenValue }",
				TokenPrototypes.TokenBodyClose.Key,
			] );
			public static IndexedString[] RegionData_UnrecognizedToken => IndexedString.IndexRawStrings(
			[
				RegionToken.QualifyKey(),
				TokenPrototypes.TokenBodyOpen.Key,
				$"\tunknown-token{ ControlSequences.TokenKeyDelimiter } value",
				TokenPrototypes.TokenBodyClose.Key,
			] );
			public static IndexedString[] RegionData_Valid => IndexedString.IndexRawStrings(
			[
				$"{RegionToken.QualifyKey()} region token value",
				$"{ TokenPrototypes.TokenBodyOpen.Key } { TokenPrototypes.LineComment.Key } inline comment",
				$"\t{ TestToken.QualifyKey() } { TestTokenValue }",
				TokenPrototypes.TokenBodyClose.Key,
			] );
			public static DataToken RegionToken => new()
			{
				Key = nameof( TokenBodyParserTests ),
			};
			public static DataToken TestToken => new()
			{
				Key = "test-token",
				MaxAllowed = 1,
				MinRequired = 1,
			};
			public static string TestTokenValue => "Value";
		}


		public TokenBodyParser<IndexedString>? parser;


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
			parser = new( TestData.ParserDescriptor );
		}


		[TestMethod]
		[DynamicData( nameof( Parse_Test_Success_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Success( IndexedString[] bodyData )
		{
			var expectedResult = new IndexedString();
			var actualResult = parser!.Parse( bodyData );
			Assert.AreEqual( expectedResult, actualResult );

			var expectedLinesParsed = bodyData.Length;
			var actualLinesParsed = parser!.TotalLinesParsed;
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
		public void Parse_Test_Throws_BracketException( IndexedString[] bodyData ) => _ = parser!.Parse( bodyData );

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
		public void Parse_Test_Throws_TokenCountException( IndexedString[] bodyData ) => _ = parser!.Parse( bodyData );

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
		public void Parse_Test_Throws_TokenNameException( IndexedString[] bodyData ) => _ = parser!.Parse( bodyData );

		public static IEnumerable<object[]> Parse_Test_Throws_TokenNameException_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_UnrecognizedToken };
			}
		}
	}
}
