using DictionaryApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Collections.Generic;

var dataService = new ReferenceDataService();
Console.WriteLine("Lookup: 你好");
PrintAll(await dataService.QueryByAnyAsync("你好"));
Console.WriteLine();
    
Console.WriteLine("Reverse lookup: hello");
PrintAll(await dataService.QueryByAnyAsync("hello"));

static void PrintAll(List<TermDefinition> definitions)
{
    int index = 1;
    foreach (var definition in definitions)
    {
        Console.WriteLine($"[{index++}] " + definition.ToDisplayString());
    }
}


/*
var storageConnectionString = "UseDevelopmentStorage=true";// Environment.GetEnvironmentVariable("STORAGE_CONNECTION_STRING");

await Host.CreateDefaultBuilder()
    .UseOrleans(siloBuilder =>
    {
        //siloBuilder.UseKubernetesHosting();
        siloBuilder.Configure<ClusterOptions>(clusterOptions => clusterOptions.ClusterId = clusterOptions.ServiceId = "dictapp");
        siloBuilder.ConfigureEndpoints(11111, 30000, listenOnAnyHostAddress: true);
        siloBuilder.UseAzureStorageClustering(options => options.ConnectionString = storageConnectionString);
        siloBuilder.AddRedisGrainStorage("definitions", redisOptions => redisOptions.DataConnectionString = storageConnectionString);
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton<ReferenceDataService>();
        services.Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromMinutes(2));
    })
    .RunConsoleAsync();
*/
