namespace Application.Common.Options
{
    public class SendGridOptions
    {
        public string Key { get; set; }
        public string From { get; set; }
        public string[] To { get; set; }
    }
}
