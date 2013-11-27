using System;
using System.Collections.Generic;
using System.IO;
using Bricksoft.PowerCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tests
{
	/// <summary>
	/// Summary description for UnitTest3
	/// </summary>
	[TestClass]
	public class ConfigTests
	{
		private static List<string> files = new List<string>();

		public ConfigTests()
		{

		}

		public TestContext TestContext { get { return testContextInstance; } set { testContextInstance = value; } }
		private TestContext testContextInstance;

		// Use ClassInitialize to run code before running the first test in the class
		[ClassInitialize()]
		public static void MyClassInitialize( TestContext testContext )
		{
			// files[0]
			files.Add(Path.GetTempFileName());
			resetFile(0);

			// files[1]
			files.Add(Path.GetTempFileName());
			resetFile(1);
		}

		private static void resetFile( int index )
		{
			string contents;

			if (index < 0 || index > files.Count - 1) {
				throw new ArgumentOutOfRangeException("index");
			}

			if (index == 0) {
				contents = @"
array=[""""]
number=25";
			} else if (index == 1) {
				contents = @"# comments shoudl be rewritten in the correct spot every time..
array=[""""]
; comments can be any line starting with a # or semi-colon
also any line that does not have an equal sign are considered a comment.
number=25";
			} else {
				throw new ArgumentOutOfRangeException("index");
			}

			File.WriteAllText(files[index], contents);
		}

		// Use ClassCleanup to run code after all tests in a class have run
		[ClassCleanup()]
		public static void MyClassCleanup()
		{
			foreach (string f in files) {
				File.SetAttributes(f, FileAttributes.Normal);
				File.Delete(f);
			}
		}

		// Use TestInitialize to run code before running each test 
		[TestInitialize()]
		public void MyTestInitialize()
		{
		}

		// Use TestCleanup to run code after each test has run
		[TestCleanup()]
		public void MyTestCleanup()
		{
		}

		[TestMethod]
		public void TheBasics()
		{
			Config config = new Config();

			config.read(files[0]);

			Assert.AreEqual<int>(1, config.attr<List<string>>("array").Count);
			Assert.AreEqual<int>(1, config.attr<string[]>("array").Length);

			Assert.AreEqual<int>(25, config.attr<int>("number"));

		}

		[TestMethod]
		public void ConstructorVsRead()
		{
			Config config = new Config(files[1]);

			Assert.AreEqual<int>(1, config.attr<List<string>>("array").Count);
			Assert.AreEqual<int>(1, config.attr<string[]>("array").Length);

			Assert.AreEqual<int>(25, config.attr<int>("number"));

			config = new Config();
			config.read(files[1]);

			Assert.AreEqual<int>(1, config.attr<List<string>>("array").Count);
			Assert.AreEqual<int>(1, config.attr<string[]>("array").Length);

			Assert.AreEqual<short>(25, config.attr<short>("number"));
			Assert.AreEqual<int>(25, config.attr<int>("number"));
			Assert.AreEqual<long>(25, config.attr<long>("number"));
			Assert.AreEqual<ulong>(25, config.attr<ulong>("number"));
		}

		[TestMethod]
		public void Comments()
		{
			Config config = new Config(files[1]);

			Assert.AreEqual<int>(1, config.attr<List<string>>("array").Count);
			Assert.AreEqual<int>(1, config.attr<string[]>("array").Length);

			Assert.AreEqual<int>(25, config.attr<int>("number"));

			Assert.AreEqual<string>(@"; comments can be any line starting with a # or semi-colon
also any line that does not have an equal sign are considered a comment.", config.comment("number"));

			Assert.AreEqual<string>("# comments shoudl be rewritten in the correct spot every time..", config.comment("array"));
		}

		[TestMethod]
		public void WriteComments()
		{
			Config config = new Config(files[1]);

			Assert.AreEqual<int>(1, config.attr<List<string>>("array").Count);
			Assert.AreEqual<int>(1, config.attr<string[]>("array").Length);

			Assert.AreEqual<int>(25, config.attr<int>("number"));

			Assert.AreEqual<string>(@"; comments can be any line starting with a # or semi-colon
also any line that does not have an equal sign are considered a comment.", config.comment("number"));

			Assert.AreEqual<string>("# comments shoudl be rewritten in the correct spot every time..", config.comment("array"));

			config.comment("number", null);

			config.write();

			config.read();

			Assert.IsNull(config.comment("number"));


			resetFile(1);
		}
	}
}
