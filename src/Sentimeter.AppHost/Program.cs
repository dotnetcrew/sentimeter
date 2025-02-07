using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var webApi = builder.AddProject<Sentimeter_Web_Api>("sentimeter-webapi");

var webApp = builder.AddProject<Sentimeter_Web_App>("sentimeter-webapp")
    .WithReference(webApi)
    .WaitFor(webApi);

var analysisWorker = builder.AddProject<Sentimeter_Analysis_Worker>("sentimeter-analysis-worker")
    .WaitFor(webApp);

builder.Build().Run();
