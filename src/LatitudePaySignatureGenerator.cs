using System;
using System.Collections.Generic;
using System.Text;
using Ladon;

namespace Yort.LatitudePay.InStore
{
	/// <summary>
	/// Generates signatures for LatitudePay requests using HMACSHA256.
	/// </summary>
	/// <seealso cref="Yort.LatitudePay.InStore.ILatitudePaySignatureGenerator" />
	public sealed class LatitudePayHMACSHA256SignatureGenerator : ILatitudePaySignatureGenerator 
	{

		private readonly System.Security.Cryptography.HMACSHA256 _Hasher;
		private bool _IsDisposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="LatitudePayHMACSHA256SignatureGenerator"/> class.
		/// </summary>
		/// <param name="apiSecret">The LatitudePay API secret.</param>
		public LatitudePayHMACSHA256SignatureGenerator(string apiSecret)
		{
			_Hasher = new System.Security.Cryptography.HMACSHA256
			(
				System.Text.UTF8Encoding.UTF8.GetBytes
				(
					apiSecret.GuardNullOrWhiteSpace(nameof(apiSecret))
				)
			);
		}

		/// <summary>
		/// Generates the signature for a string containing a LatitudePay request body in json format.
		/// </summary>
		/// <param name="jsonPayload">The json payload.</param>
		/// <remarks>
		/// <para>If <paramref name="jsonPayload"/> is null or an empty string then null is returned as the signature.</para>
		/// </remarks>
		/// <returns>A string containing the required signature.</returns>
		/// <exception cref="ObjectDisposedException">Thrown if <see cref="Dispose"/> has been called on this instance.</exception>
		public string? GenerateSignature(string jsonPayload)
		{
			if (_IsDisposed) throw new ObjectDisposedException(nameof(LatitudePayHMACSHA256SignatureGenerator));
			if (String.IsNullOrEmpty(jsonPayload)) return null;

			//TODO: Lots of allocations going on here, should be improved.
			//Convert byte array to base 64 without converting to string
			//Use recyclable memory streams
			//Reuse/pool the string builder (or replace with a recycable memory stream etc)
			var normalisedPayload = StripJsonFormatting(jsonPayload);
			var base64EncodedPayload = System.Convert.ToBase64String(normalisedPayload);
			var hashBytes = _Hasher.ComputeHash(System.Text.UTF8Encoding.UTF8.GetBytes(base64EncodedPayload));
			
			var resultBuilder = new StringBuilder(hashBytes.Length * 2);
			foreach (var b in hashBytes)
			{
				resultBuilder.Append(b.ToString("x2", System.Globalization.CultureInfo.InvariantCulture));
			}
			return resultBuilder.ToString();
		}

		private static byte[] StripJsonFormatting(string json)
		{
			//Strips json characters and all whitespace as per the GenoaPay signature spec.
			//Weird flex, but ok. Returns the result as a byte array.
			var len = json.Length;
			using (var resultStream = new System.IO.MemoryStream(len))
			using (var writer = new System.IO.StreamWriter(resultStream))
			{
				//increment for every non escaped quote
				int quoteCounter = 0;

				for (var i = 0; i < len; ++i)
				{
					var c = json[i];

					// inc the counter for each non escaped quote. When counter is even, we're processing JSON characters, when odd, we're processing a key or value.
					// With the exception of ints and floats, where the counter will be even
					if (c == '"' && i > 0 && json[i - 1] != '\\')
						++quoteCounter;

					// unconditionally remove spaces
					if (Char.IsWhiteSpace(c)) continue;

					// if processing JSON, and it's a JSON char, don't include the char
					if ((quoteCounter % 2 == 0 && (c == '"' || c == '{' || c == '}' || c == ':' || c == '\n' || c == '\r' || c == ',' || c == '[' || c == ']')))
						continue;

					// append the character as long as it is not a double quote, or if it is an escaped double quote
					if (c != '"' || (json[i - 1] == '\\'))
						writer.Write(c);
				}

				writer.Flush();
				return resultStream.ToArray();
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (!_IsDisposed)
			{
				_IsDisposed = true;
				_Hasher?.Dispose();
			}
		}
	}
}
