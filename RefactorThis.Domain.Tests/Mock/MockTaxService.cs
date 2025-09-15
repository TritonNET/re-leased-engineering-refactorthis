using RefactorThis.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace RefactorThis.Domain.Tests.Mock
{
    internal class MockTaxService : ITaxService
    {
        public Task<decimal> CalculateTaxAsync(Invoice invoice, CancellationToken cancellationToken)
        {
            if (invoice.Type == InvoiceType.Commercial)
                return Task.FromResult(invoice.Amount * 0.14m);

            return Task.FromResult(0m);
        }
    }
}
