using Microsoft.Extensions.Configuration;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

#region OLlama
var ollama = builder.AddOllama("sentimeter-ollama")
    .WithBindMount("../../volumes/ollama", "/root/.ollama")
    .WithOpenWebUI(rb => rb.WithBindMount("../../volumes/ollama-openwebui", "/app/backend/data"));

bool useGpu = builder.Configuration.GetValue<bool>("Ollama:UseGpu",false);
if(useGpu)
{
    ollama.WithGPUSupport();
}

var llama3 = ollama.AddModel("sentimter-llama3", "llama3.2:1b");
#endregion

#region Database
var postgres = builder.AddPostgres("sentimeter-postgres")
    .WithPgAdmin()
    .WithDataBindMount("../../volumes/postgres");

var db = postgres.AddDatabase("sentimeter-db");
#endregion

#region Identity
var identity = builder.AddKeycloak("sentimeter-identity", 9999)
    .WithDataBindMount("../../volumes/identity")
    .WithRealmImport("./Realms");
#endregion

#region Web Api
var webApi = builder.AddProject<Sentimeter_Web_Api>("sentimeter-webapi")
    .WithReference(identity)
    .WaitFor(identity)
    .WithReference(db)
    .WaitFor(db);
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
    .WaitFor(db);
#endregion

#region Support Migration Worker
builder.AddProject<Sentimeter_Support_Migration_Worker>("sentimeter-support-migration-worker")
    .WithReference(db)
    .WaitFor(db);
#endregion

builder.Build().Run();
