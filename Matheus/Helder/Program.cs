using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>();
var app = builder.Build();

//POST: /api/folha/cadastrar
app.MapPost("/api/folha/cadastrar", ([FromBody] Folha? folha, [FromServices] AppDbContext ctx) =>
{
    if (folha is null)
    {
        return Results.BadRequest("A folha não pode ser nula.");
    }

    if (folha.Mes < 1 || folha.Mes > 12)
    {
        return Results.BadRequest("O mês precisa estar entre 1 e 12.");
    }
    
    if (folha.Ano < 2000)
    {
        return Results.BadRequest("O ano precisa ser maior ou igual que 2000.");
    }

    foreach (Folha folhaCadastrado in ctx.Folhas.ToList())
    {
         if (folhaCadastrado.Cpf == folha.Cpf && folhaCadastrado.Mes == folha.Mes && folhaCadastrado.Ano == folha.Ano)
         {
             return Results.BadRequest("Já existe uma com esse CPF, mês e ano.");
         }
    }

    folha.SalarioBruto = folha.HorasTrabalhadas * folha.ValorHora;

    if (folha.SalarioBruto >= 1903.99 && folha.SalarioBruto <= 2826.65)
    {
      folha.Ir = (folha.SalarioBruto * 0.075) - 142.80;  
    }
    else if (folha.SalarioBruto >= 2826.66 && folha.SalarioBruto <= 3751.05)
    {
      folha.Ir = (folha.SalarioBruto * 0.15) - 354.80;  
    }
    else if (folha.SalarioBruto >= 3751.06 && folha.SalarioBruto <= 4664.68)
    {
      folha.Ir = (folha.SalarioBruto * 0.225) - 636.13;  
    }
    else if (folha.SalarioBruto > 4664.68)
    {
      folha.Ir = (folha.SalarioBruto * 0.275) - 869.36;  
    }
    else
    {
      folha.Ir = 0;  
    }

    if (folha.SalarioBruto <= 1693.72)
    {
      folha.Inss = folha.SalarioBruto * 0.08;  
    }
    else if (folha.SalarioBruto >= 1693.73 && folha.SalarioBruto <= 2822.90)
    {
      folha.Inss = folha.SalarioBruto * 0.09;  
    }
    else if (folha.SalarioBruto >= 2822.91 && folha.SalarioBruto <= 5645.80)
    {
      folha.Inss = folha.SalarioBruto * 0.11;  
    }
    else
    {
      folha.Inss = 621.03;  
    }

    folha.Fgts = folha.SalarioBruto * 0.08;
    folha.SalarioLiquido = folha.SalarioBruto - folha.Ir - folha.Inss;

    ctx.Folhas.Add(folha);
    ctx.SaveChanges();
    return Results.Created("", folha);
});

//GET: /api/folha/listar
app.MapGet("/api/folha/listar", ([FromServices] AppDbContext ctx) =>
{
    if (ctx.Folhas.Count() == 0)
    {
        return Results.BadRequest("A lista de folhas está vazia.");
    }
    return Results.Ok(ctx.Folhas.ToList());
});

//GET: /api/folha/buscar/{cpf}/{mes}/{ano}
app.MapGet("/api/folha/buscar/{cpf}/{mes}/{ano}", ([FromRoute] string cpf,  int mes, int ano, [FromServices] AppDbContext ctx) =>
{
    Folha? folhaEncontrado = ctx.Folhas.FirstOrDefault(x => x.Cpf == cpf && x.Mes == mes && x.Ano == ano);
    if (folhaEncontrado is null)
    {
        return Results.NotFound("Folha não encontrada");
    }
    return Results.Ok(folhaEncontrado);
});

//DELETE: /api/folha/remover/{cpf}/{mes}/{ano}
app.MapDelete("/api/folha/remover/{cpf}/{mes}/{ano}", ([FromRoute] string cpf,  int mes, int ano, [FromServices] AppDbContext ctx) =>
{
    Folha? folhaEncontrado = ctx.Folhas.FirstOrDefault(x => x.Cpf == cpf && x.Mes == mes && x.Ano == ano);
    if (folhaEncontrado is null)
    {
        return Results.NotFound("Folha não encontrada");
    }
    ctx.Folhas.Remove(folhaEncontrado);
    ctx.SaveChanges();
    return Results.Ok(folhaEncontrado);  
});

//GET:  /api/folha/total-liquido
app.MapGet("/api/folha/total-liquido", ([FromServices] AppDbContext ctx) =>
{
    if (ctx.Folhas.Count() == 0)
    {
        return Results.BadRequest("A lista de folhas está vazia.");
    }

    double totalLiquido = 0.0; 

    foreach (Folha folhaCadastrado in ctx.Folhas.ToList())
    {
        totalLiquido += folhaCadastrado.SalarioLiquido;
    }

    return Results.Ok(Math.Round(totalLiquido, 2));
});

app.Run();
