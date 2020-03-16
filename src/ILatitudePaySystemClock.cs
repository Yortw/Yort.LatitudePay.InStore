using System;

namespace Yort.LatitudePay.InStore
{
	/// <summary>
	/// Interface for components that can return the current date and time.
	/// </summary>
	/// <see cref="LatitudePaySystemClock"/>
	public interface ILatitudePaySystemClock
	{
		/// <summary>
		/// Returns the current date and time as a <see cref="DateTimeOffset"/> value.
		/// </summary>
		DateTimeOffset Now { get; }
	}
}
