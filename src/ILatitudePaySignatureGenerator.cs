using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.LatitudePay.InStore
{
	/// <summary>
	/// An interface for components that can calculate a signature for LatitudePay requests.
	/// </summary>
	/// <remarks>
	/// <para>This interface exists mostly for future expansion (in case the algorithm or hash used changes) and internal testing, it is not expected to be used directly from application code.</para>
	/// </remarks>
	/// <seealso cref="System.IDisposable" />
	public interface ILatitudePaySignatureGenerator : IDisposable
	{
		/// <summary>
		/// Generates a signature for the specified payload.
		/// </summary>
		/// <param name="jsonPayload">The json payload to calculate a signature for.</param>
		/// <returns></returns>
		string? GenerateSignature(string jsonPayload);
	}
}
