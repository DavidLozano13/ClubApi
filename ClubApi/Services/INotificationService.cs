namespace ClubApi.Services
{
    public interface INotificationService
    {
        /// <summary>
        /// Enviar notificación
        /// </summary>
        /// <param name="mail"></param>
        void SendNotification(string mail);
    }
}