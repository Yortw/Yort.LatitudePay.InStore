using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yort.LatitudePay.InStore;

namespace Tests
{
	[TestClass]
	public class IntegrationTests
	{
		[TestCategory("Integration")]
		[TestMethod]
		public async Task GetConfiguration()
		{
			var client = GetSandboxClient();

			var config = await client.GetConfigurationAsync(new LatitudePayConfigurationRequest() { TotalAmount = 100, DisplayInModal = false });
			Assert.IsNotNull(config);
			Assert.IsTrue(config.Availability.Any());
			Assert.IsFalse(String.IsNullOrEmpty(config.Description));
		}

		[TestCategory("Integration")]
		[TestMethod]
		public async Task Purchase()
		{
			var client = GetSandboxClient();

			var request = new LatitudePayCreatePosPurchaseRequest() 
			{ 
				Reference = System.Guid.NewGuid().ToString(),
				BillingAddress = new LatitudePayAddress()
				{
					AddressLine1 = "124 Fifth Avenue",
					Suburb = "Auckland",
					CityTown = "Auckland",
					State = "Auckland",
					Postcode = "1010",
					CountryCode = "NZ"
				},
				ShippingAddress = new LatitudePayAddress()
				{
					AddressLine1 = "124 Fifth Avenue",
					Suburb = "Auckland",
					CityTown = "Auckland",
					State = "Auckland",
					Postcode = "1010",
					CountryCode = "NZ"
				},
				Customer = new LatitudePayCustomer()
				{
					Address = new LatitudePayAddress()
					{
						AddressLine1 = "124 Fifth Avenue",
						Suburb = "Auckland",
						CityTown = "Auckland",
						State = "Auckland",
						Postcode = "1010",
						CountryCode = "NZ"
					},
					FirstName = "John",
					Surname = "Doe",
					MobileNumber = Environment.GetEnvironmentVariable("LatitudePay_TestMobileNumber")
				},
				Products = new List<LatitudePayProduct>()
				{
					new LatitudePayProduct()
					{
						Name = "Tennis Ball Multipack",
						Price = new LatitudePayMoney(30, "NZD"),
						Sku = "abc123",
						Quantity = 1,
						TaxIncluded = true
					}
				},
				ShippingLines = new List<LatitiudePayShippingLine>()
				{
					new LatitiudePayShippingLine()
					{
						Carrier = "NZ Post",
						Price = new LatitudePayMoney(5.5M, "NZD")
					}
				},
				TaxAmount = new LatitudePayMoney(5.325M, "NZD"),
				TotalAmount = new LatitudePayMoney(35.5M, "NZD"),
				ReturnUrls = new LatitudePayReturnUrls()
				{
					SuccessUrl = new Uri("http://genoapay.com/success"),
					FailUrl = new Uri("http://genoapay.com/fail"),
					CallbackUrl = new Uri("http://genoapay.com/fail-safe-callback")
				}
			};
			request.IdempotencyKey = request.Reference;

			var purchaseResponse = await client.CreatePosPurchaseAsync(request);
			Assert.IsNotNull(purchaseResponse);
			Assert.IsFalse(String.IsNullOrWhiteSpace(purchaseResponse.Token));
			Assert.IsNotNull(purchaseResponse.StatusUrl);

			//Wait until payment enters final status
			var statusRequest = new LatitudePayPurchaseStatusRequest() { PaymentPlanToken = purchaseResponse.Token };
			var finalStatus = false;
			LatitudePayPurchaseStatusResponse paymentStatus = null;
			while (!finalStatus)
			{
				await Task.Delay(5000).ConfigureAwait(false);
				paymentStatus = await client.GetPurchaseStatusAsync(statusRequest).ConfigureAwait(false);

				finalStatus = !String.Equals(paymentStatus.Status, LatitudePayConstants.StatusPending, StringComparison.OrdinalIgnoreCase);
			}

			Assert.AreEqual(LatitudePayConstants.StatusApproved, paymentStatus.Status);
			Assert.IsFalse(String.IsNullOrEmpty(paymentStatus.Token));
			Assert.IsFalse(String.IsNullOrEmpty(paymentStatus.Message));
		}

