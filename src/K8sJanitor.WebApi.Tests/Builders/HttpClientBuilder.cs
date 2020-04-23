using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace K8sJanitor.WebApi.Tests.Builders
{
    public class HttpClientBuilder : IDisposable
    {
        private readonly LinkedList<IDisposable> _disposables = new LinkedList<IDisposable>();
        private readonly Dictionary<Type, ServiceDescriptor> _serviceDescriptors = new Dictionary<Type, ServiceDescriptor>();

        public HttpClientBuilder WithService(Type serviceType, object serviceInstance)
        {
            _serviceDescriptors.Remove(serviceType);
            _serviceDescriptors.Add(serviceType, ServiceDescriptor.Singleton(serviceType, serviceInstance));

            return this;
        }

        public HttpClientBuilder WithService<TService>(TService serviceInstance)
        {
            return WithService(typeof(TService), serviceInstance);
        }

        private IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder()
                          .UseStartup<FakeStartup>()
                          .UseSetting(WebHostDefaults.ApplicationKey, typeof(Program).Assembly.FullName)
                          .ConfigureServices(services => {
                              foreach (var descriptor in _serviceDescriptors)
                              {
                                  services.Add(descriptor.Value);
                              }
                          });
        }

        private List<Action<HttpClient>> CreateCustomizations()
        {
            var customizations = new List<Action<HttpClient>>();

            customizations.Add(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(1);
            });

            return customizations;
        }

        public HttpClient Build()
        {
            var webHostBuilder = CreateWebHostBuilder();
            var testServer = new TestServer(webHostBuilder);
            _disposables.AddLast(testServer);

            var customizations = CreateCustomizations();
            var client = testServer.CreateClient();
            customizations.ForEach(x => x(client));
            _disposables.AddLast(client);

            return client;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var instance in _disposables.Reverse())
                {
                    instance.Dispose();
                }
            }
        }
    }
}