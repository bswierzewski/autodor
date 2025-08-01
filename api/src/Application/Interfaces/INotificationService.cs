namespace Application.Interfaces
{
    public interface INotificationService
    {
        Task<bool> Send(string[] adresses, string subject, string html);
    }
}
