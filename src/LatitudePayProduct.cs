using Newtonsoft.Json;

namespace Yort.LatitudePay.InStore
{
	/// <summary>
	/// Represents details of an item purchased via a <see cref="LatitudePayCreatePosPurchaseRequest"/>.
	/// </summary>
	/// <seealso cref="LatitudePayClient"/>
	/// <seealso cref="LatitudePayClient.CreatePosPurchaseAsync(LatitudePayCreatePosPurchaseRequest)"/>
	/// <seealso cref="LatitudePayCreatePosPurchaseRequest"/>
	public class LatitudePayProduct
	{
		/// <summary>
		/// Gets or sets the name/description of the product. Required.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[JsonProperty("name")]
		public string? Name { get; set; }
		/// <summary>
		/// Gets or sets the unit price of the product. Cannot be a negative value.
		/// </summary>
		/// <value>
		/// The price.
		/// </value>
		[JsonProperty("price")]
		public LatitudePayMoney Price { get; set; }
		/// <summary>
		/// Gets or sets the SKU of the item. Optional.
		/// </summary>
		/// <value>
		/// The sku.
		/// </value>
		[JsonProperty("sku")]
		public string? Sku { get; set; }
		/// <summary>
		/// Gets or sets the quantity of this item purchased. Must be a positive value greater than zero.
		/// </summary>
		/// <value>
		/// The quantity.
		/// </value>
		[JsonProperty("quantity")]
		public int Quantity { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether <see cref="Price"/> is inclusive of tax.
		/// </summary>
		/// <value>
		///   <c>true</c> if tax is included in <see cref="Price"/>; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("taxIncluded")]
		public bool TaxIncluded { get; set; }
	}
}
