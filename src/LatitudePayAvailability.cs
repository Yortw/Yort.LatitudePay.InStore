using Newtonsoft.Json;

namespace Yort.LatitudePay.InStore
{
	/// <summary>
	/// Represents details of an available country &amp; currency combination that can be used with the current LatitudePay account.
	/// </summary>
	/// <remarks>
	/// <para>Returns as part of <see cref="LatitudePayConfigurationResponse"/>.</para>
	/// </remarks>
	/// <seealso cref="LatitudePayConfigurationRequest"/>
	/// <seealso cref="ILatitudePayClient.GetConfigurationAsync(LatitudePayConfigurationRequest)"/>
	public class LatitudePayAvailability
	{
		/// <summary>
		/// Gets or sets the name of a country associated with this account.
		/// </summary>
		/// <value>
		/// The country name.
		/// </value>
		[JsonProperty("country")]
		public string? Country { get; set; }
		/// <summary>
		/// Gets or sets the ISO code of a country associated with this account.
		/// </summary>
		/// <value>
		/// The country code.
		/// </value>
		[JsonProperty("countryCode")]
		public string? CountryCode { get; set; }
		/// <summary>
		/// Gets or sets the ISO currency code associated with this account.
		/// </summary>
		/// <value>
		/// The currency.
		/// </value>
		[JsonProperty("currency")]
		public string? Currency { get; set; }
		/// <summary>
		/// Gets or sets the symbol (i.e $) used with the currency associated with this account.
		/// </summary>
		/// <value>
		/// The currency symbol.
		/// </value>
		[JsonProperty("currencySymbol")]
		public string? CurrencySymbol { get; set; }
	}
}
