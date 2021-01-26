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


	}
}
