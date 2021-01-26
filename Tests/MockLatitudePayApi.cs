using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Linq;
using Yort.LatitudePay.InStore;
using System.Threading.Tasks;
using System.Threading;

namespace Yort.LatitudePay.InStore.Tests
{
	//This is a really weakly behaved mock, it should be good enough to test the client logic for the current but doesn't guarantee 
	//exactly the same error messages or handling all possible error responses. It also doesn't really check auth tokens or request signatures.
	internal static class MockLatitudePayApi
	{
		private readonly static List<MockLatitudePayPayment> _Payments = new List<MockLatitudePayPayment>();
		private readonly static List<MockLatitudePayRefund> _Refunds = new List<MockLatitudePayRefund>();

		internal static HttpResponseMessage CreatePurchase(LatitudePayCreatePosPurchaseRequest request)
		{
			if (!String.IsNullOrWhiteSpace(request.IdempotencyKey))
			{
				var existingRequest = (from p in _Payments where request.IdempotencyKey == p.InitialRequest.IdempotencyKey select p).FirstOrDefault();
				if (existingRequest != null)
					return ToJsonHttpResponse(existingRequest.InitialResponseStatus, existingRequest.InitialResponse);
			}

			var token = System.Guid.NewGuid().ToString();
			var responseContent = new LatitudePayCreatePosPurchaseResponse()
			{
				CommissionAmount = 0M,
				ExpiryDate = DateTimeOffset.Now.AddMinutes(10),
				Reference = "GPV-" + System.Guid.NewGuid().ToString(),
				Token = token,
				StatusUrl = new Uri($"http://api.genoapay.com/v3/sale/pos/{token}/status")
			};

			var mockPayment = new MockLatitudePayPayment()
			{
				InitialResponseStatus = HttpStatusCode.OK,
				InitialRequest = request,
				InitiallyReceivedAt = DateTimeOffset.Now,
				InitialResponse = responseContent,
				CurrentPaymentStatus = LatitudePayConstants.StatusPending,
				IsCancelled = false
			};
			_Payments.Add(mockPayment);

			return ToJsonHttpResponse(mockPayment.InitialResponseStatus, mockPayment.InitialResponse);
		}

		internal static HttpResponseMessage GetStatus(LatitudePayPurchaseStatusRequest request)
		{
			var existingPayment = (from p in _Payments where request.PaymentPlanToken == p.InitialResponse.Token select p).FirstOrDefault();
			if (existingPayment == null)
				return ToJsonHttpResponse(HttpStatusCode.NotFound, new { error = "Payment not found" });

			if (existingPayment.IsCancelled || existingPayment.CurrentPaymentStatus != LatitudePayConstants.StatusPending || DateTimeOffset.Now.Subtract(existingPayment.InitiallyReceivedAt) < TimeSpan.FromSeconds(5))
			{
				//Just return current status
			}
			else if (existingPayment.InitialRequest.TotalAmount.Amount.ToString("#0.00").EndsWith("88"))
			{
				existingPayment.CurrentPaymentStatus = LatitudePayConstants.StatusDeclined;
				existingPayment.StatusMessage = "Declined: Credit denied";
			}
			else
			{
				existingPayment.CurrentPaymentStatus = LatitudePayConstants.StatusApproved;
				existingPayment.StatusMessage = "Approved";
			}

			return ToJsonHttpResponse(existingPayment.InitialResponseStatus, new LatitudePayPurchaseStatusResponse() { Message = existingPayment.StatusMessage, Status = existingPayment.CurrentPaymentStatus, Token = existingPayment.InitialResponse.Token });
		}

		internal static HttpResponseMessage CancelPurchase(LatitudePayCancelPurchaseRequest request)
		{
			var existingPayment = (from p in _Payments where request.PaymentPlanToken == p.InitialResponse.Token select p).FirstOrDefault();
			if (existingPayment == null)
				return ToJsonHttpResponse(HttpStatusCode.NotFound, new { error = "Payment not found" });

			if (existingPayment.IsCancelled || existingPayment.CurrentPaymentStatus == LatitudePayConstants.StatusDeclined)
				return ToJsonHttpResponse(HttpStatusCode.NotFound, new { error = "Payment is already declined" });

			existingPayment.IsCancelled = true;
			existingPayment.CurrentPaymentStatus = LatitudePayConstants.StatusDeclined;
			existingPayment.StatusMessage = "Cancelled by merchant";
			existingPayment.CancelledDate = DateTimeOffset.Now;

			return ToJsonHttpResponse(existingPayment.InitialResponseStatus, new LatitudePayCancelPurchaseResponse() { CancelledDate = existingPayment.CancelledDate.Date, Token = existingPayment.InitialResponse.Token });
		}

