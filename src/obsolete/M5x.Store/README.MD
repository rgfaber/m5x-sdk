# M5x.Store

## Configuration

### Environment Variables

 -  COUCH_LOCAL_PORT: Local CouchDB Port (default: 5984)
 -  COUCH_LOCAL_USER: Local DB User
 -  COUCH_LOCAL_PWD: Local DB Password
 -  COUCH_LOCAL_PROTOCOL: local Protocol 

### Assembly Entry  Points:


 ```csharp
    public static class Inject
    {
        // IoC  support
        public static IServiceCollection AddStore(this IServiceCollection services) {...}
        public static IStoreBuilder StoreBuilder => new StoreBuilder(); 
        
        // Factory Method
        public static IApplicationBuilder UseStore(this IApplicationBuilder app) {...}

    }
```

### Querying

https://github.com/matteobortolazzo/couchdb-net