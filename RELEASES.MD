# Releases

## August 2022
- v2.8.0 |> Security Updates (Bogus, Chassis, Docker, gRPC, Minio, Serilog, Swagger, TermUI, Testing, Twilio, Units, Yaml)

## July 2022
- v2.7.0 |> Security Updates (Bogus, Chassis, Couch, DEC, ElasticSearch, Grpc, Kubernetes, Redis, Serilog, Swagger, Testing, Twilio, Units)

## June 2022
- v2.6.2 |> Security Updates (Redis, Twilio, Units)
- v2.6.1 |> Security Updates (Chassis, DEC.Schema, Kubernetes, MailKit, Minio, Serilog, Testing, Units)
- v2.6.0 |> Security Updates

## May 2022
- v2.5.0 |> Security Updates (DEC.Schema, Serilog, Testing) 

## April 2022
- v2.4.2 |> Security Updates (Bogus, Grpc, Kubernetes, Serilog, Sso, Swagger, TermUI)
- v2.4.1 |> Security Updates (Bogus, ElasticSearch, Minio, Sso, Swagger, Twilio)
- v2.4.0 |> Security Updates (Ardalis, Bogus, Chassis, Grpc, Kubernetes, MailKit, Redis, Testing, Twilio, Units)

## March 2022
- v2.3.0 |> EventStoreDB Update (DEC.Infra) 

## February 2022
- v2.2.3 |> Security Updates (Chassis, Cocona, Couch, DEC.Infra, Grpc, Kubernetes, NATS, Serilog, Swagger, Testing, Units) 
- v2.2.2 |> Added M5x.Units
- v2.2.1 |> Added M5x.Cocona, Dependency Updates (AutoMapper, Testing, Kubernetes, Twilio, NSwag, ElasticSearch, Swagger)
- v2.2.0 |> Dependency updates (Bogus, Kubernetes, Grpc, Mailkit, Swagger, Twilio)

## January 2022
- Happy New Year!
- v2.1.0 |> Dependency updates (AutoMapper, Serilog, MediatR, Grpc)

## December 2021
- v2.0.3 |> Dependency Updates (Kubernetes, Bogus)
- v2.0.2 |> Dependency updates (Grpc, Mailkit, NEST, MSwag)
- v2.0.1 |> Dependency Updates

## November 2021

### v2.0.0
- Port to .NET6

### v1.11.0

- Dependency Updates
- Redis Tests

### v1.10.21

- Added Support for EtlWriter / TestKit for Redis and CouchDB

### v1.10.20

- Improved DEC TestKit
- Reader and Writer support for Redis
- Dependency Updates for Dapper, KubernetesClient, MudBlazor, Twilio, Serilog e.a

### v1.10.15

- Added Guards around RMqSubscriber

### v1.10.14

- Added IBroadcaster<TID>, ICollector<TID>

## October 2021

### v1.10.8

- Added Extension Methods to create valid IDs: ID.FromAnyString(), ID.FromDecimal()
- Full support for RabbitMQ Requesters, Responders, Emitters and Subscribers
- Initiated support for Redis (using the StackExchange.Redis library)
- initiated support for Eventstore Subscrptions and -Replay
- Updated Dependencies

### v1.10.4

- initiated support for RabbitMQ in M5x.DEC.Infra
- updated dependencies

### v1.10.3

- moved to github.com and publishes now to Nuget.Org
- Updated Dependencies

## September 2021

### v1.9.20-experimental

- Corrected Bug in STAN(Async)Responder that checked for topic on command type instead of THope

### v1.9.19-experimental

- Added Retry support to STANRequester

### v1.9.18-experimental

- Made Hope to Command Transformation the responsibility of the IActor instead of the Responder

### v1.9.17-experimental

- Refactored Xml Utils
- Added Environment Variable Support for Serilog
- Updated Neo4j Dependency
- Added M5x.NBomber Dependency Guard

### 1.9.10-experimental

- Retry in EventStore
- improved DEC.TestKit

### v1.9.7-stable

- Updates: EventFlow, EventStore

### v1.9.6-stable

- Added Polly Support for DEC.Infra

## July 2021

- Added MongoDB Support
- Updated Eventstore Client Base SDK

## October 2020

- Added M5x.PubSub

## May 2020

- Release v1.0.0