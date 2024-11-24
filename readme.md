# Project for recruitment

## Task

Using ASP.NET Core, implement a RESTful API to retrieve the details of the best n stories from the Hacker News API, as determined by their score, where n is
specified by the caller to the API.
The Hacker News API is documented here: https://github.com/HackerNews/API .
The IDs for the stories can be retrieved from this URI: https://hacker-news.firebaseio.com/v0/beststories.json .
The details for an individual story ID can be retrieved from this URI: https://hacker-news.firebaseio.com/v0/item/21233041.json (in this case for the story with ID
21233041 )
The API should return an array of the best n stories as returned by the Hacker News API in descending order of score, in the form:

```json 
[
    {
        "title": "A uBlock Origin update was rejected from the Chrome Web Store",
        "uri": "https://github.com/uBlockOrigin/uBlock-issues/issues/745",
        "postedBy": "ismaildonmez",
        "time": "2019-10-12T13:43:01+00:00",
        "score": 1716,
        "commentCount": 572
    },
    { ... },
    { ... },
    { ... },
    ...
]
```
In addition to the above, your API should be able to efficiently service large numbers of requests without risking overloading of the Hacker News API.
You should share a public repository with us, that should include a README.md file which describes how to run the application, any assumptions you have made, and
any enhancements or changes you would make, given the time.

## Solution

The solution is a .Net 9 WebApi, using minimal apis approach and vertical slice architecture.
Hybrid Cache is used to limit calls to Hacker Rank API. It is used as in-memory cache with an option of adding distributed cache in future, in case the service have multiple replicas deployed.

### Local setup

1. .Net SDK 9 and Hosting bundle must be installed. Get appropriate installers from [official download page](https://dotnet.microsoft.com/en-us/download/dotnet/9.0).
1. IDE or code editor such as Visual Studio, Rider or Visual Studio Code to send requests using samples from `santa.api.http`. Can be omitted if API testing will be done using other tools.

### Running the Project

#### Rider or Visual Studio 

One option is to open the IDE and run `santa.api` project using `http` profile. In case of using `https` profile, in order to test the API, url in `santa.api.http` file must be updated to match the url of the started service.  

#### Terminal

Another option is to navigate to the folder containing `santa.api.csproj` and execute `dotnet run --project santa.api.csproj` to start the application.

### Testing the API

Sample API requests are available in `santa.api.http`. Those can be executed from IDEs. `@santaapi_HostAddress` variable should match URL under which the service is available. One of following options can be used to test the API:

- Rider 2024.3 and Visual Studio 2022 have a built-in support for executing requests in *.http files
- Visual Studio Code extension might be required to execute requests from *.http files (REST Client extension can be used)
- sending requests through other tools to `http://localhost:5223/beststories?numberOfStories=200`, as long as the service is started using `http` profile under port 5223.

## Possible improvements

1. Preload cache on application start to reduce the time it takes to deliver response to a first request
1. Add distributed caching if there will be multiple instances of the service
1. Add tracing to have better overview over the system (if additional components were added)
1. Add logging to have better overview over the application
1. Adjust resilience options when calling 3rd party API
1. Adjust cache length to be appropriate for the frequency of updates
