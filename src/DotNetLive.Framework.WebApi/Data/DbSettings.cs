using Npgsql;

namespace DotNetLive.Framework.WebApi.Data
{
    public class DbSettings
    {
        public string QueryDbConnectionString { get; set; }
        public string CommandDbConnectionString { get; set; }
    }
}
