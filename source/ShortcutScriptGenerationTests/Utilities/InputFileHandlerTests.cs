﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Petrichor.Common.Containers;
using Petrichor.Common.Exceptions;
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
			public static int EntriesRegionLength => 3;
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
			public static int TemplatesRegionLength => 3;
		}


		public class EntriesRegionParserStub : IRegionParser< List< ScriptEntry > >
		{
			public int LinesParsed { get; private set; } = 0;
			public Dictionary< string, int > MaxAllowedTokenInstances { get; } = new();
			public Dictionary< string, int > MinRequiredTokenInstances { get; } = new();
			public string RegionName => Syntax.TokenNames.EntriesRegion;
			public Dictionary< string, int > TokenInstancesParsed { get; } = new();

			public List< ScriptEntry > Parse( string[] regionData )
			{
				TokenInstancesParsed.Add( Syntax.TokenNames.EntryRegion, 1 );
				LinesParsed = TestData.EntriesRegionLength;
				return TestData.Entries.ToList();
			}
		}

		public class MetadataRegionParserStub : IRegionParser< StringWrapper >
		{
			public int LinesParsed { get; private set; } = 0;
			public Dictionary< string, int > MaxAllowedTokenInstances { get; } = new();
			public Dictionary< string, int > MinRequiredTokenInstances { get; } = new();
			public string RegionName => Common.Syntax.TokenNames.MetadataRegion;
			public Dictionary< string, int > TokenInstancesParsed { get; } = new();

			public StringWrapper Parse( string[] regionData )
			{
				TokenInstancesParsed.Add( Common.Syntax.TokenNames.MinimumVersion, 1 );
				LinesParsed = TestData.MetadataRegionLength;
				return new StringWrapper();
			}
		}

		public class ModuleOptionsRegionParserStub : IRegionParser< ScriptModuleOptions >
		{
			public int LinesParsed { get; private set; } = 0;
			public Dictionary< string, int > MaxAllowedTokenInstances { get; } = new();
			public Dictionary< string, int > MinRequiredTokenInstances { get; } = new();
			public string RegionName => Syntax.TokenNames.ModuleOptionsRegion;
			public Dictionary< string, int > TokenInstancesParsed { get; } = new();

			public ScriptModuleOptions Parse( string[] regionData )
			{
				TokenInstancesParsed.Add( Syntax.TokenNames.DefaultIconFilePath, 1 );
				TokenInstancesParsed.Add( Syntax.TokenNames.ReloadShortcut, 1 );
				TokenInstancesParsed.Add( Syntax.TokenNames.SuspendIconFilePath, 1 );
				TokenInstancesParsed.Add( Syntax.TokenNames.SuspendShortcut, 1 );
				LinesParsed = TestData.ModuleOptionsRegionLength;
				return TestData.ModuleOptions;
			}
		}

		public class TemplatesRegionParserStub : IRegionParser< List< string > >
		{
			public int LinesParsed { get; private set; } = 0;
			public Dictionary< string, int > MaxAllowedTokenInstances { get; } = new();
			public Dictionary< string, int > MinRequiredTokenInstances { get; } = new();
			public string RegionName => Syntax.TokenNames.TemplatesRegion;
			public Dictionary< string, int > TokenInstancesParsed { get; } = new();

			public List< string > Parse( string[] regionData )
			{
				LinesParsed = TestData.TemplatesRegionLength;
				TokenInstancesParsed[ Syntax.TokenNames.Template ] = TestData.Templates.Length;
				return TestData.Templates.ToList();
			}
		}


		public EntriesRegionParserStub? entriesRegionParserStub;
		public InputFileHandler? handler;
		public Mock< IMacroGenerator >? macroGeneratorMock;
		public MetadataRegionParserStub? metadataRegionParserStub;
		public ModuleOptionsRegionParserStub? moduleOptionsRegionParserStub;
		public TemplatesRegionParserStub? templatesRegionParserStub;


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();

			entriesRegionParserStub = new();
			macroGeneratorMock = new();
			_ = macroGeneratorMock
				.Setup( x => x.Generate( It.IsAny< ScriptInput >() ) )
				.Returns( TestData.Macros );
			metadataRegionParserStub = new();
			moduleOptionsRegionParserStub = new();
			templatesRegionParserStub = new();

			handler = new InputFileHandler( metadataRegionParserStub, moduleOptionsRegionParserStub, entriesRegionParserStub, templatesRegionParserStub, macroGeneratorMock.Object );
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

		public static IEnumerable< object[] > ProcessFile_Test_Success_Data
		{
			get
			{
				yield return new object[] { $"{ nameof( InputFileHandler ) }_Valid.petrichor" };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( BracketException ) )]
		[DynamicData( nameof( ProcessFile_Test_Throws_BracketException_Data ), DynamicDataSourceType.Property )]
		public void ProcessFile_Test_Throws_BracketException( string fileName ) => _ = handler!.ProcessFile( TestUtilities.LocateInputFile( fileName ) );

		public static IEnumerable< object[] > ProcessFile_Test_Throws_BracketException_Data
		{
			get
			{
				yield return new object[] { $"{ nameof( InputFileHandler ) }_DanglingCloseBracket.petrichor" };
				yield return new object[] { $"{ nameof( InputFileHandler ) }_DanglingOpenBracket.petrichor" };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( FileNotFoundException ) )]
		[DynamicData( nameof( ProcessFile_Test_Throws_FileNotFoundException_Data ), DynamicDataSourceType.Property )]
		public void ProcessFile_Test_Throws_FileNotFoundException( string fileName ) => _ = handler!.ProcessFile( TestUtilities.LocateInputFile( fileName ) );

		public static IEnumerable< object[] > ProcessFile_Test_Throws_FileNotFoundException_Data
		{
			get
			{
				yield return new object[] { "nonexistent.txt" };
			}
		}

		[TestMethod]
		[ExpectedException( typeof( TokenException ) )]
		[DynamicData( nameof( ProcessFile_Test_Throws_TokenException_Data ), DynamicDataSourceType.Property )]
		public void ProcessFile_Test_Throws_TokenException( string fileName ) => _ = handler!.ProcessFile( TestUtilities.LocateInputFile( fileName ) );

		public static IEnumerable< object[] > ProcessFile_Test_Throws_TokenException_Data
		{
			get
			{
				yield return new object[] { $"{ nameof( InputFileHandler ) }_NoMetadataRegion.petrichor" };
				yield return new object[] { $"{ nameof( InputFileHandler ) }_UnknownToken.petrichor" };
			}
		}
	}
}
