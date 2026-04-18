using GLMS2.Interfaces;

namespace GLMS2.Services.Mediator
{
    public class NotificationModule
    {
        private readonly IMediator _mediator;

        public NotificationModule(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void SendNotification()
        {
            _mediator.Notify(this, "NotificationSent");
        }
    }
}