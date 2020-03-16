using Newtonsoft.Json;

namespace Yort.LatitudePay.InStore
{
	/// <summary>
	/// Represents a shipping charge as part of a <see cref="LatitudePayCreatePosPurchaseRequest"/>.
	/// </summary>
	/// <seealso cref="LatitudePayCreatePosPurchaseRequest"/>
	/// <seealso cref="LatitudePayClient.CreatePosPurchaseAsync(LatitudePayCreatePosPurchaseRequest)"/>
	public class LatitiudePayShippingLine
	{
		/// <summary>
		/// Gets or sets the name of carrier providing shipping services.
		/// </summary>
		/// <value>
		/// The carrier name.
		/// </value>
		[JsonProperty("carrier")]
		public string? Carrier { get; set; }
		/// <summary>
		/// Gets or sets the price charged to the customer for this shipping service on this payment plan.
		/// </summary>
		/// <value>
		/// The shipping price.
		/// </value>
		[JsonProperty("price")]
		public LatitudePayMoney Price { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether tax is included in the <see cref="Price"/> property.
		/// </summary>
		/// <value>
		///   <c>true</c> if <see cref="Price"/> is tax inclusive, else <c>false</c>.
		/// </value>
		[JsonProperty("taxIncluded")]
		public bool TaxIncluded { get; set; }
	}
}
