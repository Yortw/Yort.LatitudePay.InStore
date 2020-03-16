using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace Yort.LatitudePay.InStore
{
	/// <summary>
	/// Represents a response from a <see cref="LatitudePayClient.CreatePosPurchaseAsync(LatitudePayCreatePosPurchaseRequest)"/> request. 
	/// </summary>
	/// <seealso cref="LatitudePayCreatePosPurchaseRequest"/>
	/// <seealso cref="LatitudePayClient"/>
	public class LatitudePayCreatePosPurchaseResponse
	{
		/// <summary>
		/// Gets or sets the payment token for the plan created.
		/// </summary>
		/// <value>
		/// The payment token.
		/// </value>
		[Required]
		[JsonProperty("token")]
		public string? Token { get; set; }
		/// <summary>
		/// Gets or sets a URL that can be called to check the payment status.
		/// </summary>
		/// <value>
		/// The status URL.
		/// </value>
		/// <remarks>
		/// <para>Provided for completeness but typically not used with this library as you use <see cref="LatitudePayClient.GetPurchaseStatusAsync(LatitudePayPurchaseStatusRequest)"/> method with the payment token.</para>
		/// </remarks>
		[JsonProperty("statusUrl")]
		public Uri? StatusUrl { get; set; }
		/// <summary>
		/// Gets or sets the last date and time at which the customer can approve this payment plan.
		/// </summary>
		/// <value>
		/// The expiry date.
		/// </value>
		[JsonProperty("expiryDate")]
		public DateTimeOffset ExpiryDate { get; set; }
		/// <summary>
		/// Gets or sets the dollar figure of the fees charged to the merchant by LatitudePay for this payment plan.
		/// </summary>
		/// <value>
		/// The commission amount.
		/// </value>
		[JsonProperty("commissionAmount")]
		public decimal CommissionAmount { get; set; }
	}
}
