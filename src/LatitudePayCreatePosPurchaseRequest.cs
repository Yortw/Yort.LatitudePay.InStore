using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Ladon;
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
		/// A value unique to this payment, but common to all CreatePosPurchaseRequests sent for this payment, to ensure the payment is created only once.
		/// </summary>
		/// <remarks>
		/// <para>This value is optional but highly recommended as without using it you cannot guarantee network problems and other issues won't lead to customers making payments the system doesn't know about.</para>
		/// <para>Set this property to a value that is unique for this *payment* but the same for each call to <see cref="ILatitudePayClient.CreatePosPurchaseAsync(LatitudePayCreatePosPurchaseRequest)"/> for that payment.
		/// Any repeat requests using the same idempotency key value will return the original response without creating a second payment or sending the customer another message. If the original request never got processed 
		/// (due to power failure, network outage etc) then the next request will be treated as the first one. This guarantees the customer gets one and only one request to make payment.</para>
		/// </remarks>
		[JsonIgnore]
		public string? IdempotencyKey { get; set; }

		/// <summary>
		/// Gets or sets details about the customer the payment plan is for. Required (see remarks).
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
		/// Gets or sets the shipping address for products purchased with this payment plan. Optional.
		/// </summary>
		/// <value>
		/// The shipping address.
		/// </value>
		/// <seealso cref="LatitudePayAddress"/>
		[JsonProperty("shippingAddress")]
		public LatitudePayAddress? ShippingAddress { get; set; }
		/// <summary>
		/// Gets or sets the billing address for this payment plan. Optional.
		/// </summary>
		/// <value>
		/// The billing address.
		/// </value>
		/// <seealso cref="LatitudePayAddress"/>
		[JsonProperty("billingAddress")]
		public LatitudePayAddress? BillingAddress { get; set; }
		/// <summary>
		/// Gets or sets details of the items purchased with this payment plan. At least one valid <see cref="LatitudePayProduct"/> is required.
		/// </summary> 
		/// <remarks>
		/// <para>Note products collection may be enumerated multiple times as part of the validation and send processes, so should be a stable enumerable set and not a LINQ or dynamic query that might be subject to change or iterable only once.</para>
		/// <para>The product information is informative only (displayed to customer and merchant when viewing transaction in LatitudePay portals), it is not required for the value of items to total to the payment amount etc.</para>
		/// </remarks>
		/// <value>
		/// The products purchased.
		/// </value>
		/// <seealso cref="LatitudePayProduct"/>
		[JsonProperty("products")]
		public IEnumerable<LatitudePayProduct>? Products { get; set; }
		/// <summary>
		/// Gets or sets a collection of details about shipments paid for with this payment plan, if any. Optional.
		/// </summary>
		/// <remarks>
		/// <para>While you do not have to provide any shipping lines, this value cannot be null when sent to LatitudePay. If the value is left null, an empty array will be assigned for you when the request is sent.</para>
		/// </remarks>
		/// <value>
		/// The shipping lines.
		/// </value>
		/// <seealso cref="LatitiudePayShippingLine"/>
		[JsonProperty("shippingLines")]
		public IEnumerable<LatitiudePayShippingLine>? ShippingLines { get; set; }
		/// <summary>
		/// Gets or sets the amount of tax included in <see cref="TotalAmount"/>. Optional, can be set to zero for the currency of the payment.
		/// </summary>
		/// <value>
		/// The tax amount included in this payment plan.
		/// </value>
		/// <seealso cref="TotalAmount"/>
		/// <seealso cref="LatitudePayMoney"/>
		[JsonProperty("taxAmount")]
		public LatitudePayMoney TaxAmount { get; set; }

		/// <summary>
		/// A unique reference for this payment plan. Required.
		/// </summary>
		/// <value>
		/// The client generated reference for this payment plan.
		/// </value>
		[Required]
		[JsonProperty("reference")]
		public string? Reference { get; set; }

		/// <summary>
		/// Gets or sets the total amount. Must be a positive non-zero value.
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
		/// Gets or sets url's that will be used for callbacks when the payment plan changes status. Optional, can be null.
		/// </summary>
		/// <value>
		/// The callback urls.
		/// </value>
		[JsonProperty("returnUrls")]
		public LatitudePayReturnUrls? ReturnUrls { get; set; }

		internal void Validate(string rootParameterName)
		{
			Reference.GuardNullOrWhiteSpace(rootParameterName, nameof(Reference));

			Customer.GuardNull(rootParameterName, nameof(Customer));
			Customer?.MobileNumber.GuardNullOrWhiteSpace(rootParameterName, nameof(Customer) + "." + nameof(Customer.MobileNumber));

			TotalAmount.Amount.GuardZeroOrNegative(rootParameterName, nameof(TotalAmount));
			TotalAmount.Currency.GuardNullOrWhiteSpace(rootParameterName, nameof(TotalAmount.Currency));
			TaxAmount.Amount.GuardNegative(rootParameterName, nameof(TaxAmount));
			TaxAmount.Currency.GuardNullOrWhiteSpace(rootParameterName, nameof(TaxAmount.Currency));

			Products.GuardNull(rootParameterName, nameof(Products));

			int productIndex = 0;
			if (Products != null)
			{
				foreach (var p in Products)
				{
					if (p == null) continue;

					var propertyPath = $"Products[{productIndex.ToString(System.Globalization.CultureInfo.InvariantCulture)}]";
					p.Name.GuardNullOrWhiteSpace(rootParameterName, propertyPath + "." + nameof(p.Name));
					p.Price.Amount.GuardNegative(rootParameterName, propertyPath + "." + nameof(p.Price));
					p.Quantity.GuardZeroOrNegative(rootParameterName, propertyPath + "." + nameof(p.Quantity));

					productIndex++;
				}
			}
			if (productIndex == 0) throw new ArgumentException(ErrorMessages.AtleastOneProductRequired, rootParameterName + "." + nameof(Products));


			//LatitudePay API will return a 500 error with a useless description if ShippingLines property is null.
			//Rather than make that the clients problem, just fix it for them.
			if (ShippingLines == null)
			{
#if NETSTANDARD2_0
				ShippingLines = Array.Empty<LatitiudePayShippingLine>();
#else
				ShippingLines = new LatitiudePayShippingLine[] { };
#endif
			}
		}
	}
}
