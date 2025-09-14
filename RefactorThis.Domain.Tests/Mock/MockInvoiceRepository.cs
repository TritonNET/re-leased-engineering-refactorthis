using RefactorThis.Persistence;
using RefactorThis.Persistence.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RefactorThis.Domain.Tests.Mock
{
    internal class MockInvoiceRepository : IInvoiceRepository
    {
        // This is a mock in-memory store for testing purposes.
        private readonly List<Invoice> _invoiceStore = new List<Invoice>();

        public Task<Invoice> GetInvoiceAsync(string reference, CancellationToken cancellationToken)
        {
            return Task.FromResult(_invoiceStore.Where(inv => inv.Reference == reference).FirstOrDefault());
        }

        public async Task SaveInvoiceAsync(Invoice invoice, CancellationToken cancellationToken)
        {
            var existingInvoice = await GetInvoiceAsync(invoice.Reference, cancellationToken);
            if (existingInvoice != null)
                _invoiceStore.Remove(existingInvoice);

            _invoiceStore.Add(invoice);
        }

        public Task AddAsync(Invoice invoice, CancellationToken cancellationToken)
        {
            invoice.Reference = System.Guid.NewGuid().ToString();

            _invoiceStore.Add(invoice);

            return Task.CompletedTask;
        }
    }
}
