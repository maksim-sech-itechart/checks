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
    options.ApiVersionReader = ApiVersionReader.Combine(
        new HeaderApiVersionReader("X-Api-Version"),
        new UrlSegmentApiVersionReader(),
        new QueryStringApiVersionReader("api-version")
    );
});
builder.Services.AddVersionedApiExplorer(o =>
{
    o.SubstitutionFormat = "'v'VVV";
    o.SubstituteApiVersionInUrl = true;
});
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

builder.Services.AddCors(options =>
    {
        options.AddPolicy(
            CorsConstant.LocalhostOnly,
            b => b.WithOrigins("http://localhost")
        );

        options.AddPolicy(
            CorsConstant.Any,
            b => b
                .WithOrigins("*")
                .WithMethods("*")
        );

        options.DefaultPolicyName = CorsConstant.Any;
    }
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

app
    .MapGet(
        "/corsTest",
        async context => await context.Response.WriteAsync("CORS test")
    )
    .RequireCors(CorsConstant.LocalhostOnly);

app.MapControllers();

#endregion

app.Run();