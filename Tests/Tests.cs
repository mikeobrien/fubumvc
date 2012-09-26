using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;
using FubuCore;
using NUnit.Framework;
using Should;

namespace Tests
{
    [TestFixture]
    public class Tests
    {
        private const string BottleStartupLogPath = @"c:\windows\temp\bottlestartup.log";
        private Website _testWebsite;
        private readonly JavaScriptSerializer _json = new JavaScriptSerializer();

        [TestFixtureSetUp]
        public void Setup()
        {
            if (File.Exists(BottleStartupLogPath)) File.Delete(BottleStartupLogPath);
            _testWebsite = new Website();
            _testWebsite.Create("BottleTests", @"..\..\..\TestHarness\");
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _testWebsite.Remove();
        }


        [Test]
        public void should_have_connectivity_to_the_test_harness_site()
        {
            _testWebsite.DownloadString("test.html").ShouldEqual("oh hai");
        }

        public class MessageModel { public string Message { get; set; } }
        [Test]
        public void should_get_a_response_from_a_fubu_endpoint()
        {
            _json.Deserialize<MessageModel>(_testWebsite.DownloadString()).Message.ShouldEqual("oh hai");
        }

        public class TimeModel { public string Time { get; set; } }
        [Test]
        public void should_get_a_response_from_a_fubu_endpoint_that_takes_in_a_bottle_dependency()
        {
            _json.Deserialize<TimeModel>(_testWebsite.DownloadString("time")).Time.ShouldEqual(DateTime.Now.ToString("MM-dd-yyyy"));
        }

        [Test]
        public void should_get_a_response_from_an_html_reponse_bottle_endpoint()
        {
            _testWebsite.DownloadString("bottletest/time", "text/html").ShouldContain("<h3>{0}</h3>".ToFormat(DateTime.Now.ToString("MM-dd-yyyy")));
        }

        [Test]
        public void should_get_a_response_from_a_json_reponse_bottle_endpoint()
        {
            _json.Deserialize<TimeModel>(_testWebsite.DownloadString("bottletest/time", "application/json")).Time.ShouldEqual(DateTime.Now.ToString("MM-dd-yyyy"));
        }

        [Test]
        public void should_return_bottle_content()
        {
            _testWebsite.DownloadString("_content/test.js").ShouldEqual("{yada}");
        }

        [Test]
        public void should_return_modified_bottle_content()
        {
            // There is a prebuild event in the AssemblyBottle project to mod the file and rebuild the assembly bottle.

            var embeddedFileContents = Assembly.LoadFile(Path.GetFullPath(@"..\..\..\TestHarness\bin\AssemblyBottle.dll"))
                .GetManifestResourceStream("AssemblyBottle.pak-WebContent.zip").UnZipTextFile("Content/Styles/test.css").Trim();

            _testWebsite.DownloadString("_content/test.css").Trim().ShouldEqual(embeddedFileContents);
        }

        [Test]
        public void should_only_call_IFubuRegistryExtension_Configure_once()
        {
            _testWebsite.DownloadString(); // Ensure the bootstrapping has run
            var startupLog = File.ReadLines(BottleStartupLogPath).ToList();
            startupLog.ForEach(Console.WriteLine);
            startupLog.Count(x => !x.IsEmpty()).ShouldEqual(1);
        }
    }
}