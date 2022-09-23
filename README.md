![](https://discomco.pl/img/discomco-favicon.png)
<br>

# M5x SDK

## Important Announcement
> Starting in 2001, this SDK has been a useful and loyal companion. 
> Over time, it has seen many incarnations.
> Though it may still be of use, we are going to make the effort to port the parts that support ES/CQRS to a new library that will be implemented in F#.
> Keep an eye on the [dot-cart](https://github.com/discomco/dot-cart) repository!

## Introduction

This is an opinionated library of various tools and classes that we have been carrying around from project to project
since 2001; updating as we move along. Check the [Releases](RELEASES.MD).

### Motivation

- As **a Software Engineer**
- I need a **Library of Tools and Wrappers**
- That **support me in the Development of Distributed Systems**
- And that **allows me to centralise dependency management**

## M5x.DEC

The M5x.DEC.* namespace contains an SDK that helps with the creation of systems that are based on
**D**omain Driven Design - **E**vent Sourcing and **C**ommand Query Responsibility Segregation using Clean Architecture
with the Event/Command Handler ("mediator") pattern. 
There is built in support for **EventStoreDB**, **CouchDB**, **Redis**, **NATS** and **RabbitMQ**.

## RoadMap

### EventFlow

The current DEC.* implementation has many similarities with the
awesome [EventFlow](https://github.com/eventflow/EventFlow) package. We are investigating the possibility to integrate
it into the current programming model. The M5x.CEQS.* namespace is a first attempt.

### MediatR

The current DEC.* implementation uses its own *IDECBus* to implement the "mediator" pattern. We investigate te impact of
using [MediatR](https://github.com/jbogard/MediatR) instead.









