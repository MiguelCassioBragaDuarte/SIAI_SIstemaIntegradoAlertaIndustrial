using ApiProcessamento.Data;
using ApiProcessamento.Repositories;
using ApiProcessamento.Repositories.Interfaces;
using ApiProcessamento.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. CONFIGURAÇÃO DO BANCO DE DADOS
// Aqui você define onde os dados serão salvos. 
// Exemplo usando SQLite por ser mais fácil de transportar na entrega da SA:
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. REGISTRO DAS DEPENDÊNCIAS (Injeção de Dependência)
// Isso permite que o Controller peça o Service, e o Service peça o Repository.
builder.Services.AddScoped<IAlertaRepository, AlertaRepository>();
builder.Services.AddScoped<IAlertaService, AlertaService>();

// 3. CONFIGURAÇÕES PADRÃO DA API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Essencial para você testar a API visualmente

var app = builder.Build();

// 4. CONFIGURAÇÃO DO PIPELINE DE REQUISIÇÕES (Middleware)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Ativa a interface do Swagger no navegador
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();