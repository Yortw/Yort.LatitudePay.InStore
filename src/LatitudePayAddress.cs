using Newtonsoft.Json;

namespace Yort.LatitudePay.InStore
{
	/// <summary>
	/// Represents a physical/postal address used for delivery/shipping or billing.
	/// </summary>
	public class LatitudePayAddress
	{
		/// <summary>
		/// Gets or sets the first address line.
		/// </summary>
		/// <value>
		/// The first address line.
		/// </value>
		[JsonProperty("addressLine1")]
		public string? AddressLine1 { get; set; }
		/// <summary>
		/// Gets or sets the second address line.
		/// </summary>
		/// <value>
		/// The second address line.
		/// </value>
		[JsonProperty("addressLine2")]
		public string? AddressLine2 { get; set; }
		/// <summary>
		/// Gets or sets the suburb.
		/// </summary>
		/// <value>
		/// The suburb.
		/// </value>
		[JsonProperty("suburb")]
		public string? Suburb { get; set; }
		/// <summary>
		/// Gets or sets the city or town.
		/// </summary>
		/// <value>
		/// The city or town.
		/// </value>
		[JsonProperty("cityTown")]
		public string? CityTown { get; set; }
		/// <summary>
		/// Gets or sets the state or province.
		/// </summary>
		/// <value>
		/// The state or province.
		/// </value>
		[JsonProperty("state")]
		public string? State { get; set; }
		/// <summary>
		/// Gets or sets the postcode.
		/// </summary>
		/// <value>
		/// The postcode.
		/// </value>
		[JsonProperty("postcode")]
		public string? Postcode { get; set; }
		/// <summary>
		/// Gets or sets the country code.
		/// </summary>
		/// <value>
		/// The country code.
		/// </value>
		[JsonProperty("countryCode")]
		public string? CountryCode { get; set; }
	}
}
