using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Ladon;

namespace Yort.LatitudePay.InStore
{
	/// <summary>
	/// The primary class used to integrate to the LatitudePay API.
	/// </summary>
	/// <remarks>
	/// <para>Instances of this class should be reused between requests. If working in a single POS, create a copy on startup or first use and then re-use for subsequent payments. 
	/// Dispose only when you're done with the instance completely, such as on shutdown or if about to create a new instance with alternate configuration.</para>
	/// <para>If working in a payment service/gateway or web POS handling multiple lanes, then create a pool of instances (one for each lane) and re-use as appropriate.</para>
	/// <para>Instances of this class should be thread safe, multiple calls for the same or different payments through the same instance should work as expected.</para>
	/// </remarks>
	/// <seealso cref="Yort.LatitudePay.InStore.ILatitudePayClient" />
	public sealed class LatitudePayClient : ILatitudePayClient
	{

		#region Fields

		private readonly LatitudePayClientConfiguration _Configuration;
		private readonly HttpClient _HttpClient;
		private readonly Uri _BaseUrl;
		private readonly ILatitudePaySignatureGenerator _SignatureGenerator;
		private LatitudePayAuthToken? _Token;

		private bool _IsDisposed;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="LatitudePayClient"/> class.
		/// </summary>
		/// <param name="configuration">A <see cref="LatitudePayClientConfiguration"/> instance containing information required to connect to the LatitudePay API.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="configuration"/> is null, or if <see cref="LatitudePayClientConfiguration.ApiSecret"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <see cref="LatitudePayClientConfiguration.ApiSecret"/> is an empty string or contains only whitespace.</exception>
		/// <seealso cref="LatitudePayClientConfiguration"/>
		/// <seealso cref="ILatitudePayClient"/>
		public LatitudePayClient(LatitudePayClientConfiguration configuration)
		{
			_Configuration = configuration.GuardNull(nameof(configuration));
			configuration.LockProperties();

			_SignatureGenerator = new LatitudePayHMACSHA256SignatureGenerator
			(
				configuration.ApiSecret.GuardNullOrWhiteSpace
				(
					nameof(configuration),
					nameof(configuration.ApiSecret)
				)
			);

			//Not set on HttpClient as the client might be shared between environments (it shouldn't be, but trust no one).
			_BaseUrl = new Uri((_Configuration.Environment == LatitudePayEnvironment.Production ? LatitudePayConstants.ProductionRootUrl : LatitudePayConstants.UatRootUrl) + "/v3/");

			ConfigureServicePoint();

			_HttpClient = ConfigureHttpClient(configuration.HttpClient ?? CreateDefaultHttpClient());

		}

		#endregion

		#region ILatitudePayClient

		/// <summary>
		/// Gets configuration information for the current merchant account, specifying the supported currency, minimum and maximum purchase values etc.
		/// </summary>
		/// <param name="request">A <see cref="LatitudePayConfigurationRequest" /> that specifies options for the data returned.</param>
		/// <returns>
		/// A <see cref="LatitudePayConfigurationResponse" /> instance.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="request"/> is null.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if <see cref="Dispose"/> has been called on this instance.</exception>
		/// <exception cref="UnauthorizedAccessException">Thrown if the system is unable to obtain an authorisation token from the API (see the inner exception for details).</exception>
		/// <exception cref="System.Net.Http.HttpRequestException">Thrown if an error occurs making the request to the API.</exception>
		public async Task<LatitudePayConfigurationResponse> GetConfigurationAsync(LatitudePayConfigurationRequest request)
		{
			request.GuardNull(nameof(request));

			await EnsureAuthorisedAsync().ConfigureAwait(false);
			var requestUri = new Uri($"configuration?totalAmount={request.TotalAmount}&displayInModel={request.DisplayInModal}", UriKind.Relative);
			using (var response = await _HttpClient.GetAsync(requestUri).ConfigureAwait(false))
			{
				return await DeserialiseResponse<LatitudePayConfigurationResponse>(response).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Creates a new 'payment plan'.
		/// </summary>
		/// <param name="request">A <see cref="LatitudePayCreatePosPurchaseRequest" /> instance specifying options for the payment plan to be created.</param>
		/// <returns>
		/// A <see cref="LatitudePayCreatePosPurchaseResponse" /> instance containing details of the pending payment plan.
		/// </returns>
		/// <seealso cref="GetPurchaseStatusAsync(LatitudePayPurchaseStatusRequest)" />
		/// <seealso cref="CancelPurchaseAsync(LatitudePayCancelPurchaseRequest)" />
		/// <seealso cref="CreateRefundAsync(LatitudePayCreateRefundRequest)" />
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="request"/> or any of it's required sub-properties are null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if any sub-properties of <paramref name="request"/> are outside the allowed range, i.e negative price.</exception>
		/// <exception cref="ArgumentException">Thrown if any sub-properties of <paramref name="request"/> are invalid for a reason other than being null or outside of a valid range.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if <see cref="Dispose"/> has been called on this instance.</exception>
		/// <exception cref="UnauthorizedAccessException">Thrown if the system is unable to obtain an authorisation token from the API (see the inner exception for details).</exception>
		/// <exception cref="System.Net.Http.HttpRequestException">Thrown if an error occurs making the request to the API.</exception>
		public async Task<LatitudePayCreatePosPurchaseResponse> CreatePosPurchaseAsync(LatitudePayCreatePosPurchaseRequest request)
		{
			request.GuardNull(nameof(request));
			request.Validate(nameof(request));

			await EnsureAuthorisedAsync().ConfigureAwait(false);

			var jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(request);
			var signature = _SignatureGenerator.GenerateSignature(jsonBody);
			var requestUri = new Uri($"sale/pos?signature={signature}", UriKind.Relative);
			using (var requestContent = new StringContent(jsonBody, System.Text.UTF8Encoding.UTF8, LatitudePayConstants.LatitudePayV3ContentType))
			{
				using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri))
				{
					AddIdempotencyKeyHeader(requestMessage, request.IdempotencyKey);
					requestMessage.Content = requestContent;

					using (var response = await _HttpClient.SendAsync(requestMessage).ConfigureAwait(false))
					{
						return await DeserialiseResponse<LatitudePayCreatePosPurchaseResponse>(response).ConfigureAwait(false);
					}
				}
			}
		}

