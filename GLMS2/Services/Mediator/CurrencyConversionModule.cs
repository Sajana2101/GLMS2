using GLMS2.Interfaces;

namespace GLMS2.Services.Mediator
{
    public class CurrencyConversionModule
    {
        private readonly IMediator _mediator;

        public CurrencyConversionModule(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void ConvertCurrency()
        {
            _mediator.Notify(this, "CurrencyConverted");
        }
    }
}