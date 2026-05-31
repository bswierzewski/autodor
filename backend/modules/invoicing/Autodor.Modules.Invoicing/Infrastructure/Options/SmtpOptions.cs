namespace Autodor.Modules.Invoicing.Infrastructure.Options;

public class SmtpOptions
{
    public const string SectionName = "Email:Smtp";

    // @env: Email__Smtp__Host=smtp.gmail.com
    public string Host { get; set; } = "smtp.gmail.com";

    // @env: Email__Smtp__Port=587
    public int Port { get; set; } = 587;

    // @env: Email__Smtp__Username=
    public string Username { get; set; } = string.Empty;

    // @env: Email__Smtp__Password=
    public string Password { get; set; } = string.Empty;
}
