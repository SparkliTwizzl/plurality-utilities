using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Petrichor.Common.Containers;
using Petrichor.Common.Utilities;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.TestShared.Info;
using Petrichor.TestShared.Utilities;


namespace Petrichor.ShortcutScriptGeneration.Utilities.Tests
{
	[TestClass]
	public class InputFileHandlerTests
	{
		public struct TestData
		{
			public static ScriptEntry[] Entries => new[]
			{
				new ScriptEntry(),
			};
			public static int EntryListRegionLength => 3;
			public static ScriptInput Input => new( ModuleOptions, Entries, Templates, Macros );
			public static string[] Macros => new[]
			{
				"macro",
			};
			public static int MetadataRegionLength => 3;
			public static ScriptModuleOptions ModuleOptions => new( TestAssets.DefaultIconFilePath, TestAssets.SuspendIconFilePath, TestAssets.ReloadShortcut, TestAssets.SuspendShortcut );
			public static int ModuleOptionsRegionLength => 3;
			public static string[] Templates => new[]
			{
				"template",
			};
			public static int TemplateListRegionLength => 3;
		}


		public class EntryListRegionParserStub : IDataRegionParser<List<ScriptEntry>>
		{
			public int LinesParsed { get; private set; } = 0;
			public DataToken RegionToken => Syntax.Tokens.EntryList;
			public Dictionary<string, int> TokenInstancesParsed { get; } = new();

			public List<ScriptEntry> Parse( IndexedString[] regionData )
			{
				TokenInstancesParsed.Add( Syntax.Tokens.Entry.Key, 1 );
				LinesParsed = TestData.EntryListRegionLength;
				return TestData.Entries.ToList();
			}

			public void Reset()
			{
				LinesParsed = 0;
				foreach ( var tokenName in TokenInstancesParsed.Keys )
				{
					TokenInstancesParsed[ tokenName ] = 0;
				}
			}
		}

		public class MetadataRegionParserStub : IDataRegionParser<IndexedString>
		{
			public int LinesParsed { get; private set; } = 0;
			public DataToken RegionToken => Common.Syntax.Tokens.Metadata;
			public Dictionary<string, int> TokenInstancesParsed { get; } = new();

			public IndexedString Parse( IndexedString[] regionData )
			{
				TokenInstancesParsed.Add( Common.Syntax.Tokens.MinimumVersion.Key, 1 );
				LinesParsed = TestData.MetadataRegionLength;
				return new IndexedString();
			}

			public void Reset()
			{
				LinesParsed = 0;
				foreach ( var tokenName in TokenInstancesParsed.Keys )
				{
					TokenInstancesParsed[ tokenName ] = 0;
				}
			}
		}

		public class ModuleOptionsRegionParserStub : IDataRegionParser<ScriptModuleOptions>
		{
			public int LinesParsed { get; private set; } = 0;
			public Dictionary<string, int> MaxAllowedTokenInstances { get; } = new();
			public Dictionary<string, int> MinRequiredTokenInstances { get; } = new();
			public DataToken RegionToken => Syntax.Tokens.ModuleOptions;
			public Dictionary<string, int> TokenInstancesParsed { get; } = new();

			public ScriptModuleOptions Parse( IndexedString[] regionData )
			{
				TokenInstancesParsed.Add( Syntax.Tokens.DefaultIcon.Key, 1 );
				TokenInstancesParsed.Add( Syntax.Tokens.ReloadShortcut.Key, 1 );
				TokenInstancesParsed.Add( Syntax.Tokens.SuspendIcon.Key, 1 );
				TokenInstancesParsed.Add( Syntax.Tokens.SuspendShortcut.Key, 1 );
				LinesParsed = TestData.ModuleOptionsRegionLength;
				return TestData.ModuleOptions;
			}

			public void Reset()
			{
				LinesParsed = 0;
				foreach ( var tokenName in TokenInstancesParsed.Keys )
				{
					TokenInstancesParsed[ tokenName ] = 0;
				}
			}
		}

		public class TemplateListRegionParserStub : IDataRegionParser<List<string>>
		{
			public int LinesParsed { get; private set; } = 0;
			public Dictionary<string, int> MaxAllowedTokenInstances { get; } = new();
			public Dictionary<string, int> MinRequiredTokenInstances { get; } = new();
			public DataToken RegionToken => Syntax.Tokens.TemplateList;
			public Dictionary<string, int> TokenInstancesParsed { get; } = new();

			public List<string> Parse( IndexedString[] regionData )
			{
				LinesParsed = TestData.TemplateListRegionLength;
				TokenInstancesParsed[ Syntax.Tokens.Template.Key ] = TestData.Templates.Length;
				return TestData.Templates.ToList();
			}

			public void Reset()
			{
				LinesParsed = 0;
				foreach ( var tokenName in TokenInstancesParsed.Keys )
				{
					TokenInstancesParsed[ tokenName ] = 0;
				}
			}
		}


		public EntryListRegionParserStub? entriesRegionParserStub;
		public InputHandler? handler;
		public Mock<IMacroGenerator>? macroGeneratorMock;
		public MetadataRegionParserStub? metadataRegionParserStub;
		public ModuleOptionsRegionParserStub? moduleOptionsRegionParserStub;
		public TemplateListRegionParserStub? templatesRegionParserStub;


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();

			entriesRegionParserStub = new();
			macroGeneratorMock = new();
			_ = macroGeneratorMock
				.Setup( x => x.Generate( It.IsAny<ScriptInput>() ) )
				.Returns( TestData.Macros );
			metadataRegionParserStub = new();
			moduleOptionsRegionParserStub = new();
			templatesRegionParserStub = new();

			handler = new InputHandler( metadataRegionParserStub, moduleOptionsRegionParserStub, entriesRegionParserStub, templatesRegionParserStub, macroGeneratorMock.Object );
		}


		[TestMethod]
		[DynamicData( nameof( ProcessFile_Test_Success_Data ), DynamicDataSourceType.Property )]
		public void ProcessFile_Test_Success( string fileName )
		{
			var expected = TestData.Input;
			var filePath = TestUtilities.LocateInputFile( fileName );
			var actual = handler!.ProcessFile( filePath );
			Assert.AreEqual( expected, actual );
		}

		public static IEnumerable<object[]> ProcessFile_Test_Success_Data
		{
			get
			{
				yield return new object[] { $"{nameof( InputHandler )}_{nameof( ProcessFile_Test_Success )}.petrichor" };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( FileNotFoundException ) )]
		[DynamicData( nameof( ProcessFile_Test_Throws_FileNotFoundException_Data ), DynamicDataSourceType.Property )]
		public void ProcessFile_Test_Throws_FileNotFoundException( string fileName ) => _ = handler!.ProcessFile( TestUtilities.LocateInputFile( fileName ) );

		public static IEnumerable<object[]> ProcessFile_Test_Throws_FileNotFoundException_Data
		{
			get
			{
				yield return new object[] { "nonexistent.txt" };
			}
		}
	}
}
