using System.Text.Json.Serialization;
using Elastic.Apm.AspNetCore.DiagnosticListener;
using Elastic.Apm.DiagnosticSource;
using Elastic.Apm.EntityFrameworkCore;
using Elastic.Apm.Extensions.Hosting;
using Elastic.Apm.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using WebApiLvl2.Constants;
using WebApiLvl2.Helpers;
using WebApiLvl2.ParameterTransformers;
using WebApiLvl2.Repositories;
using WebApiLvl2.SchemaFilters;


var builder = WebApplication.CreateBuilder(args);

#region Dependencies

builder.Host.UseElasticApm(
    new HttpDiagnosticsSubscriber(),
    new AspNetCoreDiagnosticSubscriber(),
    new EfCoreDiagnosticsSubscriber(),
    new SqlClientDiagnosticSubscriber()
);

builder.Services
    .AddControllers(o => o.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer())))
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = new HeaderApiVersionReader("X-Api-Version");
});
builder.Services.AddVersionedApiExplorer(o => o.SubstituteApiVersionInUrl = true);
builder.Services.AddSwaggerGen(o =>
{
    o.SchemaFilter<EnumSchemaFilter>();

    var descriptionProvider = builder.Services
        .BuildServiceProvider()
        .GetRequiredService<IApiVersionDescriptionProvider>();

    foreach (var description in descriptionProvider.ApiVersionDescriptions)
    {
        o.SwaggerDoc(description.GroupName, new OpenApiInfo
        {
            Title = description.ApiVersion.ToString(),
            Version = description.ApiVersion.ToString(),
        });
    }
});

builder.Services.AddCors(o =>
    o.AddPolicy(
        CorsConstant.LocalhostOnly,
        b => b.WithOrigins("http://localhost")
    )
);


builder.Services.AddScoped<AuthorRepository, AuthorRepository>();
builder.Services.AddScoped<BookRepository, BookRepository>();
builder.Services.AddScoped<LuckHelper, LuckHelper>();

#endregion

var app = builder.Build();

#region Pipeline

app.UseCors(CorsConstant.LocalhostOnly);

app.UseSwagger();
app.UseSwaggerUI(o =>
{
    var descriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    foreach (var description in descriptionProvider.ApiVersionDescriptions)
    {
        o.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
    }
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

#endregion

app.Run();