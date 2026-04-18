using GLMS2.Interfaces;

namespace GLMS2.Services.Observers
{
    public class AuditObserver : IObserver
    {
        public void Update(ISubject subject)
        {
            RefreshReport(subject);
        }

        private void RefreshReport(ISubject subject)
        {
            Console.WriteLine($"Audit updated for status: {subject.GetStatus()}");
        }
    }
}