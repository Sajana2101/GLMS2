using GLMS2.Interfaces;

namespace GLMS2.Services.Observers
{
    public class ComplianceObserver : IObserver
    {
        public void Update(ISubject subject)
        {
            CheckCompliance(subject);
        }

        private void CheckCompliance(ISubject subject)
        {
            var status = subject.GetStatus();

            if (status.Equals("Expired", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Compliance check: Contract is expired.");
            }
        }
    }
}