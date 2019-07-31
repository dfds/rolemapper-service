namespace K8sJanitor.WebApi.Infrastructure.AWS
{
    public class IAM
    {
        public static string ConstructRoleArn(string accountId, string roleName)
        {
            return $"arn:aws:iam::{accountId}:role/{roleName}";
        }
    }
}