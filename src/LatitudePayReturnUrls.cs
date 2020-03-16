using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Yort.LatitudePay.InStore
{
	/// <summary>
	/// Provides a set of URLs to be called by LatitudePay as the payment plan changes status.
	/// </summary>
	/// <remarks>
	/// <para>LatitudePay with make HTTP GET requests to any callback provided, with query string arguments for details of the payment plan and status associated.</para>
	/// <para>Also included should be a Signature query argument that can be used to verify the callback came from LatitudePay and is not malicious.</para>
	/// </remarks>
	public class LatitudePayReturnUrls
	{
		/// <summary>
		/// Gets or sets the URL to call when the payment plan is approved.
		/// </summary>
		/// <value>
		/// The success URL.
		/// </value>
		[JsonProperty("successUrl")]
		public Uri? SuccessUrl { get; set; }
		/// <summary>
		/// Gets or sets the URL to call when the payment plan is declined or cancelled..
		/// </summary>
		/// <value>
		/// The fail URL.
		/// </value>
		[JsonProperty("failUrl")]
		public Uri? FailUrl { get; set; }
		/// <summary>
		/// Gets or sets a URL to call approximately 10 seconds after the initial callback, as a backup notification in case the first is missed.
		/// </summary>
		/// <value>
		/// The callback URL.
		/// </value>
		[JsonProperty("callbackUrl")]
		public Uri? CallbackUrl { get; set; }
	}
}
