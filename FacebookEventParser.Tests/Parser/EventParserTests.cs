using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FacebookEventParser.Parser;
using Microsoft.Extensions.Logging;       
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace FacebookEventParser.Tests.Parser
{
    public class EventParserTests : IClassFixture<TestCaseLoader>
    {
        private readonly IList<TestCaseContainer> _testCases;
        public EventParserTests(TestCaseLoader testCaseLoader)
        {
            _testCases = testCaseLoader.TestCases;
        }

        //https://codepen.io/nosilleg/pen/KdmLPO

        [Fact]
        public void EventTexteWerdenKorrektGeparsed()
        {
            var loggerMoq = new Mock<ILogger<EventParser>>();
            
            var errorTxt = new StringBuilder();
            var testCaseCounter = 0;
            foreach (var curTestCase in _testCases)
            {
                var eventParser = new EventParser(loggerMoq.Object, new RegExContainer());
                var erstellteVeranstaltung = eventParser.ParseVeranstaltung(curTestCase.EventText);

                if (curTestCase.Ort != erstellteVeranstaltung.Ort)
                {
                    errorTxt.AppendLine($"Fehler bei {Environment.NewLine}'{curTestCase.EventText}'{Environment.NewLine}Ort stimmt nicht. Erwartet: '{curTestCase.Ort}' ### ist: '{erstellteVeranstaltung.Ort}'");
                }

                if (curTestCase.Stadt != erstellteVeranstaltung.Stadt)
                {
                    errorTxt.AppendLine($"Fehler bei {Environment.NewLine}'{curTestCase.EventText}'{Environment.NewLine}Stadt stimmt nicht. Erwartet: '{curTestCase.Stadt}' ### ist: '{erstellteVeranstaltung.Stadt}'");
                }

                if (curTestCase.ZeitStart != erstellteVeranstaltung.ZeitStart)
                {
                    errorTxt.AppendLine($"Fehler bei {Environment.NewLine}'{curTestCase.EventText}'{Environment.NewLine}ZeitStart stimmt nicht. Erwartet: '{curTestCase.ZeitStart}' ### ist: '{erstellteVeranstaltung.ZeitStart}'");
                }

                testCaseCounter++;
            }

            var errorStr = errorTxt.ToString();
            if (!string.IsNullOrEmpty(errorStr))
            {
                throw new Exception(errorStr);
            }

            Assert.True(testCaseCounter > 0);
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
