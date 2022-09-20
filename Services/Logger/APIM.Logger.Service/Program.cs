using APIM.Logger.Service.Middlewares;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(workerApplication =>
    {
        // Register our custom middlewares with the worker.
        workerApplication.UseMiddleware<AuthMiddleware>();
    })
    .Build();

host.Run();
