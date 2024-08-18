using Braintree;
using Microsoft.Extensions.Options;

namespace MansorySupplyHub.BrainTree
{
    public class BrainTreeGate : IBrainTreeGate
    {
        private readonly BrainTreeSettings _options;
        private IBraintreeGateway _brainTreeGateway;

        public BrainTreeGate(IOptions<BrainTreeSettings> options)
        {
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public IBraintreeGateway CreateGateway()
        {
            if (string.IsNullOrEmpty(_options.Environment) ||
                string.IsNullOrEmpty(_options.MerchantId) ||
                string.IsNullOrEmpty(_options.PublicKey) ||
                string.IsNullOrEmpty(_options.PrivateKey))
            {
                throw new InvalidOperationException("Braintree configuration settings are missing.");
            }

            return new BraintreeGateway(
                _options.Environment,
                _options.MerchantId,
                _options.PublicKey,
                _options.PrivateKey
            );
        }

        public IBraintreeGateway GetGateway()
        {
            // Initialize the gateway only if it hasn't been created yet
            return _brainTreeGateway ??= CreateGateway();
        }
    }
}
