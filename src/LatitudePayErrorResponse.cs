using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Yort.LatitudePay.InStore
{
	internal class LatitudePayErrorResponse
	{
		[JsonProperty("error")]
		public string? ErrorMessage { get; set; }
	}
}
