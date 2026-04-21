using GLMS2.Interfaces;

namespace GLMS2.Services.Observers
{
    // Subject that tracks changes to service request status
    // Notifies observers whenever the status is updated
    public class ServiceRequestSubject : ISubject
    {
        private readonly List<IObserver> _observers = new();

        // Identifies the service request being tracked
        public int ServiceRequestId { get; set; }

        // Stores the current status of the service request
        public string CurrentStatus { get; private set; } = string.Empty;

        // Records when the status was last changed
        public DateTime UpdatedAt { get; private set; }

        // Adds an observer to receive updates
        public void Attach(IObserver observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }

        // Removes an observer from receiving updates
        public void Detach(IObserver observer)
        {
            if (_observers.Contains(observer))
            {
                _observers.Remove(observer);
            }
        }

        // Notifies all observers when the status changes
        public void Notify()
        {
            foreach (var observer in _observers)
            {
                observer.Update(this);
            }
        }

        // Returns the current status value
        public string GetStatus()
        {
            return CurrentStatus;
        }

        // Updates the status and triggers observer notifications
        public void SetStatus(string status)
        {
            CurrentStatus = status;
            UpdatedAt = DateTime.UtcNow;
            Notify();
        }
    }
}