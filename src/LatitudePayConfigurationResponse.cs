using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.LatitudePay.InStore
{
	/// <summary>
	/// Represents the response from a <see cref="LatitudePayClient.GetConfigurationAsync(LatitudePayConfigurationRequest)"/> call.
	/// </summary>
	/// <seealso cref="LatitudePayClient.GetConfigurationAsync(LatitudePayConfigurationRequest)"/>
	/// <seealso cref="LatitudePayConfigurationRequest"/>
	public class LatitudePayConfigurationResponse
	{
		/// <summary>
		/// Gets or sets the name of the service (LatitudePay/GenoaPay etc).
		/// </summary>
		/// <value>
		/// The service name.
		/// </value>
		public string? Name { get; set; }
		/// <summary>
		/// Gets or setsa description of a payment plan using the <see cref="LatitudePayConfigurationRequest.TotalAmount"/> specified.
		/// </summary>
		/// <value>
		/// The supposed payment plan description.
		/// </value>
		public string? Description { get; set; }
		/// <summary>
		/// Gets or sets the minimum purchase amount.
		/// </summary>
		/// <value>
		/// The minimum purchase amount.
		/// </value>
		public decimal MinimumAmount { get; set; }
		/// <summary>
		/// Gets or sets the maximum purchase amount.
		/// </summary>
		/// <value>
		/// The maximum purchase amount.
		/// </value>
		public decimal MaximumAmount { get; set; }
		/// <summary>
		/// Gets or sets a collection of <see cref="Availability"/> instances specifying currencies and countries usable with the current merchant account.
		/// </summary>
		/// <value>
		/// The availability details for the current merchant account.
		/// </value>
		public IEnumerable<LatitudePayAvailability>? Availability { get; set; }
	}
}
