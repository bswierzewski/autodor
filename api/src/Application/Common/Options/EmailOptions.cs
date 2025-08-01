namespace Application.Common.Options
{
    public class EmailOptions
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string From { get; set; }
        public string[] To { get; set; }
        public bool UseSsl { get; set; }
    }
}
