using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Azure.Identity;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace KeyVaultDemoApp {
    public class Program {
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((context, config) => {
                    var builtConfig = config.Build();
                    var vaultName = builtConfig["VaultName"];
                    string vaultUri = $"https://{vaultName}.vault.azure.net/";
                    var azureServiceTokenProvider = new AzureServiceTokenProvider();
                    var kv = new KeyVaultClient((authority, resource, scope) => azureServiceTokenProvider.KeyVaultTokenCallback(authority, resource, scope));
                    //var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                    config.AddAzureKeyVault(vaultUri, kv, new DefaultKeyVaultSecretManager());

                });
    }
}
