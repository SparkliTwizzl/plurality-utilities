using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Syntax;
using Petrichor.TestShared.Utilities;


namespace Petrichor.Common.Utilities.Tests
{
	[TestClass]
	public class RegionParserTests
	{
		public struct TestData
		{
			public static RegionParserDescriptor< StringWrapper > ParserDescriptor => new()
			{
				MaxAllowedTokenInstances = new()
				{
					{ TokenName, 1 },
				},
				MinRequiredTokenInstances = new()
				{
					{ TokenName, 1 },
				},
				RegionName = nameof( RegionParserTests ),
				TokenHandlers = new()
				{
					{ TokenName, ( string[] regionData, int tokenStartIndex, StringWrapper result ) => new() },
				},
			};
			public static string[] RegionData_TokenWithNoValue => new[]
			{
				Tokens.RegionOpen,
				$"\t{ Token }",
				Tokens.RegionClose,
			};
			public static string[] RegionData_DanglingRegionClose => new[]
			{
				$"\t{ Token } { TokenValue }",
				Tokens.RegionClose,
			};
			public static string[] RegionData_DanglingRegionOpen => new[]
			{
				Tokens.RegionOpen,
				$"\t{ Token } { TokenValue }",
			};
			public static string[] RegionData_TooFewTokenInstances => new[]
			{
				Tokens.RegionOpen,
				Tokens.RegionClose,
			};
			public static string[] RegionData_TooManyTokenInstances => new[]
			{
				Tokens.RegionOpen,
				$"\t{ Token } { TokenValue }",
				$"\t{ Token } { TokenValue }",
				Tokens.RegionClose,
			};
			public static string[] RegionData_Valid => new[]
			{
				$"{ Tokens.RegionOpen } { Tokens.LineComment } inline comment",
				$"\t{ Token } { TokenValue }",
				Tokens.RegionClose,
			};
			public static string Token => $"{ TokenName }{ OperatorChars.TokenValueDivider }";
			public static string TokenName => "token-name";
			public static string TokenValue => "Value";
		}


		public RegionParser< StringWrapper >? parser;


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();

			parser = new( TestData.ParserDescriptor );
		}


		[TestMethod]
		[DynamicData( nameof( Parse_Test_Success_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Success( string[] regionData )
		{
			var expectedResult = new StringWrapper();
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
		public void Parse_Test_Throws_BracketException( string[] regionData ) => _ = parser!.Parse( regionData );

		public static IEnumerable<object[]> Parse_Test_Throws_BracketException_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_DanglingRegionClose };
				yield return new object[] { TestData.RegionData_DanglingRegionOpen };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( TokenException ) )]
		[DynamicData( nameof( Parse_Test_Throws_TokenException_Data ), DynamicDataSourceType.Property )]
		public void Parse_Test_Throws_TokenException( string[] regionData ) => _ = parser!.Parse( regionData );

		public static IEnumerable<object[]> Parse_Test_Throws_TokenException_Data
		{
			get
			{
				yield return new object[] { TestData.RegionData_TooFewTokenInstances };
				yield return new object[] { TestData.RegionData_TooManyTokenInstances };
			}
		}
	}
}
