# Releases

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