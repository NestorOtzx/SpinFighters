var builder = WebApplication.CreateBuilder(args);

// Configura servicios necesarios para controladores
builder.Services.AddControllers();

var app = builder.Build();

// Configura el enrutamiento de controladores
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
