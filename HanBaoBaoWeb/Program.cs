using DictionaryApp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Orleans.Hosting;
using System;

var storageConnectionString = "UseDevelopmentStorage=true";// Environment.GetEnvironmentVariable("STORAGE_CONNECTION_STRING");

await Host.CreateDefaultBuilder()
    .UseOrleans(siloBuilder =>
    {
        //siloBuilder.UseKubernetesHosting();
        siloBuilder.Configure<ClusterOptions>(clusterOptions => clusterOptions.ClusterId = clusterOptions.ServiceId = "dictapp");
        siloBuilder.ConfigureEndpoints(11111, 30000, listenOnAnyHostAddress: true);
        siloBuilder.UseAzureStorageClustering(options => options.ConnectionString = storageConnectionString);
        siloBuilder.AddAzureBlobGrainStorage("definitions", options => options.ConnectionString = storageConnectionString);
        //siloBuilder.AddRedisGrainStorage("definitions", redisOptions => redisOptions.DataConnectionString = storageConnectionString);
    })
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.ConfigureServices(services => services.AddControllers());
        webBuilder.Configure((context, app) =>
        {
            var env = context.HostingEnvironment;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        });
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton<ReferenceDataService>();
        services.Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromMinutes(2));
    })
    .RunConsoleAsync();
