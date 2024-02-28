using APIAutomationTestingServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Register HttpClient
builder.Services.AddHttpClient();

//Register Services
builder.Services.RegisterService();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});


var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
//app.UseSwagger();
//app.UseSwaggerUI();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Automation Testing V1");
    c.RoutePrefix = string.Empty; // Set the URI root to the application root
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
