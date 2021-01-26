using Ladon;

namespace Yort.LatitudePay.InStore
{
	/// <summary>
	/// Represents a request to cancel a payment plan previously requested and not yet accepted or declined.
	/// </summary>
	public class LatitudePayCancelPurchaseRequest
	{
		/// <summary>
		/// Gets or sets the payment token of the plan to be cancelled. Required.
		/// </summary>
		/// <value>
		/// The payment plan token.
		/// </value>
		/// <seealso cref="LatitudePayClient.CancelPurchaseAsync(LatitudePayCancelPurchaseRequest)"/>
		/// <seealso cref="LatitudePayClient"/>
		/// <seealso cref="LatitudePayCancelPurchaseResponse"/>
		/// <seealso cref="LatitudePayCreatePosPurchaseResponse.Token"/>
		public string? PaymentPlanToken { get; set; }

		internal void Validate(string rootParameterName)
		{
			PaymentPlanToken.GuardNullOrWhiteSpace(rootParameterName, nameof(PaymentPlanToken));
		}
	}
}
