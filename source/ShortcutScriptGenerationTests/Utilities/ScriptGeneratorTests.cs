﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petrichor.Common.Info;
using Petrichor.ShortcutScriptGeneration.Containers;
using Petrichor.TestShared.Info;
using Petrichor.TestShared.Utilities;


namespace Petrichor.ShortcutScriptGeneration.Utilities.Tests
{
	[TestClass]
	public class ScriptGeneratorTests
	{
		public readonly struct TestData
		{
			public static ScriptEntry[] EntryList => new[]
			{
				new ScriptEntry(),
			};
			public static ScriptMacroTemplate[] TemplateList => new[]
			{
				new ScriptMacroTemplate()
				{
					TemplateFindString = "template-find",
					TemplateReplaceString = "template-replace",
				}
			};
			public static ScriptInput Input => new( ModuleOptions, EntryList, TemplateList, Macros );
			public static string[] Macros => new[]
			{
				"macro",
			};
			public static ScriptModuleOptions ModuleOptions => new( $"\"{TestAssets.DefaultIconFilePath}\"", $"\"{TestAssets.SuspendIconFilePath}\"", TestAssets.ReloadShortcut, TestAssets.SuspendShortcut );
			public static string[] GeneratedOutputFileContents => new[]
			{
				$"; Generated by { AppInfo.AppNameAndVersion } AutoHotkey shortcut script generator",
				"; https://github.com/SparkliTtwizzl/petrichor",
				"",
				"",
				"#Requires AutoHotkey v2.0",
				"#SingleInstance Force",
				"",
				$"defaultIcon := \"{ TestAssets.DefaultIconFilePath }\"",
				$"suspendIcon := \"{ TestAssets.SuspendIconFilePath }\"",
				"",
				"",
				"; constants used for icon handling",
				"FREEZE_ICON := true",
				"ID_FILE_SUSPEND := 65305",
				"ID_TRAY_SUSPEND := 65404",
				"SUSPEND_OFF := 0",
				"SUSPEND_ON := 1",
				"SUSPEND_TOGGLE := -1",
				"WM_COMMAND := 0x111",
				"",
				"",
				"; icon handling",
				"; based on code by ntepa on autohotkey.com/boards: https://www.autohotkey.com/boards/viewtopic.php?p=497349#p497349",
				"SuspendC := Suspend.GetMethod( \"Call\" )",
				"Suspend.DefineProp( \"Call\",",
				"	{",
				"		Call:( this, mode := SUSPEND_TOGGLE ) => ( SuspendC( this, mode ), OnSuspend( A_IsSuspended ) )",
				"	})",
				"OnMessage( WM_COMMAND, OnSuspendMsg )",
				"OnSuspendMsg( wparam, * )",
				"{",
				"	if ( wparam = ID_FILE_SUSPEND || wparam = ID_TRAY_SUSPEND )",
				"	{",
				"		OnSuspend( !A_IsSuspended )",
				"	}",
				"}",
				"",
				"OnSuspend( mode )",
				"{",
				"	scriptIcon := SelectIcon( mode )",
				"	SetIcon( scriptIcon )",
				"}",
				"",
				"SelectIcon( suspendMode )",
				"{",
				"	if ( suspendMode = SUSPEND_ON )",
				"	{",
				"		return suspendIcon",
				"	}",
				"	else if ( suspendMode = SUSPEND_OFF )",
				"	{",
				"		return defaultIcon",
				"	}",
				"	return \"\"",
				"}",
				"",
				"SetIcon( scriptIcon )",
				"{",
				"	if ( FileExist( scriptIcon ) )",
				"	{",
				"		TraySetIcon( scriptIcon,, FREEZE_ICON )",
				"	}",
				"}",
				"",
				"SetIcon( defaultIcon )",
				"",
				"",
				"; script reload / suspend shortcut(s)",
				"#SuspendExempt true",
				$"{ TestAssets.ReloadShortcut }::Reload()",
				$"{ TestAssets.SuspendShortcut }::Suspend( SUSPEND_TOGGLE )",
				"#SuspendExempt false",
				"",
				"",
				"; macros generated from entries and templates",
				"macro",
			};
		}


		public ScriptGenerator? generator;


		[TestInitialize]
		public void Setup()
		{
			TestUtilities.InitializeLoggingForTests();
			generator = new();
		}


		[TestMethod]
		public void Generate_Test_Success()
		{
			var outputFile = $@"{TestDirectories.TestOutputDirectory}\{nameof( ScriptGenerator )}_{nameof( Generate_Test_Success )}.ahk";
			generator!.Generate( TestData.Input, outputFile );

			var expected = TestData.GeneratedOutputFileContents;
			var actual = File.ReadAllLines( outputFile );
			CollectionAssert.AreEqual( expected, actual );
		}
	}
}
