using Microsoft.Extensions.Configuration;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

#region Parameters
var youtubeApiKey = builder.AddParameter("YoutubeApiKey", secret: true);
#endregion

#region OLlama
var ollama = builder.AddOllama("sentimeter-ollama")
    .WithDataVolume("sentimeter-ollama-data")
    .WithOpenWebUI(
        rb =>
        {
            rb.WithDataVolume("sentimeter-openwebui-data");
            rb.WithExplicitStart();
        });

bool useGpu = builder.Configuration.GetValue<bool>("Ollama:UseGpu",false);
if(useGpu)
{
    ollama.WithGPUSupport();
}

var llama3 = ollama.AddModel("sentimeter-llama3", "llama3.2:1b");
#endregion

#region Database
var postgres = builder.AddPostgres("sentimeter-postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin(rb =>
    {
        rb.WithExplicitStart();
        rb.WithLifetime(ContainerLifetime.Persistent);
    })
    .WithDataVolume("sentimeter-postgres-data");

var db = postgres.AddDatabase("sentimeter-db");
#endregion

#region Identity
var identity = builder.AddKeycloak("sentimeter-identity", 9999)
    .WithDataVolume("sentimeter-identity-data")
    .WithRealmImport("./Realms");
#endregion

#region RabbitMQ
var rabbitmq = builder.AddRabbitMQ("messaging")
    .WithExternalHttpEndpoints()
    .WithManagementPlugin();
#endregion

#region Web Api
var webApi = builder.AddProject<Sentimeter_Web_Api>("sentimeter-webapi")
    .WithReference(identity)
    .WithReference(rabbitmq)
    .WaitFor(identity)
    .WithReference(db)
    .WaitFor(db)
    .WaitFor(rabbitmq)
    .WithEnvironment("YOUTUBE_APIKEY", youtubeApiKey);
#endregion

#region Web App
var webApp = builder.AddProject<Sentimeter_Web_App>("sentimeter-webapp")
    .WithReference(webApi)
    .WaitFor(webApi)
    .WithReference(identity);
#endregion

#region Analysis Worker
var analysisWorker = builder.AddProject<Sentimeter_Analysis_Worker>("sentimeter-analysis-worker")
    .WithReference(llama3)
    .WaitFor(llama3)
    .WithReference(db)
    .WaitFor(db);
#endregion

#region Data Retrieval Worker
builder.AddProject<Sentimeter_DataRetrieval_Worker>("sentimeter-dataretrieval-worker")
    .WithReference(db)
    .WithReference(rabbitmq)
    .WaitFor(db)
    .WaitFor(rabbitmq)
    .WithEnvironment("YOUTUBE_APIKEY", youtubeApiKey);
#endregion

#region Support Migration Worker
builder.AddProject<Sentimeter_Support_Migration_Worker>("sentimeter-support-migration-worker")
    .WithReference(db)
    .WaitFor(db);
#endregion

builder.Build().Run();
