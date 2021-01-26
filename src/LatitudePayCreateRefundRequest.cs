using System.ComponentModel.DataAnnotations;
using Ladon;
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
		/// A value unique to this refund, but common to all CreateRefundRequest sent for this payment, to ensure the refund is created only once.
		/// </summary>
		/// <remarks>
		/// <para>This value is optional but highly recommended as without using it you cannot guarantee network problems and other issues won't lead to duplicate refunds.</para>
		/// <para>Set this property to a value that is unique for this *refund* but the same for each call to <see cref="ILatitudePayClient.CreateRefundAsync(LatitudePayCreateRefundRequest)"/> for that refund.
		/// Any repeat requests using the same idempotency key value will return the original response without creating a second refund. If the original request never got processed 
		/// (due to power failure, network outage etc) then the next request will be treated as the first one. This guarantees one and only one refund is created.</para>
		/// </remarks>
		[JsonIgnore]
		public string? IdempotencyKey { get; set; }

		/// <summary>
		/// Gets or sets the token of the payment plan to refund against. Required.
		/// </summary>
		/// <value>
		/// The payment plan token.
		/// </value>
		[Required]
		[JsonIgnore]
		public string? PaymentPlanToken { get; set; }

		/// <summary>
		/// Gets or sets the merchants unique reference for this refund. Required.
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
		/// </value>		
		[JsonProperty("reason")]
		public string? Reason { get; set; }

		/// <summary>
		/// Gets or sets the amount of this refund. Must be a positive non-zero value.
		/// </summary>
		/// <value>
		/// The amount.
		/// </value>
		[Required]
		[JsonProperty("amount")]
		public LatitudePayMoney Amount { get; set; }

		internal void Validate(string rootParameterName)
		{
			Reference.GuardNullOrWhiteSpace(rootParameterName, nameof(Reference));
			PaymentPlanToken.GuardNullOrWhiteSpace(rootParameterName, nameof(PaymentPlanToken));

			Amount.Amount.GuardZeroOrNegative(rootParameterName, nameof(Amount));
		}
	}
}
