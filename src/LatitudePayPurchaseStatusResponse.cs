using Newtonsoft.Json;

namespace Yort.LatitudePay.InStore
{
	/// <summary>
	/// Represents the status of a payment plan as returned by a <see cref="LatitudePayClient.GetPurchaseStatusAsync(LatitudePayPurchaseStatusRequest)"/> request.
	/// </summary>
	/// <seealso cref="LatitudePayPurchaseStatusRequest"/>
	/// <seealso cref="LatitudePayClient"/>
	/// <seealso cref="LatitudePayClient.GetPurchaseStatusAsync(LatitudePayPurchaseStatusRequest)"/>
	public class LatitudePayPurchaseStatusResponse
	{
		/// <summary>
		/// Gets or sets the payment token.
		/// </summary>
		/// <value>
		/// The token.
		/// </value>
		[JsonProperty("token")]
		public string? Token { get; set; }
		/// <summary>
		/// Gets or sets the status of the payment plan.
		/// </summary>
		/// <value>
		/// The status.
		/// </value>
		/// <seealso cref="LatitudePayConstants"/>
		[JsonProperty("status")]
		public string? Status { get; set; }
		/// <summary>
		/// Gets or sets a human readable message related to the status of this payment plan.
		/// </summary>
		/// <value>
		/// The message.
		/// </value>
		[JsonProperty("message")]
		public string? Message { get; set; }
	}
}
