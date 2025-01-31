var builder = WebApplication.CreateBuilder(args);

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSPFx",
        builder => builder
            .WithOrigins("https://366pidev.sharepoint.com") // Allow SharePoint domain
            .AllowAnyMethod()  // Allow all HTTP methods (GET, POST, PUT, DELETE, etc.)
            .AllowAnyHeader()  // Allow all headers
            .AllowCredentials()); // Allow cookies, authorization headers, etc.
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable CORS middleware
app.UseCors("AllowSPFx");

// Enable Swagger for API testing
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
