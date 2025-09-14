using System.Threading;
using System.Threading.Tasks;

namespace RefactorThis.Persistence.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<Invoice> GetInvoiceAsync(string reference, CancellationToken cancellationToken);

        Task SaveInvoiceAsync(Invoice invoice, CancellationToken cancellationToken);

        Task AddAsync(Invoice invoice, CancellationToken cancellationToken);
    }
}
