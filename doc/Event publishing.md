# K8s-Janitor - Event publishing

## Intro

This document is written with the purpose of helping to understand how *K8s-janitor* works with events in regard to Kafka and how it emits these events.

In order to create your own event and make it usable in K8s-janitor, you gotta do the following:

- Create a class that inherits from *IEvent*. This class will be where you store the data you're going to send/receive with the event. Ensure it implements the necessary constructor, more details on this can be found below **INSERT LINK HERE**.

- Create an EventHandler class for the previously created class. It must inherit from *IEventHandler*. 

  - ##### Even though this document's goal is event publishing, due to how the events are currently handled, you must have an EventHandler, even if you don't consume it.

- Register these two classes in *ConfigureDomainEvents()* under *Startup.cs* 

- Now use the event as you see fit. Publishing said event is explained in great detail further below.

**Table of content**

- [How to create an Event - Step by step](#how-to-create-an-event---step-by-step)
  * [1. Create Event class](#1-create-event-class)
  * [2. Create EventHandler class](#2-create-eventhandler-class)
  * [3. Register Event and EventHandler](#3-register-event-and-eventhandler)
- [How to publish an Event](#how-to-publish-an-event)
## How to create an Event - Step by step

### 1. Create Event class

Firstly, a class that acts as one's **event** must be created. It could look like this:

```c#
using System;

namespace K8sJanitor.WebApi.Domain.Events
{
    public class K8sNamespaceCreatedAndAwsArnConnectedEvent : IEvent
    {
        
        public string NamespaceName { get; }
        public Guid ContextId { get;  }

        public K8sNamespaceCreatedAndAwsArnConnectedEvent(string namespaceName, Guid contextId)
        {
            NamespaceName = namespaceName;
            ContextId = contextId;
        }

        public K8sNamespaceCreatedAndAwsArnConnectedEvent()
        {
            
        }

        public K8sNamespaceCreatedAndAwsArnConnectedEvent(GeneralDomainEvent domainEvent)
        {
            
        }
    }
}
```

*path*(File found at that path may not reflect the current version): `src/K8sJanitor.WebApi/Domain/Events/K8sNamespaceCreatedAndAwsArnConnectedEvent.cs`

Now, you may have noticed that there is quite a few constructors for that class.

1. (string namespaceName, Guid contextId) - [Optional]

   Used for convenience when creating, not necessary.

2. ()

   Used for convenience, not necessary. - [Optional]

3. (GeneralDomainEvent domainEvent) - [**MUST**]

   This is used for the event consumption loop, **is necessary**. Failure to include this constructor will result in runtime error when said event makes its way to the consumption loop.

*NamespaceName* and *ContextId* will be included in the Event payload when published.

### 2. Create EventHandler class

Now a EventHandler class that handles the Event created above, must be created. It could look like this:

```c#
using System.Threading.Tasks;
using K8sJanitor.WebApi.EventHandlers;
using K8sJanitor.WebApi.Repositories.Kubernetes;
using K8sJanitor.WebApi.Services;

namespace K8sJanitor.WebApi.Domain.Events
{
    public class K8sNamespaceCreatedAndAwsArnConnectedEventHandler : IEventHandler<K8sNamespaceCreatedAndAwsArnConnectedEvent>
    {
        private readonly IConfigMapService _configMapService;
        private readonly INamespaceRepository _namespaceRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRoleBindingRepository _roleBindingRepository;

        public K8sNamespaceCreatedAndAwsArnConnectedEventHandler(
            IConfigMapService configMapService,
            INamespaceRepository namespaceRepository,
            IRoleRepository roleRepository,
            IRoleBindingRepository roleBindingRepository
        )
        {
            _namespaceRepository = namespaceRepository;
            _roleRepository = roleRepository;
            _roleBindingRepository = roleBindingRepository;
            _configMapService = configMapService;
        }
        
        public async Task HandleAsync(K8sNamespaceCreatedAndAwsArnConnectedEvent domainEvent)
        {
            // If event "k8s_namespace_created_and_aws_arn_connected" needs to be handled it can be done here.
        }
    }
}
```

*path*(File found at that path may not reflect the current version): `src/K8sJanitor.WebApi/Domain/Events/K8sNamespaceCreatedAndAwsArnConnectedEventHandler.cs`

The EventHandler class **must** inherit and implement *IEventHandler*. This means creating the *HandleAsync* method shown above. Now, this method doesn't have to do anything if the intention with the created Event is only to publish and never consume. As shown in the example above, it merely contains a comment explaining where code could go if one wanted to react upon the consumption of the event.

### 3. Register Event and EventHandler

Now, in order for K8s-Janitor to work with your newly created Event and EventHandler, it must be registered. This is done by doing the following in *Startup.cs* in the root project directory.

```c#
private static void ConfigureDomainEvents(IServiceCollection services)
{
    //
    // Pre-existing code
    //
            
    services.AddTransient<IEventHandler<K8sNamespaceCreatedAndAwsArnConnectedEvent>, K8sNamespaceCreatedAndAwsArnConnectedEventHandler>();

    //
    // Pre-existing code
    //    
    
    eventRegistry.Register(
        eventTypeName: "k8s_namespace_created_and_aws_arn_connected",
        topicName: topic,
        eventHandler: serviceProvider.GetRequiredService<IEventHandler<K8sNamespaceCreatedAndAwsArnConnectedEvent>>()
     );
    services.AddTransient<IEventDispatcher, EventDispatcher>();
}
```

*path*(Example shown above is merely an illustration of how ConfigureDomainEvents could look like, not necessarily how it currently looks): `src/K8sJanitor.WebApi/Startup.cs`

*eventTypeName* Is used when the Event is consumed by something else. E.g. if XYZ application consumes an Event that has "*k8s_namespace_created_and_aws_arn_connected*" as its eventTypeName, it knows to pass the Event to the proper EventHandler, essentially used as a *key*.

## How to publish an Event

One could go about doing this in numerous ways, however if one were to use what is already existing in K8s-janitor, a published Event will always end up going through the *PublishingService*. Below this, there has been assembled a few ways to approach this.

1. ### Create a "helper" service

   With the Event shown above(which is an actual Event in the repo), it's being published through a "helper" service. In this case, it's called *k8sApplicationService* and looks like this:

   ```c#
   namespace K8sJanitor.WebApi.Application
   {
       public class K8sApplicationService : IK8sApplicationService
       {
           private readonly IServiceProvider _serviceProvider;
   
           public K8sApplicationService(IServiceProvider serviceProvider)
           {
               _serviceProvider = serviceProvider;
           }
   
           public async Task FireEventK8sNamespaceCreatedAndAwsArnConnected(string namespaceName, Guid contextId)
           {
               var eventRegistry = _serviceProvider.GetRequiredService<DomainEventRegistry>();
               var eventsQueue = _serviceProvider.GetRequiredService<PublishingEventsQueue>();
   
               var evtPre = new K8sNamespaceCreatedAndAwsArnConnectedEvent(namespaceName, contextId);
               
               var evt = new DomainEventEnvelope
               {
                   EventId = Guid.NewGuid(),
                   Created = DateTime.UtcNow,
                   Type = eventRegistry.GetTypeNameFor(evtPre),
                   Format = "application/json",
                   Data = JsonConvert.SerializeObject(evtPre, new JsonSerializerSettings
                   {
                       ContractResolver = new CamelCasePropertyNamesContractResolver()
                   })
               };
               
               eventsQueue.AddEventToQueue(evt);
           }
       }
   }
   ```

   It utilitises two other services, *DomainEventRegistry* and *PublishingEventsQueue*. More info will be provided about those two in the 2nd option below. Everything shown in *FireEventK8sNamespaceCreatedAndAwsArnConnect()* is what is necessary in order to create an Event, populate it with the necessary data, and forward it to the *PublishingEventsQueue*. *PublishingService* will take care of publishing the Event as long as it ends up in the queue.

   With *K8sApplicationService* available, and registered in *Startup.cs* e.g.:

   ```c#
   private static void ConfigureDomainEvents(IServiceCollection services)
   {
       services.AddTransient<IK8sApplicationService, K8sApplicationService>();
   }
   ```

   Assuming that is done, one can simply inject *K8sApplicationService* into other services, or wherever ASP.NET Core DI is available in the application, e.g.:

   Using an IServiceProvider:

   ```c#
   public class Di01
   {
   	private readonly IServiceProvider _serviceProvider;
   	
   	public Di01(IServiceProvider serviceProvider)
   	{
   		_serviceProvider = serviceProvider;
   	}
   	
   	void GetK8sApplicationService()
   	{
   		var k8sApp = _serviceProvider.GetRequiredService<IK8sApplicationService>();
   		
   		k8sApp.FireEventK8sNamespaceCreatedAndAwsArnConnect("dummytext", "aa051fec-f422-40c7-8b90-7940fc958209");
   	}
   }
   ```

   Directly injecting into a Class

   ```c#
   public class Di02
   {
   	private readonly IK8sApplicationService _k8sApplicationService;
   	
   	public Di02(IK8sApplicationService k8sApplicationService)
   	{
   		_k8sApplicationService = k8sApplicationService;
   	}
   	
   	void DoWork()
   	{
   		_k8sApplicationService.FireEventK8sNamespaceCreatedAndAwsArnConnect("dummytext", "aa051fec-f422-40c7-8b90-7940fc958209");
   	}
   }
   ```

   

2. ### Manually push Event directly into EventQueue

   Doing it without a "helper" service as mentioned above would mean creating something similiar, or doing it manually each time. That would entail essentially everything shown in the *FireEventK8sNamespaceCreatedAndAwsArnConnected* method, including having to inject the necessary services into whatever place you may want to use the event, these services being *DomainEventRegistry* and *PublishingEventsQueue*.