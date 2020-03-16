using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace Yort.LatitudePay.InStore
{
	/// <summary>
	/// Represents a request to create a new payment plan.
	/// </summary>
	/// <see cref="LatitudePayClient.CreatePosPurchaseAsync(LatitudePayCreatePosPurchaseRequest)"/>
	/// <seealso cref="LatitudePayCreatePosPurchaseResponse"/>
	public class LatitudePayCreatePosPurchaseRequest
	{
		/// <summary>
		/// Gets or sets details about the customer the payment plan is for.
		/// </summary>
		/// <remarks>
		/// <para>You must provide a <see cref="LatitudePayCustomer"/> instance with the <see cref="LatitudePayCustomer.MobileNumber"/> set, but providing additional details when known can speed the sign up process for new customers.</para>
		/// </remarks>
		/// <value>
		/// A <see cref="LatitudePayCustomer"/> containing details of the customer.
		/// </value>
		[Required]
		[JsonProperty("customer")]
		public LatitudePayCustomer? Customer { get; set; }
		/// <summary>
		/// Gets or sets the shipping address for products purchased with this payment plan.
		/// </summary>
		/// <value>
		/// The shipping address.
		/// </value>
		/// <seealso cref="LatitudePayAddress"/>
		[JsonProperty("shippingAddress")]
		public LatitudePayAddress? ShippingAddress { get; set; }
		/// <summary>
		/// Gets or sets the billing address for this payment plan.
		/// </summary>
		/// <value>
		/// The billing address.
		/// </value>
		/// <seealso cref="LatitudePayAddress"/>
		[JsonProperty("billingAddress")]
		public LatitudePayAddress? BillingAddress { get; set; }
		/// <summary>
		/// Gets or sets details of the items purchased with this payment plan.
		/// </summary>
		/// <value>
		/// The products purchased.
		/// </value>
		/// <seealso cref="LatitudePayProduct"/>
		[JsonProperty("products")]
		public IEnumerable<LatitudePayProduct>? Products { get; set; }
		/// <summary>
		/// Gets or sets a collection of details about shipments paid for with this payment plan, if any.
		/// </summary>
		/// <value>
		/// The shipping lines.
		/// </value>
		/// <seealso cref="LatitiudePayShippingLine"/>
		[JsonProperty("shippingLines")]
		public IEnumerable<LatitiudePayShippingLine>? ShippingLines { get; set; }
		/// <summary>
		/// Gets or sets the amount of tax included in <see cref="TotalAmount"/>.
		/// </summary>
		/// <value>
		/// The tax amount included in this payment plan.
		/// </value>
		/// <seealso cref="TotalAmount"/>
		/// <seealso cref="LatitudePayMoney"/>
		[JsonProperty("taxAmount")]
		public LatitudePayMoney TaxAmount { get; set; }

		/// <summary>
		/// A unique reference for this payment plan.
		/// </summary>
		/// <value>
		/// The client generated reference for this payment plan.
		/// </value>
		[Required]
		[JsonProperty("reference")]
		public string? Reference { get; set; }

		/// <summary>
		/// Gets or sets the total amount.
		/// </summary>
		/// <value>
		/// The total amount.
		/// </value>
		/// <remarks>
		/// <para>The amount specified here is the full amount charged as part of the payment plan. It does not have to match the sum of values from <see cref="Products"/> in the case of split payments or complex POS transactions.</para>
		/// </remarks>
		/// <seealso cref="LatitudePayMoney"/>
		/// <seealso cref="LatitudePayMoney"/>
		[Required]
		[JsonProperty("totalAmount")]
		public LatitudePayMoney TotalAmount { get; set; }

		/// <summary>
		/// Gets or sets url's that will be used for callbacks when the payment plan changes status.
		/// </summary>
		/// <value>
		/// The callback urls.
		/// </value>
		[JsonProperty("returnUrls")]
		public LatitudePayReturnUrls? ReturnUrls { get; set; }
	}
}
