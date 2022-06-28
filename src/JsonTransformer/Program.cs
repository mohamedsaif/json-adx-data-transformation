using JsonTransformer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var options = new ServerOptions();
builder.Configuration.GetSection(nameof(ServerOptions)).Bind(options);
builder.Services.AddSingleton<ServerOptions>(options);

builder.Services.AddControllers().AddDapr();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddApplicationInsightsKubernetesEnricher();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "JsonTransformer"));
}

app.UseAuthorization();

app.UseCloudEvents();

app.MapControllers();

app.Run();
