using ConsumerViaCep.Application.Interfaces;
using ConsumerViaCep.Application.Services;
using ConsumerViaCep.Domain.Interfaces;
using ConsumerViaCep.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();

// Configurando Logging para sair no Console
services.AddLogging(configure => configure.AddConsole());

services.AddScoped<ICepService, CepService>();
services.AddScoped<ICepAppService, CepAppService>();

var serviceProvider = services.BuildServiceProvider();
var appService = serviceProvider.GetRequiredService<ICepAppService>();

// Global Exception Handling
try
{
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)); // Timeout de 5s

    Console.Write("Digite o CEP: ");
    var input = Console.ReadLine();

    var result = await appService.BuscarEnderecoPorCepAsync(input, cts.Token);

    if (result != null)
        Console.WriteLine($"Endereço: {result.Logradouro}");
}
catch (OperationCanceledException)
{
    Console.WriteLine("Erro: A operação demorou demais e foi cancelada.");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Erro de Validação: {ex.Message}");
}
catch (Exception ex)
{
    // Aqui capturamos qualquer erro não tratado (Global Handling)
    Console.WriteLine("Ocorreu um erro inesperado no sistema. Contate o suporte.");
    // O erro completo já foi logado pelo ILogger na Application
}