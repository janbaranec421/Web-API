# Web API

**Data loading optimalizations**

### 1) Eager / Lazy / Explicit loading
Choosing between these depends on specifics of models and usage of their related entities.

In general, lazy-loading could be selected as best approach if:
 - we had a lot of nested entities in our models
 - no need to them fetched every time. 
For such scenario, lazy-loading reduces initial loading times. 
Cons of this approach is that it can lead to N+1 query problems.

Otherwise, if we had:
 - small number of nested entities or
 - need to use fetched results right away
Then it would be beneficial to use Eager-loading.
Eager-loading would reduce fetching to single query call, although our initial data load would be larger.

Explicit loading stands somewhere between.  
Could work best for large data sets, and when we would need to eager-load only part of data right away (i.e. take only first 5 records)


### 2) Caching
No one-size-fits-all solution. Should be used to the specific needs of application.
In general, Web APIs are often ideal candidate for caching, because they often contain repeteable data retrievals.

If our API should be:
 - meant to be hosted on single server (or multiple with sticky sessions), simple In-Memory caching could be sufficient.
 - scaled up and hosted on cloud/multiple servers, we could use Distributed In-Memory caching. Redis is popular way to do so.

Next are also advisable to use:  
Response caching - functions as cache on client side, reducing number of requests done to server. Valid for ASP.NET Core 6 apps and older.  
Output caching - newer, more powerful replacement for Response Cache. Allows caching on both Client and Server sides (and many more utilities). Available for apps written in ASP.NET Core 7.0 and higher.

### 3) Asynchronous data fetching
Synchronous access to DB involves a lot of waiting, which hurts scalability of application.  
Using Async/Await to reduce unnecessary waiting for resources from DB.


### 4) Other optimalization options
 - pagination - returning data in smaller chunks
 - connection pooling - increase/tune DB connection pool corresponding to app load
 - payload reduction - return only required data. typically achieved by using DTO's
 - filtering/sorting - should be done on DB level. use indexed columns.
 - rate limiting - add request rate limits to prevent server overload
 - compression - compressing response reduces data transfer sizes
 - monitoring & profiling - to identify performance bottlenecks and issues