using GLMS2.Interfaces;

namespace GLMS2.Services.Observers
{
    public class ServiceRequestSubject : ISubject
    {
        private readonly List<IObserver> _observers = new();

        public int ServiceRequestId { get; set; }
        public string CurrentStatus { get; private set; } = string.Empty;
        public DateTime UpdatedAt { get; private set; }

        public void Attach(IObserver observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }

        public void Detach(IObserver observer)
        {
            if (_observers.Contains(observer))
            {
                _observers.Remove(observer);
            }
        }

        public void Notify()
        {
            foreach (var observer in _observers)
            {
                observer.Update(this);
            }
        }

        public string GetStatus()
        {
            return CurrentStatus;
        }

        public void SetStatus(string status)
        {
            CurrentStatus = status;
            UpdatedAt = DateTime.UtcNow;
            Notify();
        }
    }
}