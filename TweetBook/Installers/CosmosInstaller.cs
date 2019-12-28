using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cosmonaut;
using Cosmonaut.Extensions.Microsoft.DependencyInjection;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TweetBook.Domain;

namespace TweetBook.Installers
{
    public class CosmosInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var cosmosStoreSettings = new CosmosStoreSettings(
                    configuration["CosmosSetting:DatabaseName"],
                    configuration["CosmosSetting:AccountUri"],
                    configuration["CosmosSetting:AccountKey"],
                    new ConnectionPolicy { ConnectionMode = ConnectionMode.Direct, ConnectionProtocol = Protocol.Https });

            services.AddCosmosStore<CosmosPostDto>(cosmosStoreSettings);
        }
    }
}
