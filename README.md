# Nike

Nike is a set of libraries designed to implement projects based on domain-driven design and micro-service architecture.

### What concepts are covered in Nike?

* [x]  Entity Framework Core
* [x] Greg Young Event Store
* [x] RabbitMQ
* [x] Kafka
* [x] Event Bus
  * [x] RabbitMQ
  * [ ] NServiceBus
* [x] CQRS + Unit of Work
  * [x] MicroBus
  * [ ] MediatR
* [ ] Saga
* [ ] Open Tracing
* [x] RESTapi Documentation Tool
  * [x] Swagger
  * [ ] RedDoc
  * [ ] OpenAPI-GUI
* [x] Caching
  * [x] Redis
  * [ ] NCache
* [x] DDD
  * [x] Event Sourcing
  * [x] Regular (CRUD)
* [x] Specification Pattern
* [x] Event Aggregator Pattern

Below you can find the instruction to use each library in this project.


## Nike Domain

Nike Domain provides set of base classes to implement DDD in your project. It also supports implementing **Event Sourcing** using DDD.

### What concepts are included:
* DDD
  * Aggregate Root
  * Entity
  * Value Object
  * Domain Event
  * Enumeration
  * Domain Exceptions
  * Snapshot (Event Sourcing)
 * Pattern Implementations
   * Specification
   * Repository
   * Unit of Work
  * Entity Builder *

### What is the Entity Builder? When to use it?

Since domain model properties setters are private and cannot be public, this utility class allows you to set properties ignoring their modifier.
This class can is used to implement the **builder pattern** and in **test scenarios** where mocking properties are needed.

### Installation

Install using NuGet:
```
Install-Package Nike.Framework.Domain
```





## Nike Entity Framework

This library provides extensions to implement DDD using entity framework core.

### Installation

Install using NuGet:
```
Install-Package Nike.EntityFramework
```



In case you are using ASP.NET Core, install the package below:

```
Install-Package Nike.EntityFramework.Microsoft.DependencyInjection
```

Then use it the way shown here:
```
services.AddSqlServerDatabaseContext<ApplicationDbContext>()
		.AddEntityFrameworkDefaultRepository()
		.AddEntityFrameworkUnitOfWork()
```


### What are covered?

* Specification Pattern Application
* Unit of Work Implementation
* Generic Repository + Specification Support
* DbContext Extensions

### Extensions

```c#
SetAllRelationshipDeleteBehavior(this ModelBuilder builder, DeleteBehavior deleteBehavior)
```

This method set delete behavior of all foreign relationships



```c#
DisallowTimestampPropertiesUpdate(this ModelBuilder builder)
```

This method will prevent the update of properties that are provided by IAuditedEntity interface.



```c#
SetTimestampsAutomatically(this DbContext dbContext)
```

This method will automatically update the properties that are provided by IAuditedEntity interface.



```c#
OnEntityCreate(this DbContext dbContext, Action<object, EntityTrackedEventArgs> callback)

OnEntityUpdate(this DbContext dbContext, Action<object, EntityStateChangedEventArgs> callback)
```

Callback methods on entity update and on entity create.



## Nike Event Bus

This library provides abstractions to implement event bus pattern

### Installation

Install using NuGet:

```
Install-Package Nike.EventBus
```



### What are covered?

* IEventBusDispatcher.cs

  ```c#
      public interface IEventBusDispatcher
      {
          Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : notnull, IEventBusMessage;
          Task PublishAsync<T>(T message, string topic, CancellationToken cancellationToken = default) where T : notnull, IEventBusMessage;
      }
  ```

* Unit of Work Implementation

  ```c#
      public interface IEventBusMessage
      {
          Dictionary<string, string> TracingSpan { get; set; }
      } 
  
  ```



**What is TracingSpan in IEventBusMessage.cs?**

