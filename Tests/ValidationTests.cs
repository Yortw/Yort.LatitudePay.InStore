using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yort.LatitudePay.InStore.Tests
{
	[TestClass]
	public class ValidationTests
	{

		#region CreatePosPurchaseRequest Validation

		[TestMethod]
		public async Task CreatePosPurchaseRequest_RequiresReference()
		{
			using (var client = TestUtils.GetSandboxClient())
			{
				var request = CreateMinimalPurchaseRequest();
				request.Reference = null;

				// Test for null
				try
				{
					var purchaseResponse = await client.CreatePosPurchaseAsync(request);
					await client.CancelPurchaseAsync(new LatitudePayCancelPurchaseRequest() { PaymentPlanToken = purchaseResponse.Token });
					Assert.Fail("ArgumentNullException expected");
				}
				catch (ArgumentNullException ane)
				{
					Assert.AreEqual("request." + nameof(request.Reference), ane.ParamName);
				}

				//Test for empty
				try
				{
					request.Reference = String.Empty;
					var purchaseResponse = await client.CreatePosPurchaseAsync(request);
					await client.CancelPurchaseAsync(new LatitudePayCancelPurchaseRequest() { PaymentPlanToken = purchaseResponse.Token });
					Assert.Fail("ArgumentException expected");
				}
				catch (ArgumentException ane)
				{
					Assert.AreEqual("request." + nameof(request.Reference), ane.ParamName);
				}

				// Test for whitespace only
				try
				{
					request.Reference = "  \t";
					var purchaseResponse = await client.CreatePosPurchaseAsync(request);
					await client.CancelPurchaseAsync(new LatitudePayCancelPurchaseRequest() { PaymentPlanToken = purchaseResponse.Token });
					Assert.Fail("ArgumentException expected");
				}
				catch (ArgumentException ane)
				{
					Assert.AreEqual("request." + nameof(request.Reference), ane.ParamName);
				}

			}
		}

		[TestMethod]
		public async Task CreatePosPurchaseRequest_RequiresCustomer()
		{
			using (var client = TestUtils.GetSandboxClient())
			{
				var request = CreateMinimalPurchaseRequest();
				request.Customer = null;

				// Test for null
				try
				{
					var purchaseResponse = await client.CreatePosPurchaseAsync(request);
					await client.CancelPurchaseAsync(new LatitudePayCancelPurchaseRequest() { PaymentPlanToken = purchaseResponse.Token });
					Assert.Fail("ArgumentNullException expected");
				}
				catch (ArgumentNullException ane)
				{
					Assert.AreEqual("request." + nameof(request.Customer), ane.ParamName);
				}
			}
		}

		[TestMethod]
		public async Task CreatePosPurchaseRequest_RequiresCustomerMobileNumber()
		{
			using (var client = TestUtils.GetSandboxClient())
			{
				var request = CreateMinimalPurchaseRequest();
				request.Customer.MobileNumber = null;

				// Test for null
				try
				{
					var purchaseResponse = await client.CreatePosPurchaseAsync(request);
					await client.CancelPurchaseAsync(new LatitudePayCancelPurchaseRequest() { PaymentPlanToken = purchaseResponse.Token });
					Assert.Fail("ArgumentNullException expected");
				}
				catch (ArgumentNullException ane)
				{
					Assert.AreEqual("request." + nameof(request.Customer) + "." + nameof(request.Customer.MobileNumber), ane.ParamName);
				}

				//Test for empty
				try
				{
					request.Customer.MobileNumber = String.Empty;
					var purchaseResponse = await client.CreatePosPurchaseAsync(request);
					await client.CancelPurchaseAsync(new LatitudePayCancelPurchaseRequest() { PaymentPlanToken = purchaseResponse.Token });
					Assert.Fail("ArgumentException expected");
				}
				catch (ArgumentException ane)
				{
					Assert.AreEqual("request." + nameof(request.Customer) + "." + nameof(request.Customer.MobileNumber), ane.ParamName);
				}

				// Test for whitespace only
				try
				{
					request.Customer.MobileNumber = "  \t";
					var purchaseResponse = await client.CreatePosPurchaseAsync(request);
					await client.CancelPurchaseAsync(new LatitudePayCancelPurchaseRequest() { PaymentPlanToken = purchaseResponse.Token });
					Assert.Fail("ArgumentException expected");
				}
				catch (ArgumentException ane)
				{
					Assert.AreEqual("request." + nameof(request.Customer) + "." + nameof(request.Customer.MobileNumber), ane.ParamName);
				}

			}
		}

		[TestMethod]
		public async Task CreatePosPurchaseRequest_RequiresPositiveAmount()
		{
			using (var client = TestUtils.GetSandboxClient())
			{
				var request = CreateMinimalPurchaseRequest();
				request.TotalAmount = new LatitudePayMoney(0M, LatitudePayCurrencies.NewZealandDollars);

				// Test for zero
				try
				{
					var purchaseResponse = await client.CreatePosPurchaseAsync(request);
					await client.CancelPurchaseAsync(new LatitudePayCancelPurchaseRequest() { PaymentPlanToken = purchaseResponse.Token });
					Assert.Fail("ArgumentOutOfRangeException expected");
				}
				catch (ArgumentOutOfRangeException aore)
				{
					Assert.AreEqual("request." + nameof(request.TotalAmount), aore.ParamName);
				}

				//Test for negative
				try
				{
					request.TotalAmount = new LatitudePayMoney(-10M, LatitudePayCurrencies.NewZealandDollars);
					var purchaseResponse = await client.CreatePosPurchaseAsync(request);
					await client.CancelPurchaseAsync(new LatitudePayCancelPurchaseRequest() { PaymentPlanToken = purchaseResponse.Token });
					Assert.Fail("ArgumentOutOfRangeException expected");
				}
				catch (ArgumentOutOfRangeException aore)
				{
					Assert.AreEqual("request." + nameof(request.TotalAmount), aore.ParamName);
				}
			}
		}

		[TestMethod]
		public async Task CreatePosPurchaseRequest_RequiresZeroOrPositiveAmount()
		{
			using (var client = TestUtils.GetSandboxClient())
			{
				var request = CreateMinimalPurchaseRequest();
				request.TaxAmount = new LatitudePayMoney(-1M, LatitudePayCurrencies.NewZealandDollars);

				Yort.LatitudePay.InStore.LatitudePayCreatePosPurchaseResponse purchaseResponse;
				
				// Test for zero
				try
				{
					purchaseResponse = await client.CreatePosPurchaseAsync(request);
					await client.CancelPurchaseAsync(new LatitudePayCancelPurchaseRequest() { PaymentPlanToken = purchaseResponse.Token });
					Assert.Fail("ArgumentOutOfRangeException expected");
				}
				catch (ArgumentOutOfRangeException aore)
				{
					Assert.AreEqual("request." + nameof(request.TaxAmount), aore.ParamName);
				}

				//Test for zero allowed
				request.TaxAmount = new LatitudePayMoney(0M, LatitudePayCurrencies.NewZealandDollars);
				purchaseResponse = await client.CreatePosPurchaseAsync(request);
				await client.CancelPurchaseAsync(new LatitudePayCancelPurchaseRequest() { PaymentPlanToken = purchaseResponse.Token });
			}
		}

		[TestMethod]
		public async Task CreatePosPurchaseRequest_RequiresProducts()
		{
			using (var client = TestUtils.GetSandboxClient())
			{
				var request = CreateMinimalPurchaseRequest();

				// Test for null
				try
				{
					request.Products = null;
					var purchaseResponse = await client.CreatePosPurchaseAsync(request);
					await client.CancelPurchaseAsync(new LatitudePayCancelPurchaseRequest() { PaymentPlanToken = purchaseResponse.Token });
					Assert.Fail("ArgumentNullException expected");
				}
				catch (ArgumentNullException ane)
				{
					Assert.AreEqual("request." + nameof(request.Products), ane.ParamName);
				}

				//Test for empty
				try
				{
					request.Products = new Yort.LatitudePay.InStore.LatitudePayProduct[] { };
					var purchaseResponse = await client.CreatePosPurchaseAsync(request);
					await client.CancelPurchaseAsync(new LatitudePayCancelPurchaseRequest() { PaymentPlanToken = purchaseResponse.Token });
					Assert.Fail("ArgumentException expected");
				}
				catch (ArgumentException ane)
				{
					Assert.AreEqual("request." + nameof(request.Products), ane.ParamName);
				}

				// Test for only null entries
				try
				{
					request.Products = new Yort.LatitudePay.InStore.LatitudePayProduct[] { null, null, null };
					var purchaseResponse = await client.CreatePosPurchaseAsync(request);
					await client.CancelPurchaseAsync(new LatitudePayCancelPurchaseRequest() { PaymentPlanToken = purchaseResponse.Token });
					Assert.Fail("ArgumentException expected");
				}
				catch (ArgumentException ane)
				{
					Assert.AreEqual("request." + nameof(request.Products), ane.ParamName);
				}

			}
		}

		[TestMethod]
		public async Task CreatePosPurchaseRequest_Products_RequiresName()
		{
			using (var client = TestUtils.GetSandboxClient())
			{
				var request = CreateMinimalPurchaseRequest();

				var expectedArgumentName = "request.Products[0].Name";
				// Test for null
				try
				{
					request.Products = new LatitudePayProduct[] { new LatitudePayProduct() { Name = null, Price = new LatitudePayMoney(1, "NZD"), Quantity = 1, Sku = "Test" } };
					var purchaseResponse = await client.CreatePosPurchaseAsync(request);
					await client.CancelPurchaseAsync(new LatitudePayCancelPurchaseRequest() { PaymentPlanToken = purchaseResponse.Token });
					Assert.Fail("ArgumentNullException expected");
				}
				catch (ArgumentNullException ane)
				{
					Assert.AreEqual(expectedArgumentName, ane.ParamName);
				}

				//Test for empty
				try
				{
					request.Products = new LatitudePayProduct[] { new LatitudePayProduct() { Name = String.Empty, Price = new LatitudePayMoney(1, "NZD"), Quantity = 1, Sku = "Test" } };
					var purchaseResponse = await client.CreatePosPurchaseAsync(request);
					await client.CancelPurchaseAsync(new LatitudePayCancelPurchaseRequest() { PaymentPlanToken = purchaseResponse.Token });
					Assert.Fail("ArgumentException expected");
				}
				catch (ArgumentException ane)
				{
					Assert.AreEqual(expectedArgumentName, ane.ParamName);
				}

				// Test for only null entries
				try
				{
					request.Products = new LatitudePayProduct[] { new LatitudePayProduct() { Name = "   \t", Price = new LatitudePayMoney(1, "NZD"), Quantity = 1, Sku = "Test" } };
					var purchaseResponse = await client.CreatePosPurchaseAsync(request);
					await client.CancelPurchaseAsync(new LatitudePayCancelPurchaseRequest() { PaymentPlanToken = purchaseResponse.Token });
					Assert.Fail("ArgumentException expected");
				}
				catch (ArgumentException ane)
				{
					Assert.AreEqual(expectedArgumentName, ane.ParamName);
				}
			}
		}

		[TestMethod]
		public async Task CreatePosPurchaseRequest_Products_RequiresNonNegativePrice()
		{
			using (var client = TestUtils.GetSandboxClient())
			{
				var request = CreateMinimalPurchaseRequest();

				Yort.LatitudePay.InStore.LatitudePayCreatePosPurchaseResponse purchaseResponse;
				var expectedArgumentName = "request.Products[0].Price";
				// Test for negative
				try
				{
					request.Products = new LatitudePayProduct[] { new LatitudePayProduct() { Name = "Test", Price = new LatitudePayMoney(-1, "NZD"), Quantity = 1, Sku = "Test" } };
					purchaseResponse = await client.CreatePosPurchaseAsync(request);
					await client.CancelPurchaseAsync(new LatitudePayCancelPurchaseRequest() { PaymentPlanToken = purchaseResponse.Token });
					Assert.Fail("ArgumentOutOfRangeException expected");
				}
				catch (ArgumentOutOfRangeException ane)
				{
					Assert.AreEqual(expectedArgumentName, ane.ParamName);
				}

				//Test for zero price
				request.Products = new LatitudePayProduct[] { new LatitudePayProduct() { Name = "test", Price = new LatitudePayMoney(0, "NZD"), Quantity = 1, Sku = "Test" } };
				purchaseResponse = await client.CreatePosPurchaseAsync(request);
				await client.CancelPurchaseAsync(new LatitudePayCancelPurchaseRequest() { PaymentPlanToken = purchaseResponse.Token });
			}
		}

		[TestMethod]
		public async Task CreatePosPurchaseRequest_Products_RequiresPositiveQuantity()
		{
			using (var client = TestUtils.GetSandboxClient())
			{
				var request = CreateMinimalPurchaseRequest();

				var expectedArgumentName = "request.Products[0].Quantity";
				// Test for negative
				try
				{
					request.Products = new LatitudePayProduct[] { new LatitudePayProduct() { Name = "Test", Price = new LatitudePayMoney(1, "NZD"), Quantity = -1, Sku = "Test" } };
					var purchaseResponse = await client.CreatePosPurchaseAsync(request);
					await client.CancelPurchaseAsync(new LatitudePayCancelPurchaseRequest() { PaymentPlanToken = purchaseResponse.Token });
					Assert.Fail("ArgumentOutOfRangeException expected");
				}
				catch (ArgumentOutOfRangeException ane)
				{
					Assert.AreEqual(expectedArgumentName, ane.ParamName);
				}

				//Test for zero quantity
				try
				{
					request.Products = new LatitudePayProduct[] { new LatitudePayProduct() { Name = "Test", Price = new LatitudePayMoney(1, "NZD"), Quantity = 0, Sku = "Test" } };
					var purchaseResponse = await client.CreatePosPurchaseAsync(request);
					await client.CancelPurchaseAsync(new LatitudePayCancelPurchaseRequest() { PaymentPlanToken = purchaseResponse.Token });
					Assert.Fail("ArgumentOutOfRangeException expected");
				}
				catch (ArgumentOutOfRangeException ane)
				{
					Assert.AreEqual(expectedArgumentName, ane.ParamName);
				}
			}
		}

		#endregion

		#region CreateRefundRequest_RequiresReference

		[TestMethod]
		public async Task CreateRefundRequest_RequiresReference()
		{
			using (var client = TestUtils.GetSandboxClient())
			{
				var refundRef = System.Guid.NewGuid().ToString();
				var request = new LatitudePayCreateRefundRequest()
				{
					Amount = new LatitudePayMoney(10M, LatitudePayCurrencies.NewZealandDollars),
					IdempotencyKey = refundRef,
					Reference = refundRef,
					PaymentPlanToken = System.Guid.NewGuid().ToString()
				};
				request.Reference = null;

				// Test for null
				try
				{
					var refundResponse = await client.CreateRefundAsync(request);
					Assert.Fail("ArgumentNullException expected");
				}
				catch (ArgumentNullException ane)
				{
					Assert.AreEqual("request." + nameof(request.Reference), ane.ParamName);
				}

				//Test for empty
				try
				{
					request.Reference = String.Empty;
					var refundResponse = await client.CreateRefundAsync(request);
					Assert.Fail("ArgumentException expected");
				}
				catch (ArgumentException ane)
				{
					Assert.AreEqual("request." + nameof(request.Reference), ane.ParamName);
				}

				// Test for whitespace only
				try
				{
					request.Reference = "  \t";
					var refundResponse = await client.CreateRefundAsync(request);
					Assert.Fail("ArgumentException expected");
				}
				catch (ArgumentException ane)
				{
					Assert.AreEqual("request." + nameof(request.Reference), ane.ParamName);
				}

			}
		}

		[TestMethod]
		public async Task CreateRefundRequest_RequiresPaymentPlanToken()
		{
			using (var client = TestUtils.GetSandboxClient())
			{
				var refundRef = System.Guid.NewGuid().ToString();
				var request = new LatitudePayCreateRefundRequest()
				{
					Amount = new LatitudePayMoney(10M, LatitudePayCurrencies.NewZealandDollars),
					IdempotencyKey = refundRef,
					Reference = refundRef,
					PaymentPlanToken = System.Guid.NewGuid().ToString()
				};
				request.PaymentPlanToken = null;

				// Test for null
				try
				{
					var refundResponse = await client.CreateRefundAsync(request);
					Assert.Fail("ArgumentNullException expected");
				}
				catch (ArgumentNullException ane)
				{
					Assert.AreEqual("request." + nameof(request.PaymentPlanToken), ane.ParamName);
				}

				//Test for empty
				try
				{
					request.PaymentPlanToken = String.Empty;
					var refundResponse = await client.CreateRefundAsync(request);
					Assert.Fail("ArgumentException expected");
				}
				catch (ArgumentException ane)
				{
					Assert.AreEqual("request." + nameof(request.PaymentPlanToken), ane.ParamName);
				}

				// Test for whitespace only
				try
				{
					request.PaymentPlanToken = "  \t";
					var refundResponse = await client.CreateRefundAsync(request);
					Assert.Fail("ArgumentException expected");
				}
				catch (ArgumentException ane)
				{
					Assert.AreEqual("request." + nameof(request.PaymentPlanToken), ane.ParamName);
				}

			}
		}

		[TestMethod]
		public async Task CreateRefundRequest_RequiresPositiveAmount()
		{
			using (var client = TestUtils.GetSandboxClient())
			{
				var refundRef = System.Guid.NewGuid().ToString();
				var request = new LatitudePayCreateRefundRequest()
				{
					Amount = new LatitudePayMoney(10M, LatitudePayCurrencies.NewZealandDollars),
					IdempotencyKey = refundRef,
					Reference = refundRef,
					PaymentPlanToken = System.Guid.NewGuid().ToString()
				};
				request.Amount = new LatitudePayMoney(0M, LatitudePayCurrencies.NewZealandDollars);

				// Test for zero
				try
				{
					var refundResponse = await client.CreateRefundAsync(request);
					Assert.Fail("ArgumentOutOfRangeException expected");
				}
				catch (ArgumentOutOfRangeException aore)
				{
					Assert.AreEqual("request." + nameof(request.Amount), aore.ParamName);
				}

				//Test for negative
				try
				{
					request.Amount = new LatitudePayMoney(-10M, LatitudePayCurrencies.NewZealandDollars);
					var refundResponse = await client.CreateRefundAsync(request);
					Assert.Fail("ArgumentOutOfRangeException expected");
				}
				catch (ArgumentOutOfRangeException aore)
				{
					Assert.AreEqual("request." + nameof(request.Amount), aore.ParamName);
				}
			}
		}

		#endregion

		#region PurchaseStatusRequest_RequiresPaymentPlanToken

		[TestMethod]
		public async Task PurchaseStatusRequest_RequiresPaymentPlanToken()
		{
			using (var client = TestUtils.GetSandboxClient())
			{
				var refundRef = System.Guid.NewGuid().ToString();
				var request = new Yort.LatitudePay.InStore.LatitudePayPurchaseStatusRequest();

				// Test for null
				try
				{
					request.PaymentPlanToken = null;
					var refundResponse = await client.GetPurchaseStatusAsync(request);
					Assert.Fail("ArgumentNullException expected");
				}
				catch (ArgumentNullException ane)
				{
					Assert.AreEqual("request." + nameof(request.PaymentPlanToken), ane.ParamName);
				}

				//Test for empty
				try
				{
					request.PaymentPlanToken = String.Empty;
					var refundResponse = await client.GetPurchaseStatusAsync(request);
					Assert.Fail("ArgumentException expected");
				}
				catch (ArgumentException ane)
				{
					Assert.AreEqual("request." + nameof(request.PaymentPlanToken), ane.ParamName);
				}

				// Test for whitespace only
				try
				{
					request.PaymentPlanToken = "  \t";
					var refundResponse = await client.GetPurchaseStatusAsync(request);
					Assert.Fail("ArgumentException expected");
				}
				catch (ArgumentException ane)
				{
					Assert.AreEqual("request." + nameof(request.PaymentPlanToken), ane.ParamName);
				}

			}
		}

		#endregion

		#region CancelPurchaseRequest_RequiresPaymentPlanToken

		[TestMethod]
		public async Task CancelPurchaseRequest_RequiresPaymentPlanToken()
		{
			using (var client = TestUtils.GetSandboxClient())
			{
				var refundRef = System.Guid.NewGuid().ToString();
				var request = new Yort.LatitudePay.InStore.LatitudePayCancelPurchaseRequest();

				// Test for null
				try
				{
					request.PaymentPlanToken = null;
					var refundResponse = await client.CancelPurchaseAsync(request);
					Assert.Fail("ArgumentNullException expected");
				}
				catch (ArgumentNullException ane)
				{
					Assert.AreEqual("request." + nameof(request.PaymentPlanToken), ane.ParamName);
				}

				//Test for empty
				try
				{
					request.PaymentPlanToken = String.Empty;
					var refundResponse = await client.CancelPurchaseAsync(request);
					Assert.Fail("ArgumentException expected");
				}
				catch (ArgumentException ane)
				{
					Assert.AreEqual("request." + nameof(request.PaymentPlanToken), ane.ParamName);
				}

				// Test for whitespace only
				try
				{
					request.PaymentPlanToken = "  \t";
					var refundResponse = await client.CancelPurchaseAsync(request);
					Assert.Fail("ArgumentException expected");
				}
				catch (ArgumentException ane)
				{
					Assert.AreEqual("request." + nameof(request.PaymentPlanToken), ane.ParamName);
				}

			}
		}

		#endregion


		private static LatitudePayCreatePosPurchaseRequest CreateMinimalPurchaseRequest()
		{
			var request = new LatitudePayCreatePosPurchaseRequest()
			{
				Reference = System.Guid.NewGuid().ToString(),
				BillingAddress = null,
				ShippingAddress = null,
				Customer = new LatitudePayCustomer()
				{
					MobileNumber = Environment.GetEnvironmentVariable("LatitudePay_TestMobileNumber")
				},
				Products = new LatitudePayProduct[] { new LatitudePayProduct() { Name = "Test", Price = new LatitudePayMoney(1, "NZD"), Quantity = 1, Sku = "Test" } },
				ShippingLines = null,
				TaxAmount = new LatitudePayMoney(0M, "NZD"),
				TotalAmount = new LatitudePayMoney(35.5M, "NZD"),
				ReturnUrls = null
			};
			request.IdempotencyKey = request.Reference;
			return request;
		}
	}
}
