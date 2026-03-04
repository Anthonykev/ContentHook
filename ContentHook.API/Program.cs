using ContentHook.DAL.ORMapper;
using Microsoft.EntityFrameworkCore;
using ContentHook.API.Extensions;
using ContentHook.API.Hubs;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Multipart Body Limit für File-Uploads erhöhen
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 524288000; // 500 MB
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddApplicationServices();
builder.Services.AddSignalR();

// CORS für React Frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//Migration 
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect root URL to Swagger UI
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();


app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("ReactPolicy");   
app.UseAuthorization();

app.MapControllers();

app.MapHub<ProgressHub>("/hubs/progress");

app.Run();
