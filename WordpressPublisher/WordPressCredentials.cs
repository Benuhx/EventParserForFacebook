namespace WordpressPublisher
{
    public class WordPressCredentials
    {
        public WordPressCredentials(string username, string password, string baseUrl)
        {
            Username = username;
            Password = password;
            BaseUrl = baseUrl;
        }

        public string Username { get; private set; }
        public string Password {get; private set;}
        public string BaseUrl { get; private set; }
    }
}
