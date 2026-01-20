using ConsumerViaCep.Application.Interfaces;
using ConsumerViaCep.Domain.Interfaces;
using ConsumerViaCep.Domain.Models;
using System.Text.RegularExpressions;

namespace ConsumerViaCep.Application.Services;

public class CepAppService : ICepAppService
{
    private readonly ICepService _cepInfrastructureService;

    // Injeção de dependência da camada de Infrastructure (via interface do Domain)
    public CepAppService(ICepService cepInfrastructureService)
    {
        _cepInfrastructureService = cepInfrastructureService;
    }

    public async Task<ViaCepResponse?> BuscarEnderecoPorCepAsync(string? cep)
    {
        // 1. Regra de Negócio: Limpeza de string
        if (string.IsNullOrWhiteSpace(cep))
            return null;

        var cepLimpo = Regex.Replace(cep, @"[^\d]", "");

        // 2. Regra de Negócio: Validação de formato antes de gastar banda de rede
        if (cepLimpo.Length != 8)
            throw new ArgumentException("O CEP deve conter exatamente 8 dígitos.");

        // 3. Orquestração: Chama a infraestrutura
        var resultado = await _cepInfrastructureService.GetAddressByCepAsync(cepLimpo);

        // 4. Lógica Adicional: Tratamento do retorno da API
        if (resultado == null || resultado.Erro)
        {
            // Poderia logar aqui ou retornar um objeto customizado
            return null;
        }

        return resultado;
    }
}
