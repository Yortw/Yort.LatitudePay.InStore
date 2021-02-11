namespace Yort.LatitudePay.InStore
{
	/// <summary>
	/// Contains constants for useful for dealing with the LatitudePay API.
	/// </summary>
	public static class LatitudePayConstants
	{
		/// <summary>
		/// Returns a string containing the root url for the live/production LatitudePay API.
		/// </summary>
		public const string ProductionRootUrl = "https://api.latitudepay.com";
		/// <summary>
		/// Returns a string containing the root url for the UAT/sandbox/test LatitudePay API.
		/// </summary>
		public const string UatRootUrl = "https://api.uat.latitudepay.com";

		/// <summary>
		/// Returns a string containing the root url for the live/production LatitudePay API.
		/// </summary>
		public const string GenoaProductionRootUrl = "https://api.genoapay.com";
		/// <summary>
		/// Returns a string containing the root url for the UAT/sandbox/test LatitudePay API.
		/// </summary>
		public const string GenoaUatRootUrl = "https://api.uat.genoapay.com";

		/// <summary>
		/// Returns the content-type to use for HTTP requests to the LatitudePay API for version 3 content entities.
		/// </summary>
		public const string LatitudePayV3ContentType = "application/com.latitudepay.ecom-v3.0+json";

		/// <summary>
		/// Returns the content-type to use for HTTP requests to the Genoapay API for version 3 content entities.
		/// </summary>
		public const string GenoapayV3ContentType = "application/com.genoapay.ecom-v3.1+json";

		/// <summary>
		/// The status of a payment plan that has been approved by the customer and payment processor.
		/// </summary>
		public const string StatusApproved = "APPROVED";

		/// <summary>
		/// The status of a payment plan that has been cancelled or declined for some reason (i.e insufficient funds).
		/// </summary>
		public const string StatusDeclined = "DECLINED";

		/// <summary>
		/// The status of a payment plan that has been requested but not yet approved or declined.
		/// </summary>
		public const string StatusPending = "PENDING";
	}
}
