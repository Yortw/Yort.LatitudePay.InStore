# Archived
LatitudePay have closed this service and the API is no longer available (at least in NZ). The package is no longer maintained.

# Yort.LatitudePay.InStore
An **unofficial**, light-weight wrapper for .Net, around the LatitudePay (formerly Genoapay) In-Store API (https://www.genoapay.com/api-doc-v3/).

# Status
![Yort.LatitudePay.InStore.Build](https://github.com/Yortw/Yort.LatitudePay.InStore/workflows/Yort.LatitudePay.InStore.Build/badge.svg) [![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT) [![Coverage Status](https://coveralls.io/repos/github/Yortw/Yort.LatitudePay.InStore/badge.svg?branch=master)](https://coveralls.io/github/Yortw/Yort.LatitudePay.InStore?branch=master)
    
## Supported Platforms
Currently;

* .Net Standard 2.0
* .Net 4.5+

Install the Nuget package;

```powershell
    PM> Install-Package Yort.LatitudePay.InStore
```

## What Does It Do (and Not Do)?
This library provides models for the request and response payloads, and a client class to access (only) the in-store API. It provides some basic error handling and handles the authentication process for renewing tokens automatically and creating digital signatures on requests. It can access either the production or sandbox environments. The client class implements an interface, so can be easily mocked/stubbed etc if required for unit testing. Custom HTTP client and clock implementations can be injected into the client. 

It **does not** 
* Provide any retry logic
* Resolve you of the responsibility for writing a robust system.
* Automatically queue reversals.
* Implement any form of crash recovery.
* Provide any access for the web based flow/API.

## How do I use it?

For getting started, samples and API reference [see the docs](https://yortw.github.io/Yort.LatitudePay.InStore/docs/api/index.html)

Also be sure to familiarize yourself with the [LatitudePay API documentation](https://www.genoapay.com/api-doc-v3/)
