using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Yort.LatitudePay.InStore
{
	/// <summary>
	/// Instances of this class represent configuration options for <see cref="ILatitudePayClient"/> instances. The static members of this class provide global configuration common to all instances.
	/// </summary>
	public sealed class LatitudePayClientConfiguration
	{

		#region Instance Members

		private HttpClient? _HttpClient;
		private LatitudePayEnvironment _Environment;

		private string? _ProductName;
		private string? _ProductVersion;
		private string? _ProductVendor;
		private string? _ApiKey;
		private string? _ApiSecret;
		private int _RetryDelay;

		private int _MinimumRetries;

		private bool _Locked;

		/// <summary>
		/// Default contructor, creates a new instance.
		/// </summary>
		/// <remarks>
		/// <para>Instances of this type become immmutable once passed to a <see cref="LatitudePayClient"/> instance. Trying to set properties after this has occurred will result in an <see cref="InvalidOperationException"/>.</para>
		/// </remarks>
		public LatitudePayClientConfiguration()
		{
			_MinimumRetries = 2;
		}

		/// <summary>
		/// Sets or returns the LatitudePay API environment to be used.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">Thrown if this property is modified after it has been passed to a <see cref="LatitudePayClient"/> instance.</exception>
		public LatitudePayEnvironment Environment
		{
			get { return _Environment; }
			set
			{
				ThrowIfLocked();

				_Environment = value;
			}
		}

		/// <summary>
		/// Sets or returns an <see cref="HttpClient"/> instance used to make calls to the LatitudePay API. If null/unset, the system will create it's own instance on first use.
		/// </summary>
		/// <remarks>
		/// <para>The library reserves the right to modify the provided client, such as setting default headers. A client can be shared amongst configuration object, but should not be shared/use outside of other <see cref="LatitudePayClient"/> instaces.
		/// The primary purpose of this method is to allow a client with injected handlers to be used. If you do not need to inject custom handlers, then leave this blank.</para>
		/// </remarks>
		/// <exception cref="System.InvalidOperationException">Thrown if this property is modified after it has been passed to a <see cref="LatitudePayClient"/> instance.</exception>
		public HttpClient? HttpClient
		{
			get { return _HttpClient; }
			set
			{
				ThrowIfLocked();

				_HttpClient = value;
			}
		}

		/// <summary>
		/// Sets or returns the product name that will be used as part of the user agent string when calling the LatitudePay API.
		/// </summary>
		/// <remarks>
		/// <para>If null, empty string or only whitespace the name of the Yort.LatitudePay.Instore assembly being used will be substituted as a default.</para>
		/// </remarks>
		/// <exception cref="System.InvalidOperationException">Thrown if this property is modified after it has been passed to a <see cref="LatitudePayClient"/> instance.</exception>
		public string? ProductName
		{
			get { return _ProductName; }
			set
			{
				ThrowIfLocked();

				_ProductName = value;
			}
		}

		/// <summary>
		/// Sets or returns the version number of the <see cref="ProductName"/> name that will be used as part of the user agent string when calling the LatitudePay API.
		/// </summary>
		/// <remarks>
		/// <para>If null, empty string or only whitespace the version of the Yort.LatitudePay.Instore assembly being used will be substituted as a default.</para>
		/// </remarks>
		/// <exception cref="System.InvalidOperationException">Thrown if this property is modified after it has been passed to a <see cref="LatitudePayClient"/> instance.</exception>
		public string? ProductVersion
		{
			get { return _ProductVersion; }
			set
			{
				ThrowIfLocked();

				_ProductVersion = value;
			}
		}

		/// <summary>
		/// Sets or returns the name of the vendor that will be used as part of the user agent string when calling the LatitudePay API.
		/// </summary>
		/// <remarks>
		/// <para>If null, empty string or only whitespace, "Yort" assembly being used will be substituted as a default.</para>
		/// </remarks>
		/// <exception cref="System.InvalidOperationException">Thrown if this property is modified after it has been passed to a <see cref="LatitudePayClient"/> instance.</exception>
		public string? ProductVendor
		{
			get { return _ProductVendor; }
			set
			{
				ThrowIfLocked();

				_ProductVendor = value;
			}
		}

		/// <summary>
		/// Sets or returns the API key that identifies the merchant account to use with LatitudePay.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">Thrown if this property is modified after it has been passed to a <see cref="LatitudePayClient"/> instance.</exception>
		public string? ApiKey
		{
			get { return _ApiKey; }
			set
			{
				ThrowIfLocked();

				_ApiKey = value;
			}
		}

		/// <summary>
		/// Sets or returns the secret value used to access the LatitudePay API.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">Thrown if this property is modified after it has been passed to a <see cref="LatitudePayClient"/> instance.</exception>
		public string? ApiSecret
		{
			get { return _ApiSecret; }
			set
			{
				ThrowIfLocked();

				_ApiSecret = value;
			}
		}

		/// <summary>
		/// The minumum number of automatic retries to perform when a create transaction (order/refund/order reversal/refund reversal etc) times out.
		/// </summary>
		/// <remarks>
		/// <para>This property defaults to a value of 2. A value of zero or less is allowed, in which case only the initial attempt will be made - no retries will be performed within the library and any error handling logic will need to be entirely implemented by the application.</para>
		/// <para>The library may attempt more retries than specified if the total time since the initial call is less than the (full, LatitudePay) recommended timeout for the endpoint being called.</para>
		/// </remarks>
		/// <exception cref="System.InvalidOperationException">Thrown if this property is modified after it has been passed to a <see cref="LatitudePayClient"/> instance.</exception>
		public int MinimumRetries
		{
			get { return _MinimumRetries; }
			set
			{
				ThrowIfLocked();

				_MinimumRetries = value;
			}
		}

		/// <summary>
		/// Sets or returns the number of seconds to wait before attempting a retry.
		/// </summary>
		/// <remarks>
		/// <para>When a transactional call (CreateOrder/Refund etc) times out the system will perform a retry (based on the <see cref="MinimumRetries"/> setting). 
		/// If that retry attempt returns a 409 conflict response indicating the first request is still in progres, 
		/// then the system will wait this many seconds before the next retry. See https://docs.LatitudePay.com.au/instore-api-v1.html#distributed-state-considerations and https://docs.LatitudePay.com.au/instore-api-v1.html#create-order for more details.</para>
		/// <para>The minimum value is 5 seconds. Any value less than 5 seconds will be ignored, and a 5 second delay will occur instead.</para>
		/// </remarks>
		/// <exception cref="System.InvalidOperationException">Thrown if this property is modified after it has been passed to a <see cref="LatitudePayClient"/> instance.</exception>
		public int RetryDelaySeconds
		{
			get { return _RetryDelay; }
			set
			{
				ThrowIfLocked();

				_RetryDelay = value;
			}
		}

		private void ThrowIfLocked()
		{
			if (_Locked) throw new InvalidOperationException(ErrorMessages.ConfigurationPropertiesLocked);
		}

		internal void LockProperties()
		{
			_Locked = true;
		}

		#endregion

		#region Static Memebers

		private static ILatitudePaySystemClock? s_SystemClock;
		private static string? s_DefaultCurrency;

		/// <summary>
		/// Sets or returns an implementation of <see cref="ILatitudePaySystemClock"/> that will be used by the library to determine the current date and time.
		/// </summary>
		/// <remarks>
		/// <para>If not clock is explicitly set, or if the property is set to null, then <see cref="LatitudePaySystemClock.DefaultInstance"/> will be used (and returned as the current value of the property).</para>
		/// <para>This property can be used to provide a mocked clock for unit testing, or to provide a clock adjusted by a calculated offset via an NTP client etc. if the system clock cannot be relied upon for accuracy.</para>
		/// <para>This is a static property and the value set here affects all clients/objects from the Yort.LatitudePay.InStore API unless otherwise specified.</para>
		/// </remarks>
		public static ILatitudePaySystemClock SystemClock
		{
			get { return s_SystemClock ?? LatitudePaySystemClock.DefaultInstance; }
			set
			{
				s_SystemClock = value;
			}
		}

		/// <summary>
		/// Sets or returns the default currency for new <see cref="LatitudePayMoney"/> instances where the currency is not explicitly provided.
		/// </summary>
		/// <remarks>
		/// <para>If this property is null or an empty string then <see cref="LatitudePayCurrencies.AustralianDollars"/> will be used as a default.</para>
		/// </remarks>
		public static string? DefaultCurrency
		{
			get { return s_DefaultCurrency; }
			set
			{
				s_DefaultCurrency = value;
			}
		}

		#endregion

	}
}
