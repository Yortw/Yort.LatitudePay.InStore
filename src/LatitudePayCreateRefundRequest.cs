using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Yort.LatitudePay.InStore
{
	/// <summary>
	/// Represents a request to refund some or all of an accepted payment plan.
	/// </summary>
	/// <seealso cref="LatitudePayCreateRefundResponse"/>
	/// <seealso cref="LatitudePayClient.CreateRefundAsync(LatitudePayCreateRefundRequest)"/>
	public class LatitudePayCreateRefundRequest
	{
		/// <summary>
		/// Gets or sets the token of the payment plan to refund against.
		/// </summary>
		/// <value>
		/// The payment plan token.
		/// </value>
		[Required]
		[JsonIgnore]
		public string? PaymentPlanToken { get; set; }

		/// <summary>
		/// Gets or sets the merchants unique reference for this refund.
		/// </summary>
		/// <value>
		/// The reference.
		/// </value>
		[JsonProperty("reference")]
		public string? Reference { get; set; }

		/// <summary>
		/// Gets or sets a descriptive reason for the refund.
		/// </summary>
		/// <value>
		/// The reason.
		/// </value>		[JsonProperty("reason")]
		public string? Reason { get; set; }

		/// <summary>
		/// Gets or sets the amount of this refund.
		/// </summary>
		/// <value>
		/// The amount.
		/// </value>
		[Required]
		[JsonProperty("amount")]
		public LatitudePayMoney Amount { get; set; }
	}
}
