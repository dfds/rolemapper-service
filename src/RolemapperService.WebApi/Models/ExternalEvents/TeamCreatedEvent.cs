using System;

namespace RolemapperService.WebApi.Models.ExternalEvents
{
    public class TeamCreatedEvent
    {
        public TeamCreatedEvent(string teamName, string roleArn, string roleName)
        {
            TeamName = teamName ?? throw new ArgumentException($"{nameof(teamName)} can not be null");
            RoleArn = roleArn ?? throw new ArgumentException($"{nameof(roleArn)} can not be null");
            RoleName = roleName ?? throw new ArgumentException($"{nameof(roleName)} can not be null");
        }

        public string TeamName { get; }
        public string RoleArn { get; }
        public string RoleName { get; }
    }
}