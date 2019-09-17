using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WordPressSharp;
using WordPressSharp.Models;


namespace WordpressPublisher
{
    public interface IWordPressApi
    {
        Task UpdatePage(int pageId, string neuerHtmlCode, WordPressCredentials credentials);
    }
    public class WordPressApi : IWordPressApi
    {
        private readonly ILogger<WordPressApi> _logger;

        public WordPressApi(ILogger<WordPressApi> logger)
        {
            _logger = logger;
        }

        public async Task UpdatePage(int pageId, string neuerHtmlCode, WordPressCredentials credentials)
        {
            _logger.LogInformation("Starte WordPress-API");
            var config = new WordPressSiteConfig
            {
                BaseUrl = credentials.BaseUrl,
                Username = credentials.Username,
                Password = credentials.Password
            };

            using (var client = new WordPressClient(config))
            {
                var post = new Post
                {
                    Id = Convert.ToString(pageId), 
                    Content = neuerHtmlCode
                };

                var sucess = await client.EditPostAsync(post);
                if (!sucess)
                {
                    throw new Exception($"EditPostAsync gab false zurück :(");
                }
            }
        }
    }
}