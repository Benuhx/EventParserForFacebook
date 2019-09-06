using System;
using System.Collections.Generic;
using System.IO;
using FacebookEventParser.Parser;
using Microsoft.Extensions.Logging;       
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace FacebookEventParser.Tests.Parser
{
    public class EventParserTests : IClassFixture<TestCaseLoader>
    {
        private IList<TestCaseContainer> _testCases;
        public EventParserTests(TestCaseLoader testCaseLoader)
        {
            _testCases = testCaseLoader.TestCases;
        }

        [Fact]
        public void EventTexteWerdenKorrektGeparsed()
        {
            var loggerMoq = new Mock<ILogger<EventParser>>();
            foreach (var curTestCase in _testCases)
            {
                var eventParser = new EventParser(loggerMoq.Object, new RegExContainer());
                var erstellteVeranstaltung = eventParser.ParseVeranstaltung(curTestCase.EventText);

                Assert.Equal(curTestCase.Ort, erstellteVeranstaltung.Ort);
                Assert.Equal(curTestCase.Stadt, erstellteVeranstaltung.Stadt);
                Assert.Equal(curTestCase.ZeitStart, erstellteVeranstaltung.ZeitStart);
            }
        }
    }

    public class TestCaseLoader : IDisposable
    {
        public TestCaseLoader()
        {
            TestCases = new List<TestCaseContainer>();

            var testCasePath = Path.Combine(Directory.GetCurrentDirectory(), "Parser", "TestCases.json");
            var testCaseString = File.ReadAllText(testCasePath);
            
            TestCases = JsonConvert.DeserializeObject<List<TestCaseContainer>>(testCaseString, new JsonSerializerSettings
            {
                DateFormatString = "dd.MM.yyyy - HH:mm"
            });
        }

        public IList<TestCaseContainer> TestCases { get; private set; }


        public void Dispose()
        {
            TestCases = null;
        }
    }

    public class TestCaseContainer
    {
        public List<string> EventTextArray { get; set; }
        
        [JsonIgnore]
        public string EventText => string.Join(Environment.NewLine, EventTextArray);
        
        public DateTime ZeitStart { get; set; }
        
        public string Ort { get; set; }
        
        public string Stadt { get; set; }
    }
}
