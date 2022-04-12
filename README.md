# CSharpWebServer
## Component Based Web Server
This project is a skeleton for a web server using my personal C# tools. \
The listener is run by an ASP.NET Kestrel server.

## Overview

### Getting Started
- Create a class derrived from `WebServer`
- Instantiate a `WebServer(...)` instance passing in http/https port numbers to run insecure or secure
- Call `Start()` on instantiated WebServer instance
- It is recommended to use async and await `Start()` to prevent the thread from exiting

### OnStart\[Async\]()
These are the entry points to the web server that are called after the web server is set up and ready to use.
- Override these functions to add custom functionality to the web server
- Should be used instead the constructor

### OnStop\[Async\]()
These are called while the webserver is stopping before it is completely shutdown.
- Override these functions to clean up the webserver before it is shutdown

### Components()
`WebComponent` instances must be instantiated here to be added to the request chain on `WebServer` setup.
- Override this `IEnumerator` and yield `WebComponent` instances to add them to the request chain
- `WebComponent` instances are added to the request chain in the order they are yielded

### Other
- The `WebServer` class contains virtual properties that can be overriden to change the `WebServer` settings
- There are some other functions contained in the `WebServer` class that can be useful

### Components
The `WebComponent` class is the basis of most the `WebServer` functionality. \
They are used to intercept and reroute requests, similar to middleware in other web servers. \
However, they provide predefined functionality to work with different types of request systems. \

`WebComponent` Classes:
- Middleware
- Routing
- Mapping
- ResponseTree

`WebComponent` instances must be instantiated and yielded in the overrided 
`WebServer.Components()` `IEnumerator` using the a `WebServer` creation function for that specific `WebComponent`.

## Middleware: `WebServer.Use(...)`
The `Middleware` `WebComponent` is the lowest level `WebComponent` that will receive every request.

## Routing: `WebServer.Route(...)`
The `Routing` `WebComponent` is used to map a client path to a server path. \
Can optionally use a middleware response to add custom functionality for intercepted requests.

## Mapping: `WebServer.Map[Get, Post](...)`
The `Mapping` `WebComponent` is used to map a client path to a server response. \
The `Read` parameter provides a response to for the given client path using the server path as a parameter.
Can optionally use the `WebServer.MapGet(...)` and `WebServer.MapPost(...)` to target specific request methods.

## ResponseTree: `WebServer.CreateResponseTree(...)`
The `ResponseTree` `WebComponent` is used to map a group of client paths to specific server responses.
A root path must be set and then a variety of responses can be added using `ResponseTree.RootResponse(...)` or `ResponseTree.RelativeResponse(...)`.

## Other Features
- `Authenticator` can be used to facilitate user authentication with any database
- `Templater` can be used to read files from a server directory
  - `WebServer` `ReadServerFile[Async](...)` can also be used for one-off reading of server files
- `Api` can write requests and read responses of any api

## Notes
- There is no implementations to manipulate DOM yet. Instead, there are a variety packages that can be used such as `HtmlAgilityPack`
