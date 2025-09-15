using RefactorThis.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace RefactorThis.Domain
{
    /// <summary>
    /// Tax service interface for calculating tax on invoices.
    /// </summary>  
    public interface ITaxService
    {
        /// <summary>
        /// Computes the tax for the specified invoice.
        /// </summary>
        /// <param name="invoice">The invoice to evaluate.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>The calculated tax amount.</returns>
        /// <remarks>
        /// For this demo, the entire invoice is passed to support potentially complex tax rules.
        /// In a production system, a smaller model or specific parameters might be sufficient.
        /// </remarks>
        Task<decimal> CalculateTaxAsync(Invoice invoice, CancellationToken cancellationToken);
    }
}
