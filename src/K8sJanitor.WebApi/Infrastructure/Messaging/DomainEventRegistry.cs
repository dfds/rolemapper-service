using K8sJanitor.WebApi.Domain.Events;
using K8sJanitor.WebApi.EventHandlers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace K8sJanitor.WebApi.Infrastructure.Messaging
{
    public class DomainEventRegistry : IDomainEventRegistry
    {
        private readonly List<DomainEventRegistration> _registrations;
        private readonly Lazy<IEnumerable<IEventHandler>> _eventHandlers;

        protected IEnumerable<IEventHandler> EventHandlers
        {
            get
            {
                return _eventHandlers.Value;
            }
        }

        public DomainEventRegistry(Lazy<IEnumerable<IEventHandler>> eventHandlers = default, IEnumerable<DomainEventRegistration> registrations = default) 
        {
            _eventHandlers = eventHandlers;
            _registrations = registrations?.ToList() ?? new List<DomainEventRegistration>();
        }

        public IDomainEventRegistry Register<TEvent>(string eventTypeName, string topicName) where TEvent : IEvent
        {
            _registrations.Add(new DomainEventRegistration
            {
                EventTypeName = eventTypeName,
                EventType = typeof(TEvent),
                Topic = topicName
            });

            return this;
        }

        public string GetTopicFor(string eventTypeName)
        {
            var registration = _registrations.SingleOrDefault(x => x.EventTypeName == eventTypeName);

            if (registration != null)
            {
                return registration.Topic;
            }

            return string.Empty;
        }

        public IEnumerable<string> GetAllTopics()
        {
            var topics = _registrations.Select(x => x.Topic).Distinct();           

            return topics;
        }

        public Type GetTypeFor(string eventTypeName)
        {
            var registration = _registrations.SingleOrDefault(x => x.EventTypeName == eventTypeName);

            if (registration == null)
            {
                throw new EventTypeNotFoundException($"Error! Could not determine \"event instance type\" due to no registration was found for type {eventTypeName}!");
            }

            return registration.EventType;
        }

        public string GetTypeNameFor(IEvent domainEvent)
        {
            var domainEventType = domainEvent.GetType();
            var registration = _registrations.SingleOrDefault(x => x.EventType == domainEventType);

            if (registration == null)
            {
                throw new MessagingException($"Error! Could not determine \"event type name\" due to no registration was found for type {domainEventType.FullName}!");
            }

            return registration.EventTypeName;
        }

        public IEnumerable<IEventHandler> GetEventHandlersFor(IEvent domainEvent)
        {
            var domainEventType = domainEvent.GetType();
            var handlers = EventHandlers.Where(eventhandler => eventhandler.GetType().GetInterfaces().Any(i => i.GetGenericArguments().Contains(domainEventType)));

            if (handlers == null)
            {
                throw new EventHandlerNotFoundException($"Error! Could not determine \"event handlers\" due to no registration was found for type {domainEventType.FullName}!");
            }

            return handlers;
        }
    }

    public class DomainEventRegistration
    {
        public string EventTypeName { get; set; }

        public Type EventType { get; set; }

        public string Topic { get; set; }
    }
}