using System;
using System.Threading.Tasks;

namespace Yort.LatitudePay.InStore
{
	/// <summary>
	/// Interface for the <see cref="ILatitudePayClient"/>, provided to make testing via mocks/stubs etc. easier.
	/// </summary>
	/// <seealso cref="System.IDisposable" />
	public interface ILatitudePayClient : IDisposable
	{
		/// <summary>
		/// Gets configuration information for the current merchant account, specifying the supported currency, minimum and maximum purchase values etc.
		/// </summary>
		/// <param name="request">A <see cref="LatitudePayConfigurationRequest"/> that specifies options for the data returned.</param>
		/// <returns>A <see cref="LatitudePayConfigurationResponse"/> instance.</returns>
		Task<LatitudePayConfigurationResponse> GetConfigurationAsync(LatitudePayConfigurationRequest request);

		/// <summary>
		/// Creates a new 'payment plan'.
		/// </summary>
		/// <param name="request">A <see cref="LatitudePayCreatePosPurchaseRequest"/> instance specifying options for the payment plan to be created.</param>
		/// <returns>A <see cref="LatitudePayCreatePosPurchaseResponse"/> instance containing details of the pending payment plan.</returns>
		/// <seealso cref="GetPurchaseStatusAsync(LatitudePayPurchaseStatusRequest)"/>
		/// <seealso cref="CancelPurchaseAsync(LatitudePayCancelPurchaseRequest)"/>
		/// <seealso cref="CreateRefundAsync(LatitudePayCreateRefundRequest)"/>
		Task<LatitudePayCreatePosPurchaseResponse> CreatePosPurchaseAsync(LatitudePayCreatePosPurchaseRequest request);

		/// <summary>
		/// Gets the status of a payment plan previously requested via <see cref="CreatePosPurchaseAsync(LatitudePayCreatePosPurchaseRequest)"/>.
		/// </summary>
		/// <param name="request">A <see cref="LatitudePayPurchaseStatusRequest"/> instance containing the token of the payment plan who's status should be queried.</param>
		/// <returns>A <see cref="LatitudePayPurchaseStatusResponse"/> instance containing the status and other details of the plan.</returns>
		/// <seealso cref="CreatePosPurchaseAsync(LatitudePayCreatePosPurchaseRequest)"/>
		/// <seealso cref="CancelPurchaseAsync(LatitudePayCancelPurchaseRequest)"/>
		Task<LatitudePayPurchaseStatusResponse> GetPurchaseStatusAsync(LatitudePayPurchaseStatusRequest request);

		/// <summary>
		/// Cancels a (pending) payment plan previously requested via <see cref="CreatePosPurchaseAsync(LatitudePayCreatePosPurchaseRequest)"/>.
		/// </summary>
		/// <remarks>
		/// <para>This only cancels pending/unapproved payment plans. If a plan has been accepted/approved you must refund it instead of cancel it.</para>
		/// </remarks>
		/// <param name="request">A <see cref="LatitudePayCancelPurchaseRequest"/> containing the token of the payment plan to cancel.</param>
		/// <returns>An instance of a <see cref="LatitudePayCancelPurchaseResponse"/> indicating if the cancellation was successful.</returns>
		/// <seealso cref="CreatePosPurchaseAsync(LatitudePayCreatePosPurchaseRequest)"/>
		/// <seealso cref="CreateRefundAsync(LatitudePayCreateRefundRequest)"/>
		Task<LatitudePayCancelPurchaseResponse> CancelPurchaseAsync(LatitudePayCancelPurchaseRequest request);

		/// <summary>
		/// Refunds a previously approved payment plan.
		/// </summary>
		/// <remarks>
		/// <para>You can only refund a previously accepted payment plan. Use <see cref="CancelPurchaseAsync(LatitudePayCancelPurchaseRequest)"/> to cancel a payment plan that is pending.</para>
		/// <para>A refund can be full or partial, and multiple partial refunds can be made. You can only refund up to the total amount of the original payment plan, across all refunds.</para>
		/// </remarks>
		/// <param name="request">A <see cref="LatitudePayCreateRefundRequest"/> instance containing details of the refund and payment plan to refund against.</param>
		/// <returns>A <see cref="LatitudePayCreateRefundResponse"/> instance indicating if the refund was successful and details of the refund created.</returns>
		Task<LatitudePayCreateRefundResponse> CreateRefundAsync(LatitudePayCreateRefundRequest request);
	}
}
