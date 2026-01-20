using Microsoft.Extensions.DependencyInjection;
using ConsumerViaCep.Domain.Interfaces;
using ConsumerViaCep.Infrastructure.Services;
using ConsumerViaCep.Application.Interfaces;
using ConsumerViaCep.Application.Services;

// 1. Configuração do Container de DI
var serviceProvider = new ServiceCollection()
    .AddScoped<ICepService, CepService>() // Infra no Domain
    .AddScoped<ICepAppService, CepAppService>() // Application
    .BuildServiceProvider();

// 2. Obtendo o serviço da camada de Application
var appService = serviceProvider.GetRequiredService<ICepAppService>();

Console.WriteLine("--- Consultor de CEP (Clean Architecture) ---");
Console.Write("Digite o CEP: ");
var input = Console.ReadLine();

try
{
    var endereco = await appService.BuscarEnderecoPorCepAsync(input ?? "");

    if (endereco != null)
    {
        Console.WriteLine($"\nSucesso: {endereco.Logradouro}, {endereco.Bairro}");
        Console.WriteLine($"{endereco.Localidade} - {endereco.Uf}");
    }
    else
    {
        Console.WriteLine("\nCEP não encontrado.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"\nErro de validação: {ex.Message}");
}