		[TestCategory("Integration")]
		[TestMethod]
		public async Task Cancel()
		{
			var client = GetSandboxClient();

			var request = new LatitudePayCreatePosPurchaseRequest()
			{
				Reference = "TBC-" + System.Guid.NewGuid().ToString(),
				BillingAddress = new LatitudePayAddress()
				{
					AddressLine1 = "124 Fifth Avenue",
					Suburb = "Auckland",
					CityTown = "Auckland",
					State = "Auckland",
					Postcode = "1010",
					CountryCode = "NZ"
				},
				ShippingAddress = new LatitudePayAddress()
				{
					AddressLine1 = "124 Fifth Avenue",
					Suburb = "Auckland",
					CityTown = "Auckland",
					State = "Auckland",
					Postcode = "1010",
					CountryCode = "NZ"
				},
				Customer = new LatitudePayCustomer()
				{
					Address = new LatitudePayAddress()
					{
						AddressLine1 = "124 Fifth Avenue",
						Suburb = "Auckland",
						CityTown = "Auckland",
						State = "Auckland",
						Postcode = "1010",
						CountryCode = "NZ"
					},
					FirstName = "John",
					Surname = "Doe",
					MobileNumber = Environment.GetEnvironmentVariable("LatitudePay_TestMobileNumber")
				},
				Products = new List<LatitudePayProduct>()
				{
					new LatitudePayProduct()
					{
						Name = "Tennis Ball Multipack",
						Price = new LatitudePayMoney(30, "NZD"),
						Sku = "abc123",
						Quantity = 1,
						TaxIncluded = true
					}
				},
				ShippingLines = new List<LatitiudePayShippingLine>()
				{
					new LatitiudePayShippingLine()
					{
						Carrier = "NZ Post",
						Price = new LatitudePayMoney(5.5M, "NZD")
					}
				},
				TaxAmount = new LatitudePayMoney(5.325M, "NZD"),
				TotalAmount = new LatitudePayMoney(35.5M, "NZD"),
				ReturnUrls = new LatitudePayReturnUrls()
				{
					SuccessUrl = new Uri("http://genoapay.com/success"),
					FailUrl = new Uri("http://genoapay.com/fail"),
					CallbackUrl = new Uri("http://genoapay.com/fail-safe-callback")
				}
			};

			var purchaseResponse = await client.CreatePosPurchaseAsync(request);
			Assert.IsNotNull(purchaseResponse);
			Assert.IsFalse(String.IsNullOrWhiteSpace(purchaseResponse.Token));
			Assert.IsNotNull(purchaseResponse.StatusUrl);

			//Wait until payment enters final status
			var cancelRequest = new LatitudePayCancelPurchaseRequest() { PaymentPlanToken = purchaseResponse.Token };

			await Task.Delay(5000).ConfigureAwait(false);
			var cancelResponse = await client.CancelPurchaseAsync(cancelRequest).ConfigureAwait(false);

			Assert.IsNotNull(cancelResponse);
			Assert.IsFalse(String.IsNullOrWhiteSpace(cancelResponse.Token));

			var statusRequest = new LatitudePayPurchaseStatusRequest() { PaymentPlanToken = purchaseResponse.Token };
			var paymentStatus = await client.GetPurchaseStatusAsync(statusRequest).ConfigureAwait(false);
			Assert.IsNotNull(paymentStatus);
			Assert.AreEqual(LatitudePayConstants.StatusDeclined, paymentStatus.Status);
		}

