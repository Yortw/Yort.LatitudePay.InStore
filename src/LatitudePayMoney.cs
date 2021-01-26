using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;
using Ladon;
using Mozzarella;
using Newtonsoft.Json;

namespace Yort.LatitudePay.InStore
{
	/// <summary>
	/// Represents a monetary or financial value in the LatitudePay API as a tuple containing a numeric value and a string representing the associated currency.
	/// </summary>
	/// <seealso cref="LatitudePayCurrencies"/>
	/// <remarks>
	/// <para>This is an immutable value type. To set the value or currency you must use the <see cref="LatitudePayMoney(decimal, string)"/> constructor.</para>
	/// <para>Instances created using the default construtor will return a zero value with the <see cref="Currency"/> property returning the value of <see cref="LatitudePayCurrencies.NewZealandDollars"/>.</para>
	/// <para>
	/// <code>
	///		//Assuming you have an variable called payment with a 'value' property containing the decimal amount 
	///		//you want to send.
	///		var amount = new LatitudePayMoney(Math.Round(payment.Value, 2));
	///		
	///		//This sample uses a literal value for illustration purposes
	///		var amount = new LatitudePayMoney(Math.Round(99.9900, 2));
	/// </code>
	/// </para>
	/// </remarks>
	[DebuggerDisplay("{Amount}{Currency}")]
	public struct LatitudePayMoney : IEquatable<LatitudePayMoney>
	{

		private readonly decimal _Amount;
		private string? _Currency;

		/// <summary>
		/// Constructs a new instance using the specified amount and currency.
		/// </summary>
		/// <remarks>
		/// <para>This constructor uses the <see cref="LatitudePayClientConfiguration.DefaultCurrency"/> value for the <see cref="Currency"/> property of this instance. 
		/// If <see cref="LatitudePayClientConfiguration.DefaultCurrency"/> is null or empty string then <see cref="LatitudePayCurrencies.AustralianDollars"/> will be used.</para>
		/// </remarks>
		/// <param name="amount">A decimal value indicating the numeric value of this monetary value. This value should be rounded to the appropriate number of decimal places associated with the currency specified by <see cref="Currency"/>.</param>
		public LatitudePayMoney(decimal amount) : this(amount, null)
		{
		}

		/// <summary>
		/// Constructs a new instance using the specified amount and currency.
		/// </summary>
		/// <param name="amount">A decimal value indicating the numeric value of this monetary value. This value should be rounded to the appropriate number of decimal places associated with the currency specified by <see cref="Currency"/>.</param>
		/// <param name="currency">A three chracter string that identifies the currency this monetary value is in.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="currency"/> is null.</exception>
		/// <exception cref="System.ArgumentException">Thrown if <paramref name="currency"/> is an empty string or contains only whitespace, or has a length other than 3.</exception>
		public LatitudePayMoney(decimal amount, string? currency)
		{
			_Amount = amount;
			if (String.IsNullOrWhiteSpace(currency))
			{
				if (String.IsNullOrWhiteSpace(LatitudePayClientConfiguration.DefaultCurrency))
					currency = LatitudePayCurrencies.AustralianDollars;
				else
					currency = LatitudePayClientConfiguration.DefaultCurrency;
			}
			_Currency = currency.GuardNullOrWhiteSpace(nameof(currency)).GuardLength(nameof(currency), 3, 3);
		}

		/// <summary>
		/// Returns the numeric amount of this monetary value in the currency specified by <see cref="Currency"/>.
		/// </summary>
		/// <remarks>
		/// <para>This value should be rounded to the appropriate number of decimal places associated with the currency specified by <see cref="Currency"/>.</para>
		/// </remarks>
		/// <seealso cref="Currency"/>
		[JsonProperty("amount")]
		public decimal Amount { get { return _Amount; } }

