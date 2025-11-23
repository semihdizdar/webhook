using Aspire.Hosting;
using Microsoft.Extensions.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// WebApi’yi register et
var webApi = builder.AddProject<WebApi>("webapi");


builder.Build().Run();