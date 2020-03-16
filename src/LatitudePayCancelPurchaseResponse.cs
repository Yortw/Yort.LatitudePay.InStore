using System;
using Newtonsoft.Json;

namespace Yort.LatitudePay.InStore
{
	/// <summary>
	/// Represents the response to a request to cancel a pending payment plan (via <see cref="LatitudePayClient.CancelPurchaseAsync(LatitudePayCancelPurchaseRequest)"/>).
	/// </summary>
	/// <seealso cref="LatitudePayClient.CancelPurchaseAsync(LatitudePayCancelPurchaseRequest)"/>
	/// <seealso cref="LatitudePayClient"/>
	/// <seealso cref="LatitudePayCancelPurchaseResponse"/>
	/// <seealso cref="LatitudePayCreatePosPurchaseResponse.Token"/>
	public class LatitudePayCancelPurchaseResponse
	{
		/// <summary>
		/// Gets or sets the token of the payment plan to be cancelled.
		/// </summary>
		/// <value>
		/// The token.
		/// </value>
		[JsonProperty("token")]
		public string? Token { get; set; }
		/// <summary>
		/// Gets or sets the date and time at which the plan was cancelled.
		/// </summary>
		/// <value>
		/// The cancelled date.
		/// </value>
		[JsonProperty("cancelledDate")]
		public DateTime CancelledDate { get; set; }
	}
}
