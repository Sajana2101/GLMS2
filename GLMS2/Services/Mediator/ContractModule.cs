using GLMS2.Interfaces;

namespace GLMS2.Services.Mediator
{
    public class ContractModule
    {
        private readonly IMediator _mediator;

        public ContractModule(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void ApproveContract()
        {
            _mediator.Notify(this, "ContractApproved");
        }
    }
}