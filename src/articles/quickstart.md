# Yort.LatitudePay.InStore Quick Start

## Creating a Payment Plan (Taking Payment)
### Basic Process
1. Create an instance of [LatitudePayClient](Yort.LatitudePay.InStore.LatitudePayClient.html)
2. Call [LatitudePayClient.CreatePosPurchaseAsync](Yort.LatitudePay.InStore.LatitudePayClient.html#Yort_LatitudePay_InStore_LatitudePayClient_CreatePosPurchaseAsync_Yort_LatitudePay_InStore_LatitudePayCreatePosPurchaseRequest_)
3. If the response from of step 2 returns a response with a PaymentPlanToken, use [LatitudePayClient.GetPurchaseStatusAsync](Yort.LatitudePay.InStore.LatitudePayClient.html#Yort_LatitudePay_InStore_LatitudePayClient_GetPurchaseStatusAsync_Yort_LatitudePay_InStore_LatitudePayPurchaseStatusRequest_) to poll at *five* seconds (or longer) intervals until an accepted, declined or CancelPurchaseAsyncled status is reached. Prior to the payment request being accepted, declined or CancelPurchaseAsyncled LatitudePay will return a status of *pending*. Handle any transient errors thrown from the polling request.
4. If you decide to timeout/give up polling, call [LatitudePayClient.CancelPurchaseAsync](Yort.LatitudePay.InStore.LatitudePayClient.html#Yort_LatitudePay_InStore_LatitudePayClient_CancelPurchaseAsync_Yort_LatitudePay_InStore_LatitudePayCancelPurchaseRequest_).
5. A status of *Approved* means the payment is successfully completed, and you need to record the *PaymentPlanToken* somewhere if you want to be able to process refunds in the future. A status of *Declined* means the payment was cancelled or failed (i.e due to insufficient credit).

**NB This is only a simple/demo implementation. A production quality implementation needs much more robust logic to handle power failure/crash/network outages and other problems.**

### Add the Nuget package
Install the Nuget package;

```powershell
    PM> Install-Package Yort.LatitudePay.InStore
```

[![NuGet Badge](https://buildstats.info/nuget/Yort.LatitudePay.InStore)]

### Full Code Sample
A full (demo quality) code sample for creating payment, with minimum comments/noise.
See the following sections for a breakdown of what's going on and why.

```c#
    ILatitudePayClient client = new LatitudePayClient
    (
        new LatitudePayClientConfiguration("YourApiKey", "YourClientSecret", LatitudePayEnvironment.Uat)
    );    

    // This is a minimal request, but providing other optional details on the request
    // may help first time users sign up/get approved more quickly and/or provide 
    // a better experience within the LatitudePay portal.
    var request = new LatitudePayCreatePosPurchaseRequest()
    {
        Reference = System.Guid.NewGuid().ToString(),
        Customer = new LatitudePayCustomer()
        {
            MobileNumber = "025555555" // You need to specify the (customer's) mobile number to send the payment request to
        },
        Products = new LatitudePayProduct[] { new LatitudePayProduct() { Name = "Test", Price = new LatitudePayMoney(35.5M, "NZD"), Quantity = 1, Sku = "Test" } }, // LatitudePay requires at least one item specified
        TaxAmount = new LatitudePayMoney(0M, "NZD"),
        TotalAmount = new LatitudePayMoney(35.5M, "NZD")
    };
    request.IdempotencyKey = request.Reference; //Set this to the same value for each request for *this* payment (in the case of retries/crash recovery) to ensure duplicate payments aren't requested.

    LatitudePayCreatePosPurchaseResponse result = await client.CreatePosPurchaseAsync(request);

    //Wait until payment enters final status
    var statusRequest = new LatitudePayPurchaseStatusRequest() { PaymentPlanToken = purchaseResponse.Token };
    var finalStatus = false;
    LatitudePayPurchaseStatusResponse paymentStatus = null;
    while (!finalStatus)
    {
        await Task.Delay(5000).ConfigureAwait(false); //LatitudePay says you should wait 5 seconds between status polls.
        paymentStatus = await client.GetPurchaseStatusAsync(statusRequest).ConfigureAwait(false);

        finalStatus = !String.Equals(paymentStatus.Status, LatitudePayConstants.StatusPending, StringComparison.OrdinalIgnoreCase);
    }

    if (paymentStatus.Status == LatitudePayConstants.StatusApproved)
    {
        //The payment is approved, store the purchaseResponse.Token somewhere so it can be used for refunds in future if required.
    }
    else if (paymentStatus.Status == LatitudePayConstants.StatusDeclined)
    {
        //The payment is declined, get the operator to try again or take some other form of payment etc.
    }
```

### Configure and Create A LatitudePayClient Instance
The LatitudePayClient is the main class in this library to use for accessing the LatitudePay API functionality. To create an instance first you must create a [LatitudePayClientConfiguration](Yort.LatitudePay.InStore.LatitudePayClientConfiguration.html) object that tells the client how to behave. This sample configures a client for accessing the sandbox/text version of the API.

You'll need an api key and secret issued by LatitudePay for the sandbox environment for this to work.

Normally you would create the LatitudePay instance on start-up or first use, and then re-use it across requests instead of creating a new one each time. This allows HTTP connection pooling and improves performance. The LatitudePayClient instance is thread-safe in so much as you can run multiple requests through the same instance for different payments from different threads.

```c#
    //LatitudePay will issue a client id and secret for each environment,
    //you need to contact LatitudePay to get these.
    var config = new LatitudePayClientConfiguration
    (
        "YourApiKey", 
        "YourClientSecret", 
        LatitudePayEnvironment.Uat)
    );

    //We use ILatitudePayClient as the type for the variable holding 
    //the LatitudePayClient instance here as it makes it easier if we 
    //want a test implementation later.
    ILatitudePayClient client = new LatitudePayClient(config);    
```

### Call Create POS Purchase
Now we can create an payment plan. Create a LatitudePayCreatePosPurchaseRequest with details of the payment to take, and pass it to the [LatitudePayClient.CreatePosPurchaseAsync](Yort.LatitudePay.InStore.LatitudePayClient.html#Yort_LatitudePay_InStore_LatitudePayClient_CreatePosPurchaseAsync_Yort_LatitudePay_InStore_LatitudePayCreatePosPurchaseRequest_) method.

```c#
    //We'll set the minimum required properties for this example
    var request = new LatitudePayCreatePosPurchaseRequest()
    {
        Reference = System.Guid.NewGuid().ToString(),
        Customer = new LatitudePayCustomer()
        {
            MobileNumber = "025555555" 
        },
        Products = new LatitudePayProduct[] { new LatitudePayProduct() { Name = "Test", Price = new LatitudePayMoney(35.5M, "NZD"), Quantity = 1, Sku = "Test" } }, 
        TaxAmount = new LatitudePayMoney(0M, "NZD"),
        TotalAmount = new LatitudePayMoney(35.5M, "NZD")
    };
    request.IdempotencyKey = request.Reference; //Set this to the same value for each request for *this* payment (in the case of retries/crash recovery) to ensure duplicate payments aren't requested.

    //Note, this method could throw exceptions. Production quality code
    //would handle them appropriately. 
    LatitudePayCreatePosPurchaseResponse result = await client.CreatePosPurchaseAsync(request);
```

### Poll for Status
Now we need to wait for the customer to approve or decline the payment request via the LatitudePay app or website. If they approve the payment then it will be sent by LatitudePay to the payment processor, and if the payment processor accepts then we'll get a completed status. If either the customer or the payment processor declines the response we'll get a CancelPurchaseAsync/declined or error response.

```c#
    // Create a status request. You can create a new one on each loop or 
    // reuse the same one, which is more efficient so that's what we do here.
    // Unfortunately the LatitudePay status endpoint doesn't use the merchant reference, but 
    // instead the payment plan token returned by the initial create request. If that initial 
    // request is interrupted by a crash, power failure, timeout/network error etc. 
    // you must retry it with duplicate checking enabled until such time as you 
    // receive an payment plan token you can status check against.
    var statusRequest = new LatitudePayPurchaseStatusRequest() { PaymentPlanToken = result.Token };

    //Production quality code would also have a timeout, in case the user
    //is unable to approve or cancel the payment plan in a timely fashion at 
    //the till. There should also be an option for the operator to manually 
    //cancel, in the case where the payment amount has been incorrectly entered 
    //or the consumer is unable to cancel themselves.
    //Wait until payment enters final status
    var statusRequest = new LatitudePayPurchaseStatusRequest() { PaymentPlanToken = purchaseResponse.Token };
    var finalStatus = false;
    LatitudePayPurchaseStatusResponse paymentStatus = null;
    while (!finalStatus)
    {
        await Task.Delay(5000).ConfigureAwait(false); //LatitudePay says you should wait 5 seconds between status polls.
        paymentStatus = await client.GetPurchaseStatusAsync(statusRequest).ConfigureAwait(false);

        finalStatus = !String.Equals(paymentStatus.Status, LatitudePayConstants.StatusPending, StringComparison.OrdinalIgnoreCase);
    }

    if (paymentStatus.Status == LatitudePayConstants.StatusApproved)
    {
        //The payment is approved, store the purchaseResponse.Token somewhere so it can be used for refunds in future if required.
    }
    else if (paymentStatus.Status == LatitudePayConstants.StatusDeclined)
    {
        //The payment is declined, get the operator to try again or take some other form of payment etc.
    }
```

## CancelPurchaseAsyncling a Payment Plan
To cancel a LatitudePay payment plan that is still pending approval (not accepted or declined) you will need the *PaymentPlanToken* returned in the [LatitudePayCreatePosPurchaseResponse](Yort.LatitudePay.InStore.LatitudePayCreatePosPurchaseResponse.html). CancelPurchaseAsync is successful if the CancelPurchaseAsync method does not throw an exception. The LatitudePayCancelPurchaseRequest instance returned will contain the payment plan token and cancellation date as confirmation.

```c#
    try
    {
        var cancelRequest = new LatitudePayCancelPurchaseRequest() { PaymentPlanToken = createPlanResult.Token };
	    var cancelResponse = await client.CancelPurchaseAsync(cancelRequest);
        //Cancellation succeeded
    }
    catch (Exception) // Production code should catch specific exception types
    {
        //CancelPurchaseAsync failed
    }
```

## Refunding a Payment Plan
Once an payment plan has been approved you can make one or more refunds against it, up to the total value of the original payment plan. For each refund create a LatitudePayCreateRefundRequest, specify the payment plan token that was returned when the payment was created, the refund amount and a unique reference for the refund request. Ensure the IdempotentKey is set to a value unique to this refund but common to all retry requests (using the merchant reference for the IdempotentKey is often a good idea). If you get interrupted (power failure, crash, network outage) or suffer a transient error sending a refund, resend the same request again (with the same IdempotentKey value) until you get a valid response. Using the same IdempotentKey value  on the retry requests should prevent multiple refunds being created if the earlier attempts did actually succeed.

```c#
    var refundRequest = new LatitudePayCreateRefundRequest() 
    { 
        PaymentPlanToken = purchaseResponse.Token, 
        Amount = request.TotalAmount, 
        Reason = "Test refund", 
        Reference = System.Guid.NewGuid().ToString() 
    };

    try
    {
        var refundResponse = await client.CreateRefundAsync(refundRequest);
        //Refund succeeded, refundResponse.RefundId will contain the id of the new refund.
    }
    catch (Exception) // Production code should catch specific exception types
    {
        //Refund failed
    }
```
