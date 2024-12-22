var builder = WebApplication.CreateBuilder(args);

// Agregar servicios necesarios para los controladores
builder.Services.AddControllers();

// Configuración de logging para consola
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Opcional: Agregar compatibilidad con Swagger para documentación
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar el middleware para el entorno de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Usar HTTPS Redirection y autorización (si es necesario)
app.UseHttpsRedirection();
app.UseAuthorization();

// Mapear los controladores
app.MapControllers();

app.Run();