using RefactorThis.Persistence;
using RefactorThis.Persistence.Interfaces;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RefactorThis.Domain
{
	public class InvoiceService : IInvoiceService
    {
		private readonly IInvoiceRepository _invoiceRepository;
        private readonly ITaxService _taxService;

        public InvoiceService(IInvoiceRepository invoiceRepository, ITaxService taxService)
		{
			_invoiceRepository = invoiceRepository;
            _taxService = taxService;
        }

		public async Task<(bool status, string message)> ProcessPaymentAsync(Payment payment, CancellationToken cancellationToken)
        {
            if(payment == null)
                throw new ArgumentNullException(nameof(payment));

            if(payment.Amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(payment), "Payment amount must be greater than zero.");

            var inv = await _invoiceRepository.GetInvoiceAsync(payment.Reference, cancellationToken);
			if (inv == null)
				throw new InvalidOperationException($"There is no invoice matching this payment reference '{payment.Reference}'");

            if (inv.Amount == 0)
            {
                // This is a logic error, rather than a data error. No point throwing a client exception.
                // An invoice with an amount of 0 should not have any payments associated with it.
                Debug.Assert(inv.Payments.Count == 0, "The invoice is in an invalid state, it has an amount of 0 and it has payments.");

                return (false, "no payment needed");
            }
            else if (payment.Amount > inv.Amount)
            {
                return (false, "the payment is greater than the invoice amount");
            }
            else if (inv.Amount == inv.AmountPaid)
            {
                return (false, "invoice was already fully paid");
            }
            else if (inv.AmountPaid != 0 && payment.Amount > (inv.Amount - inv.AmountPaid))
            {
                return (false, "the payment is greater than the partial amount remaining");
            }

            string status;
            if (inv.AmountPaid > 0)
            {
                if ((inv.Amount - inv.AmountPaid) == payment.Amount)
                    status = "final partial payment received, invoice is now fully paid";
                else
                    status = "another partial payment received, still not fully paid";
            }
            else
            {
                if (inv.Amount == payment.Amount)
                    status = "invoice is now fully paid";
                else
                    status = "invoice is now partially paid";
            }

            inv.Payments.Add(payment);
            inv.TaxAmount = await _taxService.CalculateTaxAsync(inv, cancellationToken);

            await _invoiceRepository.SaveInvoiceAsync(inv, cancellationToken);

			return (true, status);
		}
	}
}