		/// <summary>
		/// Gets the status of a payment plan previously requested via <see cref="CreatePosPurchaseAsync(LatitudePayCreatePosPurchaseRequest)" />.
		/// </summary>
		/// <param name="request">A <see cref="LatitudePayPurchaseStatusRequest" /> instance containing the token of the payment plan who's status should be queried.</param>
		/// <returns>
		/// A <see cref="LatitudePayPurchaseStatusResponse" /> instance containing the status and other details of the plan.
		/// </returns>
		/// <seealso cref="CreatePosPurchaseAsync(LatitudePayCreatePosPurchaseRequest)" />
		/// <seealso cref="CancelPurchaseAsync(LatitudePayCancelPurchaseRequest)" />
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="request"/> or any of it's required sub-properties are null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if any sub-properties of <paramref name="request"/> are outside the allowed range, i.e negative price.</exception>
		/// <exception cref="ArgumentException">Thrown if any sub-properties of <paramref name="request"/> are invalid for a reason other than being null or outside of a valid range.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if <see cref="Dispose"/> has been called on this instance.</exception>
		/// <exception cref="UnauthorizedAccessException">Thrown if the system is unable to obtain an authorisation token from the API (see the inner exception for details).</exception>
		/// <exception cref="System.Net.Http.HttpRequestException">Thrown if an error occurs making the request to the API.</exception>
		public async Task<LatitudePayPurchaseStatusResponse> GetPurchaseStatusAsync(LatitudePayPurchaseStatusRequest request)
		{
			request.GuardNull(nameof(request));
			request.Validate(nameof(request));

			await EnsureAuthorisedAsync().ConfigureAwait(false);

			var requestUri = new Uri($"sale/pos/{request.PaymentPlanToken}/status", UriKind.Relative);
			using (var response = await _HttpClient.GetAsync(requestUri).ConfigureAwait(false))
			{
				return await DeserialiseResponse<LatitudePayPurchaseStatusResponse>(response).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Cancels a (pending) payment plan previously requested via <see cref="CreatePosPurchaseAsync(LatitudePayCreatePosPurchaseRequest)" />.
		/// </summary>
		/// <param name="request">A <see cref="LatitudePayCancelPurchaseRequest" /> containing the token of the payment plan to cancel.</param>
		/// <returns>
		/// An instance of a <see cref="LatitudePayCancelPurchaseResponse" /> indicating if the cancellation was successful.
		/// </returns>
		/// <remarks>
		/// This only cancels pending/unapproved payment plans. If a plan has been accepted/approved you must refund it instead of cancel it.
		/// </remarks>
		/// <seealso cref="CreatePosPurchaseAsync(LatitudePayCreatePosPurchaseRequest)" />
		/// <seealso cref="CreateRefundAsync(LatitudePayCreateRefundRequest)" />
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="request"/> or any of it's required sub-properties are null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if any sub-properties of <paramref name="request"/> are outside the allowed range, i.e negative price.</exception>
		/// <exception cref="ArgumentException">Thrown if any sub-properties of <paramref name="request"/> are invalid for a reason other than being null or outside of a valid range.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if <see cref="Dispose"/> has been called on this instance.</exception>
		/// <exception cref="UnauthorizedAccessException">Thrown if the system is unable to obtain an authorisation token from the API (see the inner exception for details).</exception>
		/// <exception cref="System.Net.Http.HttpRequestException">Thrown if an error occurs making the request to the API.</exception>
		public async Task<LatitudePayCancelPurchaseResponse> CancelPurchaseAsync(LatitudePayCancelPurchaseRequest request)
		{
			request.GuardNull(nameof(request));
			request.Validate(nameof(request));

			await EnsureAuthorisedAsync().ConfigureAwait(false);

			var requestUri = new Uri($"sale/{request.PaymentPlanToken}/cancel", UriKind.Relative);
			using (var requestContent = new StringContent(String.Empty, System.Text.UTF8Encoding.UTF8, LatitudePayConstants.LatitudePayV3ContentType))
			{
				using (var response = await _HttpClient.PutAsync(requestUri, requestContent).ConfigureAwait(false))
				{
					return await DeserialiseResponse<LatitudePayCancelPurchaseResponse>(response).ConfigureAwait(false);
				}
			}
		}

		/// <summary>
		/// Refunds a previously approved payment plan.
		/// </summary>
		/// <param name="request">A <see cref="LatitudePayCreateRefundRequest" /> instance containing details of the refund and payment plan to refund against.</param>
		/// <returns>
		/// A <see cref="LatitudePayCreateRefundResponse" /> instance indicating if the refund was successful and details of the refund created.
		/// </returns>
		/// <remarks>
		/// <para>You can only refund a previously accepted payment plan. Use <see cref="CancelPurchaseAsync(LatitudePayCancelPurchaseRequest)" /> to cancel a payment plan that is pending.</para>
		/// <para>A refund can be full or partial, and multiple partial refunds can be made. You can only refund up to the total amount of the original payment plan, across all refunds.</para>
		/// </remarks>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="request"/> or any of it's required sub-properties are null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if any sub-properties of <paramref name="request"/> are outside the allowed range, i.e negative price.</exception>
		/// <exception cref="ArgumentException">Thrown if any sub-properties of <paramref name="request"/> are invalid for a reason other than being null or outside of a valid range.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if <see cref="Dispose"/> has been called on this instance.</exception>
		/// <exception cref="UnauthorizedAccessException">Thrown if the system is unable to obtain an authorisation token from the API (see the inner exception for details).</exception>
		/// <exception cref="System.Net.Http.HttpRequestException">Thrown if an error occurs making the request to the API.</exception>
		public async Task<LatitudePayCreateRefundResponse> CreateRefundAsync(LatitudePayCreateRefundRequest request)
		{
			request.GuardNull(nameof(request));
			request.Validate(nameof(request));

			await EnsureAuthorisedAsync().ConfigureAwait(false);

			var jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(request);
			var signature = _SignatureGenerator.GenerateSignature(jsonBody);
			var requestUri = new Uri($"sale/{request.PaymentPlanToken}/refund?signature={signature}", UriKind.Relative);
			using (var requestContent = new StringContent(jsonBody, System.Text.UTF8Encoding.UTF8, LatitudePayConstants.LatitudePayV3ContentType))
			{
				using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri))
				{
					AddIdempotencyKeyHeader(requestMessage, request.IdempotencyKey);
					requestMessage.Content = requestContent;

					using (var response = await _HttpClient.SendAsync(requestMessage).ConfigureAwait(false))
					{
						return await DeserialiseResponse<LatitudePayCreateRefundResponse>(response).ConfigureAwait(false);
					}
				}
			}
		}

