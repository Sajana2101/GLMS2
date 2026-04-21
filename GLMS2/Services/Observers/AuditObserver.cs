using GLMS2.Interfaces;

namespace GLMS2.Services.Observers
{
    // Observer that tracks changes for audit purposes
    // Updates logs or reports when the subject status changes
    public class AuditObserver : IObserver
    {
        // Called automatically when the subject notifies observers
        public void Update(ISubject subject)
        {
            RefreshReport(subject);
        }

        // Simulates updating audit records based on the current status
        private void RefreshReport(ISubject subject)
        {
            Console.WriteLine($"Audit updated for status: {subject.GetStatus()}");
        }
    }
}