It is based on the concept of [Tracing Span Extraction](https://opentracing.io/specification/). It should be set automatically by event bus provider to allow passing tracing span through event bus. 



## Nike Event Bus RabbitMq

This library provides implement the event bus pattern. Its implementation is based on Nike Event Bus. This library has a dependency on ASP.Net Core constructs therefore is only usable in this platform.

### Installation

Install using NuGet:

```
Install-Package Nike.EventBus.RabbitMq
```



Add it to your ConfigureServices:

```c#
services.AddRabbitMq(rabbitMqConnectionString)
```



Add it to your Configure:

```
app.UseRabbitMq() 
```



### What are covered?

* Automatic detection of messages and message handler. It follows the practice of [Auto-Subscriber](https://github.com/EasyNetQ/EasyNetQ/wiki/Auto-Subscriber). In order to make it work you should annotate your message handlers with IConsume/IConsumeAsync interface. These classes are found on application start and are registered into DI container.
* Automatic extraction & injection of tracing spans. Event bus dispatcher extracts current tracing span on sending messages and inject them on receiving a message. (Please read the https://opentracing.io/specification/ to understand the concepts of Injection and Extraction).
* Eventbus dispatcher uses MessagePack which is the fastest and most memory efficient binary serialization library for .NET to Serialize/Deserialize event bus messages.
* Eventbus dispatcher only uses type short name (e.g, OnUserOrder) for its operation not the full name.



## Nike Reactive

This library provides patterns and practices that are implemented using net reactive.

### Installation

Install using NuGet:

```
Install-Package Nike.Reactive
```



### Event Aggregator Pattern

An Event Aggregator is a service which sits between your publishers and subscribers acting as an intermediary pushing your messages (events) from one entity to another. It is mainly used in WPF applications to handle communications between components. It's also used to return any result (e.g, entity id) from commands in CQRS pattern.



**How to add it in ASP.Net Core application:**

Install using NuGet:

```
Install-Package Nike.Reactive.Microsoft.DependencyInjection
```



then add it to your ConfigureServices:

```
services.AddEventAggregator()
```



**How to use it:**

You should inject IEventAggregator to your classes in order to use it. IEventAggregator has the following signature:

```c#
    public interface IEventAggregator : IDisposable
    {
        IDisposable Subscribe<T>(Action<T> action);
        void Publish<T>(T @event);
    }
```

You may can use Publish method to publish any message with any type as intended. However in order to use the Subscribe method, you should call it before the desired message is sent.



## Nike Test Utilities

This library provides utility classes that can be used in both unit and integration testing.

### Installation

Install using NuGet:

```
Install-Package Nike.TestUtilities
```



### Faker.cs

This class exposes many methods and properties to generate random data.

```c#
Build<T>() // creates fake object
RandomString
RandomStringEnumerable
RandomInt
RandomFloat
NewGuid
RandomDecimal
    
and more...
```



### Verify.cs

This class is a extension to Fluent Assertions library and adds the ability to test the arguments of methods that are called (received).



**How to use it: **

```c#
string productName = "product1";

repository.Received(Times.Once).Add(Verify.That<Product>(input => {
    
    input.Name.Should.Be(productName);
    
}));
```



## Nike MicroBus

This library provides extensions to MicroBus. It adds the ability to handle commands based on the unit of work pattern and to handle domain events without  making domain layer dependent on MicroBus.



### Installation

Install using NuGet:

```
Install-Package Nike.MicroBus
```



For asp.net core application install the package below:

```
Install-Package Nike.MicroBus.Microsoft.DependencyInjection
```



### Unit of Work

After sending each command, a delegator is run in background to commit the unit of work that has occurred in command. The delegator also publishes all the domain events that have been raised.

**Note:** if you are using this library, you should not commit changes yourself as it violates the unit of work pattern. For example, calling dbContext.SaveChanges().



### Handling Domain Events:

In order to keep the separation of concerns, this library provides a base class to handle domain events in your application layer:



```
DomainEventHandler<TDomainEvent> : IDomainEventHandler<TDomainEvent> where TDomainEvent : IDomainEvent
```



example:



```c#
public class UserOrderSubmittedDomainEventHandler : DomainEventHandler<UserOrderSubmittedDomainEvent> {
    public override Task HandleAsync(UserOrderSubmittedDomainEvent domainEvent){

        // handle your domain event
        
    }
}
```



**Note:** the handlers are automatically found on application startup.