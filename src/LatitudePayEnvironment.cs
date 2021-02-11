namespace Yort.LatitudePay.InStore
{
	/// <summary>
	/// A enum providing a list of environments for the LatitudePay API.
	/// </summary>
	public enum LatitudePayEnvironment
	{
		/// <summary>
		/// The test/sandbox API for LatitudePay (use in Australia).
		/// </summary>
		Uat = 0,
		/// <summary>
		/// The live/production API for LatitudePay (use in Australia).
		/// </summary>
		Production,
		/// <summary>
		/// The test/sandbox API for Genoapay (use in New Zealand).
		/// </summary>
		GenoapayUat,
		/// <summary>
		/// The live/production API for Genoapay (use in New Zealand).
		/// </summary>
		GenoapayProduction,
	}
}
