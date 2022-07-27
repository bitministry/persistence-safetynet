using Microsoft.Extensions.Configuration;
using Turnit.GenericStore.WebApi.Common;

namespace Turnit.GenericStore.WebApi.CatalogHost
{
    public class Startup : StartupBase
    {
        public Startup(IConfiguration configuration) : base(configuration)
        {
        }
    }
}