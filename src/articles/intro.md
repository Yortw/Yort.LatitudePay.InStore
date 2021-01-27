# Introduction

This library is a thin/light weight OOP wrapper around the [LatitudePay API](https://www.genoapay.com/api-doc-v3/). It provides a clean OOP style interface and saves you having to do generate your own models, make HTTP calls, serialise/deserialise requests and responses and so on. It doesn't add any retry logic, persistent stores or reliability handling. It is still up to the application developer to provide a [robust implementation](articles/productionrequirements.html).

This library only implements the in-store related API endpoints and does not implement e-commerce flows.
