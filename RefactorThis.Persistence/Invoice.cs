using RefactorThis.Persistence.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace RefactorThis.Persistence
{
	public class Invoice
	{
		public Invoice()
		{
			Payments = new List<Payment>();
        }

        public string Reference { get; set; }

        public decimal Amount { get; set; }

        // AmountPaid is a derived property that calculates the total amount paid based on the Payments list.
		// This may or may want to store.
        public decimal AmountPaid
		{
			get => Payments == null ? 0 : Payments.Sum(x => x.Amount);
        }
		
		public decimal TaxAmount { get; set; }

		public readonly List<Payment> Payments;
		
		public InvoiceType Type { get; set; }
	}

    /// <summary>
	/// Invoice types
	/// </summary>
    public enum InvoiceType
	{
        /// <summary>
        /// Standard invoice type
        /// </summary>
        Standard,

        /// <summary>
		/// Commercial invoice type
		/// </summary>
        Commercial
    }
}