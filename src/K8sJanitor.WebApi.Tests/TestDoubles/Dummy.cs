using Moq;

namespace K8sJanitor.WebApi.Tests.TestDoubles
{
    public static class Dummy
    {
        public static T Of<T>() where T : class
        {
            return new Mock<T>().Object;
        }
    }
}