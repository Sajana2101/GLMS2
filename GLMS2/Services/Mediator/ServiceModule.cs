using GLMS2.Interfaces;

namespace GLMS2.Services.Mediator
{
    public class ServiceModule
    {
        private readonly IMediator _mediator;

        public ServiceModule(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void CreateServiceRequest()
        {
            _mediator.Notify(this, "ServiceRequestCreated");
        }
    }
}