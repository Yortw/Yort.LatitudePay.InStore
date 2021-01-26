using System.ComponentModel.DataAnnotations;
using Ladon;
using Newtonsoft.Json;

namespace Yort.LatitudePay.InStore
{
	/// <summary>
	/// Represents a request to obtain the status of a payment plan created via <see cref="LatitudePayClient.CreatePosPurchaseAsync(LatitudePayCreatePosPurchaseRequest)"/>.
	/// </summary>
	public class LatitudePayPurchaseStatusRequest
	{
		/// <summary>
		/// Gets or sets the payment token for the plan. Required.
		/// </summary>
		/// <value>
		/// The payment plan token.
		/// </value>
		[Required]
		[JsonProperty("paymentPlanToken")]
		public string? PaymentPlanToken { get; set; }

		internal void Validate(string rootParameterName)
		{
			PaymentPlanToken.GuardNullOrWhiteSpace(rootParameterName, nameof(PaymentPlanToken));
		}

	}
}
