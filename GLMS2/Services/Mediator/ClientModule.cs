using GLMS2.Interfaces;

namespace GLMS2.Services.Mediator
{
    public class ClientModule
    {
        private readonly IMediator _mediator;

        public ClientModule(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void RegisterClient()
        {
            _mediator.Notify(this, "ClientRegistered");
        }
    }
}