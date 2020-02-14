using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using k8s.Models;
using K8sJanitor.WebApi.Models;
using K8sJanitor.WebApi.Repositories.Kubernetes;

namespace K8sJanitor.WebApi.Tests.TestDoubles
{
    public class ErrornousNamespaceRepository : INamespaceRepository
    {
        private readonly Exception _exceptionToThrow;
        private readonly AnnotationAlreadyExistWithDifferentValueException _annotationException;


        public ErrornousNamespaceRepository(Exception exceptionToThrow,
            AnnotationAlreadyExistWithDifferentValueException annotationException = null)
        {
            _exceptionToThrow = exceptionToThrow;
            _annotationException = annotationException;
        }
        public Task CreateNamespaceAsync(string namespaceName, string roleName)
        {
            throw _exceptionToThrow;
        }

        public Task CreateNamespaceAsync(NamespaceName namespaceName, IEnumerable<Label> labels)
        {
            throw _exceptionToThrow;
        }

        public Task CreateNamespaceAsync(NamespaceName namespaceName)
        {
            throw _exceptionToThrow;
        }

        public Task AddAnnotations(NamespaceName namespaceName, Dictionary<string, string> annotations)
        {
            if (_annotationException != null)
                throw _annotationException;

            return Task.CompletedTask;
        }

        public Task<IEnumerable<Namespace>> GetAllCapabilityNamespacesAsync()
        {
            throw _exceptionToThrow;
        }
    }
}