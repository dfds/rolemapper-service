using System;

namespace K8sJanitor.WebApi.Infrastructure.Messaging
{
    public class MessagingException : Exception
    {
        public MessagingException(string message) : base(message)
        {
        }
    }
}