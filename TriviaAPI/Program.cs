using TriviaAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy",
        builder =>
        {
            builder.WithOrigins("http://127.0.0.1:3000", "https://localhost:7114")
               .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});
builder.Services.AddSignalR();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseCors("MyPolicy");

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<TriviaHub>("/trivia").RequireCors("MyPolicy");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseRouting();



app.Run();
