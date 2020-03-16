using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Yort.LatitudePay.InStore
{
	/// <summary>
	/// Represents a customer as part of a <see cref="LatitudePayCreatePosPurchaseRequest"/>.
	/// </summary>
	public class LatitudePayCustomer
	{
		/// <summary>
		/// Gets or sets the mobile number of the customer.
		/// </summary>
		/// <remarks>
		/// <para>Required as this is used to send the customer an SMS message prompting them to approve the payment plan.</para>
		/// </remarks>
		/// <value>
		/// The mobile number.
		/// </value>
		[Required]
		[JsonProperty("mobileNumber")]
		public string? MobileNumber { get; set; }
		/// <summary>
		/// Gets or sets the first name of the customer.
		/// </summary>
		/// <value>
		/// The first name.
		/// </value>
		[JsonProperty("firstName")]
		public string? FirstName { get; set; }
		/// <summary>
		/// Gets or sets the surname of the customer.
		/// </summary>
		/// <value>
		/// The surname.
		/// </value>
		[JsonProperty("surname")]
		public string? Surname { get; set; }
		/// <summary>
		/// Gets or sets the email address of the customer.
		/// </summary>
		/// <value>
		/// The email.
		/// </value>
		[JsonProperty("email")]
		public string? Email { get; set; }
		/// <summary>
		/// Gets or sets the physical/postal address of the customer.
		/// </summary>
		/// <value>
		/// The address.
		/// </value>
		[JsonProperty("address")]
		public LatitudePayAddress? Address { get; set; }
		/// <summary>
		/// Gets or sets the birth date of the customer.
		/// </summary>
		/// <value>
		/// The date of birth.
		/// </value>
		[JsonProperty("dateOfBirth")]
		public DateTime? DateOfBirth { get; set; }
	}
}
