using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yort.LatitudePay.InStore.Tests
{
	[TestClass]
	public class LatitudePayMoneyTests
	{
		[TestMethod]
		public void InitialisesWith_DefaultCurrency()
		{
			LatitudePayClientConfiguration.DefaultCurrency = LatitudePayCurrencies.AustralianDollars;

			var value = new LatitudePayMoney(10M, null);
			Assert.AreEqual(value.Currency, LatitudePayCurrencies.AustralianDollars);

			LatitudePayClientConfiguration.DefaultCurrency = LatitudePayCurrencies.NewZealandDollars;

			var value2 = new LatitudePayMoney(10M, null);
			Assert.AreEqual(value2.Currency, LatitudePayCurrencies.NewZealandDollars);

			LatitudePayClientConfiguration.DefaultCurrency = LatitudePayCurrencies.AustralianDollars;

			var value3 = new LatitudePayMoney();
			Assert.AreEqual(value3.Currency, LatitudePayCurrencies.AustralianDollars);


			LatitudePayClientConfiguration.DefaultCurrency = LatitudePayCurrencies.NewZealandDollars;

			var value4 = new LatitudePayMoney();
			Assert.AreEqual(value4.Currency, LatitudePayCurrencies.NewZealandDollars);

		}

		[TestMethod]
		public void ToString_FormatsAmountAsCurrency()
		{
			LatitudePayClientConfiguration.DefaultCurrency = LatitudePayCurrencies.AustralianDollars;

			var value = new LatitudePayMoney(15M, LatitudePayCurrencies.AustralianDollars);
			var x = value.ToString();
			Assert.AreEqual(15.ToString("c", System.Globalization.CultureInfo.CurrentCulture), x);
		}

		[TestMethod]
		public void EquivalentValues_GenerateSameHashCode()
		{
			LatitudePayClientConfiguration.DefaultCurrency = LatitudePayCurrencies.AustralianDollars;

			var value = new LatitudePayMoney(15M, LatitudePayCurrencies.AustralianDollars);
			var value2 = new LatitudePayMoney(15M, LatitudePayCurrencies.AustralianDollars);

			Assert.AreEqual(value.GetHashCode(), value2.GetHashCode());
		}

		[TestMethod]
		public void DifferentCurrencies_GenerateDifferentHashCode()
		{
			LatitudePayClientConfiguration.DefaultCurrency = LatitudePayCurrencies.AustralianDollars;

			var value = new LatitudePayMoney(15M, LatitudePayCurrencies.AustralianDollars);
			var value2 = new LatitudePayMoney(15M, LatitudePayCurrencies.NewZealandDollars);

			Assert.AreNotEqual(value.GetHashCode(), value2.GetHashCode());
		}

		[TestMethod]
		public void DifferentValues_GenerateDifferentHashCode()
		{
			LatitudePayClientConfiguration.DefaultCurrency = LatitudePayCurrencies.AustralianDollars;

			var value = new LatitudePayMoney(15M, LatitudePayCurrencies.AustralianDollars);
			var value2 = new LatitudePayMoney(10M, LatitudePayCurrencies.AustralianDollars);

			Assert.AreNotEqual(value.GetHashCode(), value2.GetHashCode());
		}

		[TestMethod]
		public void DoesNotEqualNull()
		{
			LatitudePayClientConfiguration.DefaultCurrency = LatitudePayCurrencies.AustralianDollars;

			var value = new LatitudePayMoney(15M, LatitudePayCurrencies.AustralianDollars);
			Assert.IsFalse(value.Equals(null));
		}

		[TestMethod]
		public void DoesNotEqualOtherType()
		{
			LatitudePayClientConfiguration.DefaultCurrency = LatitudePayCurrencies.AustralianDollars;

			var value = new LatitudePayMoney(15M, LatitudePayCurrencies.AustralianDollars);
			Assert.IsFalse(value.Equals(new object()));
		}

		[TestMethod]
		public void EqualsSelf()
		{
			LatitudePayClientConfiguration.DefaultCurrency = LatitudePayCurrencies.AustralianDollars;

			var value = new LatitudePayMoney(15M, LatitudePayCurrencies.AustralianDollars);
			Assert.IsTrue(value.Equals(value));
			Assert.AreEqual(value, value);
#pragma warning disable CS1718 // Comparison made to same variable
			Assert.IsTrue(value == value);
#pragma warning restore CS1718 // Comparison made to same variable
		}

		[TestMethod]
		public void EqualsEquivalent()
		{
			LatitudePayClientConfiguration.DefaultCurrency = LatitudePayCurrencies.AustralianDollars;

			var value = new LatitudePayMoney(15M, LatitudePayCurrencies.AustralianDollars);
			var value2 = new LatitudePayMoney(15M, LatitudePayCurrencies.AustralianDollars);
			Assert.IsTrue(value.Equals(value2));
			Assert.AreEqual(value, value2);
			Assert.IsTrue(value == value2);
		}

		[TestMethod]
		public void InequalWhenCurrencyDifferent()
		{
			var value = new LatitudePayMoney(15M, LatitudePayCurrencies.AustralianDollars);
			var value2 = new LatitudePayMoney(15M, LatitudePayCurrencies.NewZealandDollars);
			Assert.IsFalse(value.Equals(value2));
			Assert.AreNotEqual(value, value2);
			Assert.IsTrue(value != value2);
		}

		[TestMethod]
		public void InequalWhenAmountDifferent()
		{
			var value = new LatitudePayMoney(15M, LatitudePayCurrencies.AustralianDollars);
			var value2 = new LatitudePayMoney(10M, LatitudePayCurrencies.AustralianDollars);
			Assert.IsFalse(value.Equals(value2));
			Assert.AreNotEqual(value, value2);
			Assert.IsTrue(value != value2);
		}

	}
}
