using Microsoft.VisualStudio.TestTools.UnitTesting;

using PluralityUtilities.Logging;
using PluralityUtilities.TestCommon;


namespace PluralityUtilities.AutoHotkeyScripts.Utilities.Tests
{
	[TestClass]
	public class AutoHotkeyScriptGeneratorTests
	{
		[TestInitialize]
		public void Setup()
		{
			Log.SetLogFolder(TestDirectories.TestLogDir);
			Log.SetLogFileName(DateTime.Now.ToString("test_yyyy-MM-dd_hh-mm-ss.log"));
			Log.EnableVerbose();
		}


		[TestMethod]
		public void GenerateScriptTest_Success()
		{
			Assert.Fail();
		}
	}
}