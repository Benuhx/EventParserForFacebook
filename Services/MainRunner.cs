using SixLabors.ImageSharp;

namespace JuLiMl.Services
{
    public interface IMainRunner
    {
        void Run();
    }
    public class MainRunner : IMainRunner
    {
        private readonly ISeleniumService _seleniumService;
        private readonly IEventBoxenService _eventBoxenService;
        public MainRunner(ISeleniumService seleniumService, IEventBoxenService eventBoxenService)
        {
            _seleniumService = seleniumService;
            _eventBoxenService = eventBoxenService;
        }
        public void Run()
        {
            var url = "https://www.facebook.com/pg/julisbochum/events/";
            var screenshot = _seleniumService.ErstelleScreenshotVonWebsite(url);
            screenshot.Save("a.png");
            screenshot.Dispose();
            screenshot = Image.Load("a.png");
            _eventBoxenService.FindeEventBoxen(screenshot);
        }
    }
}
