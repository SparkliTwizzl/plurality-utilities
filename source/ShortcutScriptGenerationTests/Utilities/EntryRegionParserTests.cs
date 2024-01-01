using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Common.Info;
using Petrichor.ShortcutScriptGeneration.Containers;


namespace Petrichor.ShortcutScriptGeneration.Utilities.Tests
{
	[TestClass]
	public class EntryRegionParserTests
	{
		public struct TestData
		{
			public static ScriptEntry Entry => new(
					new List<ShortcutScriptIdentity>
					{
						new("name1", "tag1"),
						new("name2", "tag2"),
					},
					"pronoun",
					"decoration"
				);
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
		}


		[TestMethod]
		public void Parse_Test_Success()
		{
			Assert.Fail();
		}
	}
}
