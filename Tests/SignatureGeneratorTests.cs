using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yort.LatitudePay.InStore.Tests
{
	[TestClass]
	public class SignatureGeneratorTests
	{

		[TestMethod]
		public void Generates_Expected_Signature()
		{
      //This test uses the sample/test data (including secret) from 
      //https://s3-ap-southeast-2.amazonaws.com/genoapay-public-assets/Online+API+Signing+Mechanisms.pdf

      #region Test Data

      var jsonPayload = @"{
    ""customer"": {
        ""mobileNumber"": ""02222222620"",
        ""firstName"": ""John"",
        ""surname"": ""Doe"",
        ""email"": ""jd@genoapay.com"",
        ""address"": {
        ""addressLine1"": ""124 Fifth Avenue"",
            ""suburb"": ""Hobsonville"",
            ""cityTown"": ""Auckland"",
            ""state"": ""Auckland"",
            ""postcode"": ""0618"",
            ""countryCode"": ""NZ""
        },
        ""dateOfBirth"": ""1987-10-17""
    },
    ""shippingAddress"": {
        ""addressLine1"": ""Unit F, 16 Workday Drive"",
        ""suburb"": ""Albany"",
        ""cityTown"": ""Auckland"",
        ""state"": ""Auckland"",
        ""postcode"": ""0751"",
        ""countryCode"": ""NZ""
    },
    ""billingAddress"": {
        ""addressLine1"": ""124 Fifth Avenue"",
        ""suburb"": ""Hobsonville"",
        ""cityTown"": ""Auckland"",
        ""state"": ""Auckland"",
        ""postcode"": ""0618"",
        ""countryCode"": ""NZ""
    },
    ""products"": [
        {
            ""name"": ""Tennis Ball Multipack"",
            ""price"": {
                ""amount"": 30,
                ""currency"": ""NZD""
            },
            ""sku"": ""abc123"",
            ""quantity"": 1,
            ""taxIncluded"": true
        }
    ],
    ""shippingLines"": [
        {
            ""carrier"": ""NZ Post"",
            ""price"": {
                ""amount"": 5.50,
                ""currency"": ""NZD""
            }
        }
    ],
    ""taxAmount"": {
        ""amount"": 5.325,
        ""currency"": ""NZD""
    },
    ""reference"": ""INV000045"",
    ""totalAmount"": {
        ""amount"": 35.5,
        ""currency"": ""NZD""
    },
    ""returnUrls"": {
        ""successUrl"": ""http://genoapay.com/success"",
        ""failUrl"": ""http://.genoapay.com/fail"",
        ""callbackUrl"": ""http://genoapay.com/fail-safe-callback""
    }
}";


			#endregion

			using (var generator = new LatitudePayHMACSHA256SignatureGenerator("1y02Nwqzj1FbznAw"))
      {
        Assert.AreEqual("81ddf72b57031a0b956cc368edac0fcd51d6669a4a0b82cd7aeb3b17e2712389", generator.GenerateSignature(jsonPayload));
      }
		}

    [TestMethod]
    public void Disposes_Ok()
    {
      using (var generator = new LatitudePayHMACSHA256SignatureGenerator("1y02Nwqzj1FbznAw"))
      {
      }
    }

    [ExpectedException(typeof(ObjectDisposedException))]
    [TestMethod]
    public void GenerateSignature_Throws_If_Disposed()
    {
      using (var generator = new LatitudePayHMACSHA256SignatureGenerator("1y02Nwqzj1FbznAw"))
      {
        generator.Dispose();
        generator.GenerateSignature("{ test: \"testvalue\" }");
      }
    }

    [ExpectedException(typeof(ArgumentNullException))]
    [TestMethod]
    public void Constructor_Throws_If_ApiKey_Null()
    {
      using (var generator = new LatitudePayHMACSHA256SignatureGenerator(null))
      {
      }
    }

    [ExpectedException(typeof(ArgumentException))]
    [TestMethod]
    public void Constructor_Throws_If_ApiKey_Empty()
    {
      using (var generator = new LatitudePayHMACSHA256SignatureGenerator(String.Empty))
      {
      }
    }

    [ExpectedException(typeof(ArgumentException))]
    [TestMethod]
    public void Constructor_Throws_If_ApiKey_Is_Whitespace()
    {
      using (var generator = new LatitudePayHMACSHA256SignatureGenerator("   \r\n\t"))
      {
      }
    }

  }
}
