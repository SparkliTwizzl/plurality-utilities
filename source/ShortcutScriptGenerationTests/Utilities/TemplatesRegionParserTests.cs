﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Common.Exceptions;
using Petrichor.Common.Info;
using Petrichor.ShortcutScriptGeneration.Exceptions;
using Petrichor.TestShared.Utilities;


namespace Petrichor.ShortcutScriptGeneration.Utilities.Tests
{
	[TestClass]
	public class TemplatesRegionParserTests
	{
		public struct TestData
		{
			public static string[] RegionData_TrailingExcapeCharacter => new[]
			{
				CommonSyntax.OpenBracketToken,
				"\t::\\@@:: #\\",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] RegionData_Valid => new[]
			{
				CommonSyntax.OpenBracketToken,
				$"\t{ CommonSyntax.LineCommentToken } line comment",
				string.Empty,
				"\t::\\@@:: #",
				$"\t::\\@\\$\\&@:: # $ & { CommonSyntax.LineCommentToken } inline comment",
				CommonSyntax.CloseBracketToken,
			};
			public static string[] Templates => new[]
			{
				"::@`tag`:: `name`",
				"::@$&`tag`:: `name` `pronoun` `decoration`",
			};
		}


		public TemplatesRegionParser Parser { get; set; } = new();


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
			Parser = new();
		}


		[TestMethod]
		public void Parse_Test_Success()
		{
			var expected = TestData.Templates;
			var actual = Parser.Parse( TestData.RegionData_Valid );
			CollectionAssert.AreEqual( expected, actual );
		}

		[TestMethod]
		[ExpectedException( typeof( EscapeCharacterMismatchException ) )]
		public void Parse_Test_Throws_EscapeCharacterMismatchException()
			=> _ = Parser.Parse( TestData.RegionData_TrailingExcapeCharacter );

		[TestMethod]
		[ExpectedException( typeof( FileRegionException ) )]
		public void Parse_Test_Throws_FileRegionException()
		{
			_ = Parser.Parse( TestData.RegionData_Valid );
			_ = Parser.Parse( TestData.RegionData_Valid );
		}
	}
}
