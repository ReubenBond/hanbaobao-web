using DictionaryApp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Orleans.Hosting;
using System;


await Host.CreateDefaultBuilder(args)
    .UseOrleans((ctx, siloBuilder) =>
    {
        string storageConnectionString;
        if (ctx.HostingEnvironment.IsDevelopment())
        {
            // For local dev, we use some defaults
            storageConnectionString = "UseDevelopmentStorage=true";
            siloBuilder.Configure<ClusterOptions>(clusterOptions => clusterOptions.ClusterId = clusterOptions.ServiceId = "dictapp");
            siloBuilder.ConfigureEndpoints(11111, 30000, listenOnAnyHostAddress: true);
            siloBuilder.ConfigureServices(services => services.Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromMinutes(2)));
        }
        else
        {
            // In Kubernetes, we use environment variables
            storageConnectionString = Environment.GetEnvironmentVariable("STORAGE_CONNECTION_STRING");
            siloBuilder.UseKubernetesHosting();
        }

        siloBuilder.UseAzureStorageClustering(options => options.ConnectionString = storageConnectionString);
        siloBuilder.AddAzureBlobGrainStorage("definitions", options => options.ConnectionString = storageConnectionString);
    })
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.ConfigureServices(services => services.AddControllers());
        webBuilder.Configure((ctx, app) =>
        {
            if (ctx.HostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
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
    })
    .RunConsoleAsync();
