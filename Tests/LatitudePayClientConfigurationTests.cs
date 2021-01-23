using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yort.LatitudePay.InStore.Tests
{
	[TestClass]
	public class LatitudePayClientConfigurationTests
	{
		[TestMethod]
		public void Config_ThrowsOnHttpClientSetWhenLocked()
		{
			var config = new LatitudePayClientConfiguration()
			{
				ApiKey = "Test",
				ApiSecret = "Test"
			};
			var hc = new System.Net.Http.HttpClient();
			config.HttpClient = hc;

			Assert.AreEqual(hc, config.HttpClient);

			using (var client = new LatitudePayClient(config))
			{
				Assert.ThrowsException<InvalidOperationException>(() => config.HttpClient = new System.Net.Http.HttpClient());
			}
		}

		[TestMethod]
		public void Config_ThrowsOnProductNameSetWhenLocked()
		{
			var config = new LatitudePayClientConfiguration()
			{
				ApiKey = "Test",
				ApiSecret = "Test"
			};

			config.ProductName = "Test Product";

			Assert.AreEqual("Test Product", config.ProductName);

			using (var client = new LatitudePayClient(config))
			{
				Assert.ThrowsException<InvalidOperationException>(() => config.ProductName = "Test Product 2");
			}
		}

		[TestMethod]
		public void Config_ThrowsOnProductVersionSetWhenLocked()
		{
			var config = new LatitudePayClientConfiguration()
			{
				ApiKey = "Test",
				ApiSecret = "Test"
			};

			config.ProductVersion = "1.0";

			Assert.AreEqual("1.0", config.ProductVersion);

			using (var client = new LatitudePayClient(config))
			{
				Assert.ThrowsException<InvalidOperationException>(() => config.ProductVersion = "1.1");
			}
		}

		[TestMethod]
		public void Config_ThrowsOnProductVendorSetWhenLocked()
		{
			var config = new LatitudePayClientConfiguration()
			{
				ApiKey = "Test",
				ApiSecret = "Test"
			};

			config.ProductVendor = "Test Vendor";

			Assert.AreEqual("Test Vendor", config.ProductVendor);

			using (var client = new LatitudePayClient(config))
			{
				Assert.ThrowsException<InvalidOperationException>(() => config.ProductVendor = "Alternate Test Vendor");
			}
		}

		[TestMethod]
		public void Config_ThrowsOnMinimumRetriesSetWhenLocked()
		{
			var config = new LatitudePayClientConfiguration()
			{
				ApiKey = "Test",
				ApiSecret = "Test"
			};

			config.MinimumRetries = 3;

			Assert.AreEqual(3, config.MinimumRetries);

			using (var client = new LatitudePayClient(config))
			{
				Assert.ThrowsException<InvalidOperationException>(() => config.MinimumRetries = 4);
			}
		}

		[TestMethod]
		public void Config_ThrowsOnRetryDelaySecondsSetWhenLocked()
		{
			var config = new LatitudePayClientConfiguration()
			{
				ApiKey = "Test",
				ApiSecret = "Test"
			};

			config.RetryDelaySeconds = 10;

			Assert.AreEqual(10, config.RetryDelaySeconds);

			using (var client = new LatitudePayClient(config))
			{
				Assert.ThrowsException<InvalidOperationException>(() => config.RetryDelaySeconds = 20);
			}
		}

	}
}
