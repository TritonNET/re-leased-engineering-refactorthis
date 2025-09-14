using NUnit.Framework;
using RefactorThis.Domain.Tests.Mock;
using RefactorThis.Persistence;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RefactorThis.Domain.Tests
{
	[TestFixture]
	public class InvoicePaymentProcessorTests
	{
		[Test]
		public void ProcessPayment_Should_ThrowException_When_NoInoiceFoundForPaymentReference()
		{
			var paymentProcessor = new InvoiceService(new MockInvoiceRepository(), new MockTaxService());

            Assert.ThrowsAsync<InvalidOperationException>(
				() => paymentProcessor.ProcessPaymentAsync(new Payment() { Reference = "", Amount = 10 }, CancellationToken.None),
				"There is no invoice matching this payment");
		}

		[Test]
		public async Task ProcessPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded( )
		{
			var repo = new MockInvoiceRepository();

			var invoice = new Invoice()
			{
				Reference = Guid.NewGuid().ToString(),
                Amount = 0,
			};

			await repo.AddAsync(invoice, CancellationToken.None);

			var paymentProcessor = new InvoiceService(repo, new MockTaxService());

			var payment = new Payment()
			{
				Reference = invoice.Reference,
				Amount = 10
            };

            (bool status, string message) = await paymentProcessor.ProcessPaymentAsync(payment, CancellationToken.None);

			Assert.IsFalse(status);
            Assert.AreEqual("no payment needed", message);
		}

		[Test]
		public async Task ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid( )
		{
			var repo = new MockInvoiceRepository();

			var invoice = new Invoice()
			{
				Reference = Guid.NewGuid().ToString(),
                Amount = 10
			};
			invoice.Payments.Add(new Payment() { Amount = 10 });

            await repo.AddAsync(invoice, CancellationToken.None);

			var paymentProcessor = new InvoiceService(repo, new MockTaxService());

			var payment = new Payment()
			{
				Reference = invoice.Reference,
				Amount = 10
            };

			(bool status, string message) = await paymentProcessor.ProcessPaymentAsync(payment, CancellationToken.None);
			
			Assert.IsFalse(status);
            Assert.AreEqual("invoice was already fully paid", message);
		}

		[Test]
		public async Task ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue( )
		{
			var repo = new MockInvoiceRepository();
			var invoice = new Invoice()
			{
				Reference = Guid.NewGuid().ToString(),
                Amount = 10
			};
			invoice.Payments.Add(new Payment() { Amount = 5 });

            await repo.AddAsync(invoice, CancellationToken.None);

			var paymentProcessor = new InvoiceService(repo, new MockTaxService());

			var payment = new Payment()
			{
				Reference = invoice.Reference,
                Amount = 6
			};

            (bool status, string message) = await paymentProcessor.ProcessPaymentAsync(payment, CancellationToken.None);

			Assert.IsFalse(status);
            Assert.AreEqual("the payment is greater than the partial amount remaining", message);
		}

		[Test]
		public async Task ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount( )
		{
			var repo = new MockInvoiceRepository();
			var invoice = new Invoice()
			{
				Reference = Guid.NewGuid().ToString(),
                Amount = 5,
			};

			await repo.AddAsync(invoice, CancellationToken.None);

			var paymentProcessor = new InvoiceService(repo, new MockTaxService());

			var payment = new Payment( )
			{
				Reference = invoice.Reference,
                Amount = 6
			};

            (bool status, string message) =  await paymentProcessor.ProcessPaymentAsync(payment, CancellationToken.None);

			Assert.IsFalse(status);
            Assert.AreEqual( "the payment is greater than the invoice amount", message);
		}

		[Test]
		public async Task ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue( )
		{
			var repo = new InvoiceRepository();
			var invoice = new Invoice()
			{
				Reference = Guid.NewGuid().ToString(),
				Amount = 10
			};
			invoice.Payments.Add(new Payment() { Amount = 5 });

            await repo.AddAsync(invoice, CancellationToken.None);

			var paymentProcessor = new InvoiceService(repo, new MockTaxService());

			var payment = new Payment()
			{
				Reference = invoice.Reference,
                Amount = 5
			};

            (bool status, string message) = await paymentProcessor.ProcessPaymentAsync(payment, CancellationToken.None);

			Assert.IsTrue(status);
            Assert.AreEqual("final partial payment received, invoice is now fully paid", message);
		}

		[Test]
		public async Task ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount()
		{
			var repo = new InvoiceRepository();
			var invoice = new Invoice()
			{
				Reference = Guid.NewGuid().ToString(),
				Amount = 10,
			};
			invoice.Payments.Add(new Payment() { Amount = 10 });

            await repo.AddAsync(invoice, CancellationToken.None);

			var paymentProcessor = new InvoiceService(repo, new MockTaxService());

			var payment = new Payment()
			{
				Reference = invoice.Reference,
                Amount = 10
			};

            (bool status, string message) = await paymentProcessor.ProcessPaymentAsync(payment, CancellationToken.None);

			Assert.IsFalse(status);
            Assert.AreEqual("invoice was already fully paid", message);
		}

		[Test]
		public async Task ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue()
		{
			var repo = new InvoiceRepository();
			var invoice = new Invoice()
			{
				Reference = Guid.NewGuid().ToString(),
                Amount = 10
			};
			invoice.Payments.Add(new Payment() { Amount = 5 });

            await repo.AddAsync(invoice, CancellationToken.None);

			var paymentProcessor = new InvoiceService(repo, new MockTaxService());

			var payment = new Payment()
			{
				Reference = invoice.Reference,
                Amount = 1
			};

            (bool status, string message) = await paymentProcessor.ProcessPaymentAsync(payment, CancellationToken.None);

			Assert.IsTrue(status);
            Assert.AreEqual("another partial payment received, still not fully paid", message);
		}

		[Test]
		public async Task ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount()
		{
			var repo = new InvoiceRepository();
			var invoice = new Invoice()
			{
				Reference = Guid.NewGuid().ToString(),
                Amount = 10,
			};

            await repo.AddAsync(invoice, CancellationToken.None);

			var paymentProcessor = new InvoiceService(repo, new MockTaxService());

			var payment = new Payment()
			{
				Reference = invoice.Reference,
                Amount = 1
			};

            (bool status, string message) = await paymentProcessor.ProcessPaymentAsync(payment, CancellationToken.None);

			Assert.IsTrue(status);
            Assert.AreEqual("invoice is now partially paid", message);
		}
	}
}