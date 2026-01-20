using ConsumerViaCep.Domain.Models;

namespace ConsumerViaCep.Application.Interfaces;

public interface ICepAppService
{
    // A Application retorna DTOs ou os próprios modelos de domínio 
    // após aplicar regras de negócio.
    Task<ViaCepResponse?> BuscarEnderecoPorCepAsync(string? cep, CancellationToken ct = default);
}
