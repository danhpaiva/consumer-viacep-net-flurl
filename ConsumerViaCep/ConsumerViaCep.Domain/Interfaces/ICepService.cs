using ConsumerViaCep.Domain.Models;

namespace ConsumerViaCep.Domain.Interfaces;

public interface ICepService
{
    Task<ViaCepResponse> GetAddressByCepAsync(string cep);
}
