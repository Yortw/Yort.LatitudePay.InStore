using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.LatitudePay.InStore
{
	/// <summary>
	/// Thrown when an (HTTP) error response is received from the LatitudePay API.
	/// </summary>
	[Serializable]
	public class LatitudePayApiException : Exception
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public LatitudePayApiException() { }
		/// <summary>
		/// Partial constructor.
		/// </summary>
		/// <param name="message">The error message to associated with this exception.</param>
		public LatitudePayApiException(string message) : base(message) { }
		/// <summary>
		/// Partial constructor.
		/// </summary>
		/// <param name="message">The error message to associated with this exception.</param>
		/// <param name="inner">The exception wrapped by this exception.</param>
		public LatitudePayApiException(string message, Exception inner) : base(message, inner) { }

		/// <summary>
		/// Recommended constructor.
		/// </summary>
		/// <param name="message">The error message returned by the LatitudePay API.</param>
		/// <param name="statusCode">The HTTP status code returned by the LatitudePay API.</param>
		public LatitudePayApiException(string message, int statusCode) : base(message)
		{
			this.Data["StatusCode"] = statusCode;
		}

		/// <summary>
		/// Deserialisation constructor.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected LatitudePayApiException(
		System.Runtime.Serialization.SerializationInfo info,
		System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

		/// <summary>
		/// Returns the HTTP status code associated with this exception, if any, otherwise -1.
		/// </summary>
		public int StatusCode
		{
			get 
			{
				if (this.Data.Contains("StatusCode"))
					return Convert.ToInt32(this.Data["StatusCode"]);

				return -1;
			}
		}
	}
}
