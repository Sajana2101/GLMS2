using GLMS2.Interfaces;

namespace GLMS2.Services.Observers
{
    public class NotificationObserver : IObserver
    {
        public void Update(ISubject subject)
        {
            SendNotification(subject);
        }

        private void SendNotification(ISubject subject)
        {
            Console.WriteLine($"Notification sent for status change: {subject.GetStatus()}");
        }
    }
}