		#endregion

		#region IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (!_IsDisposed)
			{
				_IsDisposed = true;

				_SignatureGenerator?.Dispose();
				_HttpClient?.Dispose();
				_Token = null;
			}
		}

		#endregion

		#region Private Methods

		private async Task EnsureAuthorisedAsync()
		{
			if (_IsDisposed) throw new ObjectDisposedException(nameof(LatitudePayClient));

			try
			{
				if (_Token?.IsValid(LatitudePaySystemClock.DefaultInstance) ?? false) return;

				using (var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_HttpClient.BaseAddress, "token"))
				{
					Content = new StringContent(String.Empty, System.Text.UTF8Encoding.UTF8, LatitudePayConstants.LatitudePayV3ContentType)
				})
				{
					var creds = _Configuration.ApiKey + ":" + _Configuration.ApiSecret;
					creds = System.Convert.ToBase64String(System.Text.UTF8Encoding.UTF8.GetBytes(creds));

					request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", creds);
					using (var response = await _HttpClient.SendAsync(request).ConfigureAwait(false))
					{
						var token = await DeserialiseResponse<LatitudePayAuthToken>(response).ConfigureAwait(false);
						_HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Token);
						_Token = token;
					}
				}
			}
			catch (Exception ex)
			{
				throw new UnauthorizedAccessException(ErrorMessages.UnableToObtainToken, ex);
			}
		}

		private static async Task<T> DeserialiseResponse<T>(HttpResponseMessage response)
		{
			if (!response.IsSuccessStatusCode)
			{
				var errorDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<LatitudePayErrorResponse>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
				throw new LatitudePayApiException(String.IsNullOrWhiteSpace(errorDetails.ErrorMessage) ? response.StatusCode.ToString() : errorDetails.ErrorMessage ?? response.StatusCode.ToString(), Convert.ToInt32(response.StatusCode));
			}

			return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
		}

		private void ConfigureServicePoint()
		{
			try
			{
				var servicePoint = System.Net.ServicePointManager.FindServicePoint(_BaseUrl);
				servicePoint.Expect100Continue = false; //Improves performance by reducing round trips
				servicePoint.UseNagleAlgorithm = true; //Improves latency/performance
			}
			//Ignore any exceptions that might be thrown from poorly/partially implemented Net Standard 2.0.
			catch (PlatformNotSupportedException) { }
			catch (NotImplementedException) { }
			catch (NotSupportedException) { }

			try
			{
				//TLS 1.2 is required for AfterPay servers.
				//.Net 4.0 doesn't contain the TLS enum value, but converting the expected numeric value (3072)
				//to the enum type works, so long as either a later .Net version is installed, or the machine 
				//has had registry edits & patches to enable that protocol. Either way, we need to ensure TLS 1.2 is turned on
				//in System.Net.ServicePointManager.SecurityProtocol.
				var tls12 = (System.Net.SecurityProtocolType)3072;
				if ((System.Net.ServicePointManager.SecurityProtocol & tls12) != tls12)
				{
					System.Net.ServicePointManager.SecurityProtocol |= tls12;
				}
			}
			//Ignore any exceptions that might be thrown from poorly/partially implemented Net Standard 2.0.
			catch (PlatformNotSupportedException) { }
			catch (NotImplementedException) { }
			catch (NotSupportedException) { }
		}

		private static HttpClient CreateDefaultHttpClient()
		{
			HttpClientHandler? handler = null;
			try
			{
				handler = new HttpClientHandler();
				if (handler.SupportsAutomaticDecompression)
					handler.AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip;

				if (handler.SupportsRedirectConfiguration)
					handler.AllowAutoRedirect = true;

				handler.ClientCertificateOptions = ClientCertificateOption.Manual;
				handler.UseCookies = false;

				handler.UseDefaultCredentials = false;

				return new HttpClient(handler);
			}
			catch
			{
				handler?.Dispose();

				throw;
			}
		}

		private HttpClient ConfigureHttpClient(HttpClient client)
		{
			client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(LatitudePayConstants.LatitudePayV3ContentType));
			client.DefaultRequestHeaders.UserAgent.Clear();
			client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", GetUserAgent());
			client.BaseAddress = _BaseUrl;

			client.Timeout = TimeSpan.FromSeconds(30);

			return client;
		}

		private string GetUserAgent()
		{
			var assemblyName = System.Reflection.Assembly.GetAssembly(typeof(LatitudePayClient)).GetName();

			return String.Format(System.Globalization.CultureInfo.InvariantCulture, "{1}/{2} ({0})",
				String.IsNullOrWhiteSpace(_Configuration.ProductVendor) ? "Yort" : _Configuration.ProductVendor,
				String.IsNullOrWhiteSpace(_Configuration.ProductName) ? assemblyName.Name : _Configuration.ProductName,
				String.IsNullOrWhiteSpace(_Configuration.ProductVersion) ? assemblyName.Version.ToString() : _Configuration.ProductVersion
			);
		}

		private static void AddIdempotencyKeyHeader(HttpRequestMessage requestMessage, string? idempotencyKey)
		{
			if (String.IsNullOrWhiteSpace(idempotencyKey)) return;

			requestMessage.Headers.Add("X-Idempotency-Key", idempotencyKey);
		}

		#endregion

	}
}
