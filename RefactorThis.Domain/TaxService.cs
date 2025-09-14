using RefactorThis.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RefactorThis.Domain
{
    public class TaxService : ITaxService
    {
        public Task<decimal> CalculateTaxAsync(Invoice invoice, CancellationToken cancellationToken)
        {
            switch (invoice.Type)
            {
                case InvoiceType.Standard:
                    return Task.FromResult(0m);
                case InvoiceType.Commercial:
                    return Task.FromResult(invoice.Amount * 0.14m);
                default:
                    throw new ArgumentOutOfRangeException(nameof(invoice.Type), invoice.Type, null);
            }
        }
    }
}
