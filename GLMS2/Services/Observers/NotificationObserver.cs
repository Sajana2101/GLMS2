using GLMS2.Interfaces;

namespace GLMS2.Services.Observers
{
    // Observer that sends a notification when the status changes
    // Responds automatically when the subject triggers an update
    public class NotificationObserver : IObserver
    {
        // Called when the subject notifies observers
        public void Update(ISubject subject)
        {
            SendNotification(subject);
        }

        // Simulates sending a notification based on the current status
        private void SendNotification(ISubject subject)
        {
            Console.WriteLine($"Notification sent for status change: {subject.GetStatus()}");
        }
    }
}