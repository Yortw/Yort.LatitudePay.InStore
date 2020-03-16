using System;
using System.Collections.Generic;
using System.Text;
using Ladon;
using Newtonsoft.Json;

namespace Yort.LatitudePay.InStore
{
	/// <summary>
	/// Represents a token returned from the LatitudePay Authorization endpoint.
	/// </summary>
	/// <remarks>
	/// <para>This class is used internally by <see cref="LatitudePayClient"/> which will automatically request and renew tokens as required. It is not expected to be used directly by application code.</para>
	/// </remarks>
	public class LatitudePayAuthToken
	{

		private static readonly TimeSpan ExpiryThreshold = TimeSpan.FromMinutes(1);

		/// <summary>
		/// Gets or sets the authorization token return by LatitudePay.
		/// </summary>
		/// <value>
		/// The token.
		/// </value>
		[JsonProperty("authToken")]
		public string? Token { get; set; }
		/// <summary>
		/// Gets or sets the date and time at which this token expires.
		/// </summary>
		/// <value>
		/// The expiry date and time.
		/// </value>
		[JsonProperty("expiryDate")]
		public DateTimeOffset ExpiryDate { get; set; }

		/// <summary>
		/// Returns a boolean indicating if this <see cref="Token"/> value should currently be usable with the LatitudePay API.
		/// </summary>
		/// <param name="clock">An instance implementing <see cref="ILatitudePaySystemClock"/> used to determine the current time.</param>
		/// <remarks>
		/// <para>Returns true if <paramref name="clock"/> is not empty and the current time (using <paramref name="clock"/>) is more than one minute before <see cref="ExpiryDate"/>.</para>
		/// <para>If <paramref name="clock"/> is null then <see cref="LatitudePaySystemClock.DefaultInstance"/> is used.</para>
		/// </remarks>
		/// <returns>
		///   <c>true</c> if the specified clock is valid; otherwise, <c>false</c>.
		/// </returns>
		public bool IsValid(ILatitudePaySystemClock? clock)
		{
			clock ??= LatitudePaySystemClock.DefaultInstance;

			return !String.IsNullOrEmpty(Token)
				&& clock.Now < ExpiryDate.Subtract(ExpiryThreshold);
		}
	}
}
