using RefactorThis.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace RefactorThis.Domain
{
    /// <summary>
    /// Service for processing invoices.
    /// </summary>
    internal interface IInvoiceService
    {
        /// <summary>Applies the payment to its invoice and returns a status message.</summary>
        /// <param name="payment">Payment with invoice reference and amount.</param>
        /// <returns>A status message.</returns>
        Task<(bool status, string message)> ProcessPaymentAsync(Payment payment, CancellationToken cancellationToken);
    }
}
