using RefactorThis.Persistence.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RefactorThis.Persistence 
{
	public class InvoiceRepository : IInvoiceRepository
    {
        // This is a simple in-memory store for demonstration purposes.
        // In a real implementation, this would interact with a database.
        private readonly List<Invoice> _invoiceStore = new List<Invoice>();

		public Task<Invoice> GetInvoiceAsync(string reference, CancellationToken cancellationToken)
		{
			return Task.FromResult(_invoiceStore.Where(inv => inv.Reference == reference).FirstOrDefault()); // Or FirstOrDefaultAsync on EF6+
        }

		public async Task SaveInvoiceAsync(Invoice invoice, CancellationToken cancellationToken)
		{
            // Simple drop-and-replace logic for the in-memory store.
            // In a real implementation, this would update the record in the database.

            var existingInvoice = await GetInvoiceAsync(invoice.Reference, cancellationToken);
            if (existingInvoice != null)
                _invoiceStore.Remove(existingInvoice);

            _invoiceStore.Add(invoice);  // or AddAsync with cancellation token on EF Core
        }

		public Task AddAsync(Invoice invoice, CancellationToken cancellationToken)
		{
            invoice.Reference = System.Guid.NewGuid().ToString();

            _invoiceStore.Add(invoice); // or AddAsync with cancellation token on EF Core

            return Task.CompletedTask;
        }
	}
}