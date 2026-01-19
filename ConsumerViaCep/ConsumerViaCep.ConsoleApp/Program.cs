using ConsumerViaCep.Domain.Interfaces;
using ConsumerViaCep.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddSingleton<ICepService, CepService>();

var serviceProvider = services.BuildServiceProvider();

var cepService = serviceProvider.GetService<ICepService>();
var dados = await cepService.GetAddressByCepAsync("01001000");

Console.WriteLine($"Endereço: {dados.Logradouro}, {dados.Bairro} - {dados.Localidade}/{dados.Uf}");