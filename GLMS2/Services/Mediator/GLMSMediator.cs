using GLMS2.Interfaces;

namespace GLMS2.Services.Mediator
{
    public class GLMSMediator : IMediator
    {
        public void Notify(object sender, string eventType)
        {
            switch (eventType)
            {
                case "ContractApproved":
                    Console.WriteLine("Mediator: handling contract approval workflow.");
                    break;

                case "ServiceRequestCreated":
                    Console.WriteLine("Mediator: handling service request workflow.");
                    break;

                case "ClientRegistered":
                    Console.WriteLine("Mediator: handling client registration workflow.");
                    break;

                case "CurrencyConverted":
                    Console.WriteLine("Mediator: handling currency conversion workflow.");
                    break;

                case "NotificationSent":
                    Console.WriteLine("Mediator: notification process completed.");
                    break;
            }
        }
    }
}