		[TestCategory("Integration")]
		[TestMethod]
		public async Task Refund()
		{
			var client = GetSandboxClient();

			var request = new LatitudePayCreatePosPurchaseRequest()
			{
				Reference = "TBR-" + System.Guid.NewGuid().ToString(),
				BillingAddress = new LatitudePayAddress()
				{
					AddressLine1 = "124 Fifth Avenue",
					Suburb = "Auckland",
					CityTown = "Auckland",
					State = "Auckland",
					Postcode = "1010",
					CountryCode = "NZ"
				},
				ShippingAddress = new LatitudePayAddress()
				{
					AddressLine1 = "124 Fifth Avenue",
					Suburb = "Auckland",
					CityTown = "Auckland",
					State = "Auckland",
					Postcode = "1010",
					CountryCode = "NZ"
				},
				Customer = new LatitudePayCustomer()
				{
					Address = new LatitudePayAddress()
					{
						AddressLine1 = "124 Fifth Avenue",
						Suburb = "Auckland",
						CityTown = "Auckland",
						State = "Auckland",
						Postcode = "1010",
						CountryCode = "NZ"
					},
					FirstName = "John",
					Surname = "Doe",
					MobileNumber = Environment.GetEnvironmentVariable("LatitudePay_TestMobileNumber")
				},
				Products = new List<LatitudePayProduct>()
				{
					new LatitudePayProduct()
					{
						Name = "Tennis Ball Multipack",
						Price = new LatitudePayMoney(30, "NZD"),
						Sku = "abc123",
						Quantity = 1,
						TaxIncluded = true
					}
				},
				ShippingLines = new List<LatitiudePayShippingLine>()
				{
					new LatitiudePayShippingLine()
					{
						Carrier = "NZ Post",
						Price = new LatitudePayMoney(5.5M, "NZD")
					}
				},
				TaxAmount = new LatitudePayMoney(5.325M, "NZD"),
				TotalAmount = new LatitudePayMoney(35.5M, "NZD"),
				ReturnUrls = new LatitudePayReturnUrls()
				{
					SuccessUrl = new Uri("http://genoapay.com/success"),
					FailUrl = new Uri("http://genoapay.com/fail"),
					CallbackUrl = new Uri("http://genoapay.com/fail-safe-callback")
				}
			};

			var purchaseResponse = await client.CreatePosPurchaseAsync(request);
			Assert.IsNotNull(purchaseResponse);
			Assert.IsFalse(String.IsNullOrWhiteSpace(purchaseResponse.Token));
			Assert.IsNotNull(purchaseResponse.StatusUrl);

			//Wait until payment enters final status
			var statusRequest = new LatitudePayPurchaseStatusRequest() { PaymentPlanToken = purchaseResponse.Token };
			var finalStatus = false;
			LatitudePayPurchaseStatusResponse paymentStatus;
			while (!finalStatus)
			{
				await Task.Delay(5000).ConfigureAwait(false);
				paymentStatus = await client.GetPurchaseStatusAsync(statusRequest).ConfigureAwait(false);

				finalStatus = !String.Equals(paymentStatus.Status, LatitudePayConstants.StatusPending, StringComparison.OrdinalIgnoreCase);
			}

			var refundRequest = new LatitudePayCreateRefundRequest() { PaymentPlanToken = purchaseResponse.Token, Amount = request.TotalAmount, Reason = "Test refund", Reference = System.Guid.NewGuid().ToString() };
			var refundResponse = await client.CreateRefundAsync(refundRequest);
			Assert.IsNotNull(refundResponse);
			Assert.IsFalse(String.IsNullOrEmpty(refundResponse.RefundId));
			Assert.IsFalse(String.IsNullOrEmpty(refundResponse.Reference));
		}

		private ILatitudePayClient GetSandboxClient()
		{
			var config = new LatitudePayClientConfiguration()
			{
				ApiKey = Environment.GetEnvironmentVariable("LatitudePay_ApiKey"),
				ApiSecret = Environment.GetEnvironmentVariable("LatitudePay_ApiSecret"),
				Environment = LatitudePayEnvironment.Uat				
			};
			return new LatitudePayClient(config);
		}
	}
}
