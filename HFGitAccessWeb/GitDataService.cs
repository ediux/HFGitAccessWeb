using System.Data.Services;
using System.Data.Services.Common;
using GitTools;

public class GitDataService : DataService<GitDataSource>
{
    public static void InitializeService(DataServiceConfiguration config)
    {
        config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
        config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2;
    }
}
