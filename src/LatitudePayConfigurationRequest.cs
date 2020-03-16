using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Yort.LatitudePay.InStore
{
	/// <summary>
	/// Represents the options available when requesting configuration information from the LatitudePay API.
	/// </summary>
	/// <seealso cref="LatitudePayConfigurationResponse"/>
	/// <seealso cref="LatitudePayClient.GetConfigurationAsync(LatitudePayConfigurationRequest)"/>
	public class LatitudePayConfigurationRequest
	{
		/// <summary>
		/// Gets or sets the total purchase amount, inclusive of any shipping and tax components.
		/// </summary>
		/// <value>
		/// The total purchase amount, inclusive of any shipping and tax components.
		/// </value>
		[JsonProperty("totalAmount")]
		public decimal TotalAmount { get; set; }
		/// <summary>
		/// If true the description field will not contain any reference to redirecting to the user to the LatitudePay application.
		/// </summary>
		/// <value>
		///   <c>true</c> if the description shoud mention redirecting to LatitudePay, otherwise <c>false</c>.
		/// </value>
		[JsonProperty("displayInModal")]
		public bool DisplayInModal { get; set; } = true;
	}
}
