using System;
using System.Threading.Tasks;
using K8sJanitor.WebApi.Repositories.Kubernetes;

namespace K8sJanitor.WebApi.Tests.TestDoubles
{
    public class ErrornousRoleBindingRepository : IRoleBindingRepository
    {
        private readonly Exception _exceptionToThrow;

        public Task BindNamespaceRoleToGroup(string namespaceName, string role, string @group)
        {
            throw _exceptionToThrow;
        }

        public ErrornousRoleBindingRepository(Exception exceptionToThrow)
        {
            _exceptionToThrow = exceptionToThrow;
        }
    }
}