		internal static HttpResponseMessage CreateRefund(LatitudePayCreateRefundRequest request)
		{
			if (!String.IsNullOrWhiteSpace(request.IdempotencyKey))
			{
				var existingRequest = (from r in _Refunds where request.IdempotencyKey == r.InitialRequest.IdempotencyKey select r).FirstOrDefault();
				if (existingRequest != null)
					return ToJsonHttpResponse(existingRequest.InitialResponseStatus, existingRequest.InitialResponse);
			}

			var existingPayment = (from p in _Payments where request.IdempotencyKey == p.InitialRequest.IdempotencyKey select p).FirstOrDefault();
			if (existingPayment == null)
				return ToJsonHttpResponse(HttpStatusCode.NotFound, new { error = "Payment plan not found." });

			if (request.Amount.Amount > existingPayment.InitialRequest.TotalAmount.Amount)
				return ToJsonHttpResponse(HttpStatusCode.NotFound, new { error = "Refund is more than payment amount." });

			var token = System.Guid.NewGuid().ToString();
			var responseContent = new LatitudePayCreateRefundResponse()
			{
				CommissionAmount = 0M,
				RefundDate = DateTimeOffset.Now.DateTime,
				RefundId = System.Guid.NewGuid().ToString(),
				Reference = request.Reference
			};

			var mockRefund = new MockLatitudePayRefund()
			{
				InitialResponseStatus = HttpStatusCode.OK,
				InitialRequest = request,
				InitiallyReceivedAt = DateTimeOffset.Now,
				InitialResponse = responseContent,
			};
			_Refunds.Add(mockRefund);

			return ToJsonHttpResponse(mockRefund.InitialResponseStatus, mockRefund.InitialResponse);
		}

		private static HttpResponseMessage ToJsonHttpResponse<T>(HttpStatusCode statusCode, T bodyContent)
		{
			return new HttpResponseMessage(statusCode)
			{
				Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(bodyContent), System.Text.UTF8Encoding.UTF8, LatitudePayConstants.LatitudePayV3ContentType)
			};
		}
	}

	internal class MockLatitudePayPayment
	{
		public LatitudePayCreatePosPurchaseRequest InitialRequest { get; set; }
		public LatitudePayCreatePosPurchaseResponse InitialResponse { get; set; }
		public HttpStatusCode InitialResponseStatus { get; set; }
		public bool IsCancelled { get; set; }
		public string StatusMessage { get; set; }
		public DateTimeOffset CancelledDate { get; set; }

		public DateTimeOffset InitiallyReceivedAt { get; set; }
		public string CurrentPaymentStatus { get; set; }
	}

	internal class MockLatitudePayRefund
	{
		public LatitudePayCreateRefundRequest InitialRequest { get; set; }
		public LatitudePayCreateRefundResponse InitialResponse { get; set; }
		public HttpStatusCode InitialResponseStatus { get; set; }
		public DateTimeOffset InitiallyReceivedAt { get; set; }
	}

	internal class MockLatitudePayHttpHandler : DelegatingHandler
	{
		public MockLatitudePayHttpHandler()
		{
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			if (request.RequestUri.Authority != "api.genoapay.com" && request.RequestUri.Authority != "api.uat.latitudepay.com" && request.RequestUri.Authority != "api.latitudepay.com")
				return new HttpResponseMessage(HttpStatusCode.NotFound);

			string contentString = null;
			if (request.Content != null)
				contentString = await request.Content.ReadAsStringAsync();

			if (request.RequestUri.PathAndQuery == "/v3/token" && request.Method == HttpMethod.Post)
			{
				return new HttpResponseMessage(HttpStatusCode.OK)
				{
					Content = new StringContent
					(
						Newtonsoft.Json.JsonConvert.SerializeObject(new LatitudePayAuthToken() { ExpiryDate = DateTime.Now.AddDays(1), Token = System.Guid.NewGuid().ToString() }),
						System.Text.UTF8Encoding.UTF8,
						LatitudePayConstants.LatitudePayV3ContentType
					)
				};
			}
			else if (request.RequestUri.AbsolutePath == "/v3/sale/pos" && request.Method == HttpMethod.Post)
			{
				var rb = Newtonsoft.Json.JsonConvert.DeserializeObject<LatitudePayCreatePosPurchaseRequest>(contentString);
				return MockLatitudePayApi.CreatePurchase(rb);
			}
			else if (request.RequestUri.AbsolutePath.StartsWith("/v3/sale/pos/") && request.RequestUri.PathAndQuery.EndsWith("/status") && request.Method == HttpMethod.Get)
			{
				var token =  request.RequestUri.Segments[4].Trim('/');
				return MockLatitudePayApi.GetStatus(new LatitudePayPurchaseStatusRequest() { PaymentPlanToken = token } );
			}
			else if (request.RequestUri.AbsolutePath.StartsWith("/v3/sale/") && request.RequestUri.PathAndQuery.EndsWith("/cancel") && request.Method == HttpMethod.Put)
			{
				var token = request.RequestUri.Segments[3].Trim('/');
				return MockLatitudePayApi.CancelPurchase(new LatitudePayCancelPurchaseRequest() { PaymentPlanToken = token });
			}
			else if (request.RequestUri.AbsolutePath.StartsWith("/v3/sale/") && request.RequestUri.AbsolutePath.EndsWith("/refund") && request.Method == HttpMethod.Post)
			{
				var rb = Newtonsoft.Json.JsonConvert.DeserializeObject<LatitudePayCreateRefundRequest>(contentString);
				return MockLatitudePayApi.CreateRefund(rb);
			}

			return new HttpResponseMessage(HttpStatusCode.NotFound);
		}
	}
}
