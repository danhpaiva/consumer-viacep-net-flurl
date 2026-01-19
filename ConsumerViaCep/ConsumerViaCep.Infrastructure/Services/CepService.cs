using ConsumerViaCep.Domain.Interfaces;
using ConsumerViaCep.Domain.Models;
using Flurl;
using Flurl.Http;

namespace ConsumerViaCep.Infrastructure.Services;

public class CepService : ICepService
{
    private const string BaseUrl = "https://viacep.com.br/ws";

    public async Task<ViaCepResponse> GetAddressByCepAsync(string cep)
    {
        if (string.IsNullOrWhiteSpace(cep))
            throw new ArgumentException("CEP não pode ser vazio.");

        return await BaseUrl
            .AppendPathSegment(cep)
            .AppendPathSegment("json")
            .GetJsonAsync<ViaCepResponse>();
    }
}
