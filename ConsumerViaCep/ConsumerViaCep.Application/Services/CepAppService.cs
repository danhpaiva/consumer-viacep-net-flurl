using ConsumerViaCep.Application.Interfaces;
using ConsumerViaCep.Domain.Interfaces;
using ConsumerViaCep.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace ConsumerViaCep.Application.Services;

public class CepAppService : ICepAppService
{
    private readonly ICepService _cepService;
    private readonly ILogger<CepAppService> _logger;

    // Injeção de dependência da camada de Infrastructure (via interface do Domain)
    public CepAppService(ICepService cepService, ILogger<CepAppService> logger)
    {
        _cepService = cepService;
        _logger = logger;
    }

    public async Task<ViaCepResponse?> BuscarEnderecoPorCepAsync(string? cep, CancellationToken ct = default)
    {
        _logger.LogInformation("Iniciando busca do CEP: {Cep}", cep);

        try
        {
            var cepLimpo = new string(cep?.Where(char.IsDigit).ToArray());

            if (cepLimpo.Length != 8)
            {
                _logger.LogWarning("Tentativa de busca com CEP inválido: {Cep}", cep);
                throw new ArgumentException("CEP inválido.");
            }

            var resultado = await _cepService.GetAddressByCepAsync(cepLimpo, ct);

            if (resultado == null || resultado.Erro)
            {
                _logger.LogInformation("CEP {Cep} não encontrado na base do ViaCep.", cepLimpo);
                return null;
            }

            return resultado;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("A busca do CEP {Cep} foi cancelada.", cep);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico ao buscar o CEP {Cep}", cep);
            throw;
        }
    }
}
