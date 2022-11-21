using powerr.Extensions;


var builder = WebApplication.CreateBuilder(args);


var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("powerappdb");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication();

//adds services configured from /extenstions/ServiceExtensions.cs file
builder.Services.ConfigureCors();
builder.Services.ConfigureDbContext(connectionString);
builder.Services.ConfigureJwtAuthentication(configuration);
builder.Services.ConfigureIdentity();

//enable cors
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("power_cors_policy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
