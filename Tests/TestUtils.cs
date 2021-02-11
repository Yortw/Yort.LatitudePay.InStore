using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.LatitudePay.InStore.Tests
{
	internal static class TestUtils
	{
		internal static ILatitudePayClient GetSandboxClient()
		{
			var config = new LatitudePayClientConfiguration()
			{
				ApiKey = Environment.GetEnvironmentVariable("LatitudePay_ApiKey"),
				ApiSecret = Environment.GetEnvironmentVariable("LatitudePay_ApiSecret"),
				Environment = LatitudePayEnvironment.Uat
			};
			return new LatitudePayClient(config);
		}

		internal static ILatitudePayClient GetGenoapaySandboxClient()
		{
			var config = new LatitudePayClientConfiguration()
			{
				ApiKey = Environment.GetEnvironmentVariable("LatitudePay_ApiKey"),
				ApiSecret = Environment.GetEnvironmentVariable("LatitudePay_ApiSecret"),
				Environment = LatitudePayEnvironment.GenoapayUat
			};
			return new LatitudePayClient(config);
		}

		internal static ILatitudePayClient GetMockClient()
		{
			var config = new LatitudePayClientConfiguration()
			{
				ApiKey = "MockKey",
				ApiSecret = "MockSecret",
				Environment = LatitudePayEnvironment.Uat,
				HttpClient = new System.Net.Http.HttpClient(new MockLatitudePayHttpHandler())
			};
			return new LatitudePayClient(config);
		}

	}
}