		/// <summary>
		/// Specifies the currency of the <see cref="Amount"/>/
		/// </summary>
		/// <seealso cref="Amount"/>
		[Required]
		[JsonProperty("currency")]
		public string Currency 
		{ 
			get 
			{
				if (String.IsNullOrWhiteSpace(_Currency))
					EnsureCurrencySet();

				return _Currency ?? LatitudePayClientConfiguration.DefaultCurrency ?? LatitudePay.InStore.LatitudePayCurrencies.AustralianDollars; 
			} 
		}

		private void EnsureCurrencySet()
		{
			if (String.IsNullOrWhiteSpace(_Currency))
			{
				if (String.IsNullOrWhiteSpace(LatitudePayClientConfiguration.DefaultCurrency))
					_Currency = LatitudePayCurrencies.AustralianDollars;
				else
					_Currency = LatitudePayClientConfiguration.DefaultCurrency;
			}
		}

		/// <summary>
		/// Returns the <see cref="Amount"/> property formatted as a currency using the current thread culture (which may or may not match the currency defined by <see cref="Currency"/>).
		/// </summary>
		/// <returns>A string containing the formatted amount.</returns>
		public override string ToString()
		{
			return _Amount.ToString("c", System.Globalization.CultureInfo.CurrentCulture);
		}

		#region Equality Related Members & Overrides

		/// <summary>
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A integer that is the hashcode for this instance.</returns>
		public override int GetHashCode()
		{
			unchecked
			{
				return ((_Currency ?? String.Empty).GetHashCode() * 397) ^ _Amount.GetHashCode();
			}
		}

		/// <summary>
		/// Compares this instance to an object value and returns true if they are both <see cref="LatitudePayMoney"/> instances that are considered equal, otherwise false.
		/// </summary>
		/// <param name="obj">The other value to comnpare to.</param>
		/// <returns>True if <paramref name="obj"/> is considered equal to this instance.</returns>
		/// <remarks>
		/// <para>If <paramref name="obj"/> is null or not an instance of <see cref="LatitudePayMoney"/> then false is returned, otherwise the result of <see cref="Equals(LatitudePayMoney)"/> is returned.</para>
		/// </remarks>
		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			if (!(obj is LatitudePayMoney)) return false;

			return Equals((LatitudePayMoney)obj);
		}

		/// <summary>
		/// Returns true if this instance is considered equal to <paramref name="other"/>, otherwise returns false.
		/// </summary>
		/// <param name="other">The other instance to compare to.</param>
		/// <remarks>
		/// <para>Equality is determined by comparing the <see cref="Amount"/> properties for an exact match and the <see cref="Currency"/> properties for an ordinal, case-insensitive match.</para>
		/// </remarks>
		/// <returns>True if the two instances are considered equal.</returns>
		public bool Equals(LatitudePayMoney other)
		{
			return this == other;
		}

		/// <summary>
		/// Performs equality checking on <see cref="LatitudePayMoney"/> instances.
		/// </summary>
		/// <param name="value1">A <see cref="LatitudePayMoney"/> instance.</param>
		/// <param name="value2">A <see cref="LatitudePayMoney"/> instance.</param>
		/// <remarks>
		/// <para>Equality is determined by comparing the <see cref="Amount"/> properties for an exact match and the <see cref="Currency"/> properties for an ordinal, case-insensitive match.</para>
		/// </remarks>
		/// <returns>True if the instances are equal, otherwise false.</returns>
		public static bool operator ==(LatitudePayMoney value1, LatitudePayMoney value2)
		{
			return value1.Amount == value2.Amount
				&& value1.Currency.OCIEquals(value2.Currency);
		}

		/// <summary>
		/// Performs inequality checking on <see cref="LatitudePayMoney"/> instances.
		/// </summary>
		/// <param name="value1">A <see cref="LatitudePayMoney"/> instance.</param>
		/// <param name="value2">A <see cref="LatitudePayMoney"/> instance.</param>
		/// <remarks>Inverts the result of an == comparison.</remarks>
		/// <returns>True if the instances are not equal, otherwise false.</returns>
		public static bool operator !=(LatitudePayMoney value1, LatitudePayMoney value2)
		{
			return !(value1 == value2);
		}

		#endregion

	}
}
