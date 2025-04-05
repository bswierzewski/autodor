namespace Application.Interfaces
{
    public interface ISendGridService
    {
        Task<bool> SendEmail(string[] adresses, string subject, string html);
    }
}
