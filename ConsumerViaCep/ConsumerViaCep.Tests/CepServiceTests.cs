using ConsumerViaCep.Domain.Models;
using ConsumerViaCep.Infrastructure.Services;
using Flurl.Http;
using Flurl.Http.Testing;
using Xunit;

namespace ConsumerViaCep.Tests;

public class CepServiceTests : IDisposable
{
    private readonly CepService _sut;
    private readonly HttpTest _httpTest;

    public CepServiceTests()
    {
        _sut = new CepService();
        _httpTest = new HttpTest(); // Intercepta todas as chamadas HTTP durante o teste
    }

    // O xUnit executa o Dispose após cada teste, garantindo que o HttpTest seja limpo
    public void Dispose()
    {
        _httpTest.Dispose();
    }

    [Fact]
    public async Task GetAddressByCepAsync_DeveRetornarDadosCorretos_QuandoCepValido()
    {
        // Arrange
        var mock = new ViaCepResponse
        {
            Cep = "01001-000",
            Logradouro = "Praça da Sé",
            Localidade = "São Paulo"
        };
        _httpTest.RespondWithJson(mock);

        // Act
        var result = await _sut.GetAddressByCepAsync("01001000");

        // Assert (Nativo xUnit)
        Assert.NotNull(result);
        Assert.Equal("01001-000", result.Cep);
        Assert.Equal("São Paulo", result.Localidade);

        // Assert (Nativo Flurl para verificar se a chamada foi correta)
        _httpTest.ShouldHaveCalled("https://viacep.com.br/ws/01001000/json")
                 .WithVerb(HttpMethod.Get);
    }

    [Fact]
    public async Task GetAddressByCepAsync_DeveRetornarErroTrue_QuandoCepNaoExistir()
    {
        // Arrange (ViaCep retorna 200 OK mas com erro: true no corpo)
        _httpTest.RespondWithJson(new { erro = true });

        // Act
        var result = await _sut.GetAddressByCepAsync("99999999");

        // Assert
        Assert.True(result.Erro);
    }

    [Fact]
    public async Task GetAddressByCepAsync_DeveLancarExcecao_QuandoCepForVazio()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _sut.GetAddressByCepAsync(""));
    }

    [Fact]
    public async Task GetAddressByCepAsync_DeveLancarFlurlException_QuandoApiFalhar()
    {
        // Arrange
        _httpTest.RespondWith("Erro Interno", 500);

        // Act & Assert
        await Assert.ThrowsAsync<FlurlHttpException>(() => _sut.GetAddressByCepAsync("01001000"));
    }

    [Fact]
    public async Task GetAddressByCepAsync_DeveRespeitarEstruturaDaUrl()
    {
        // Arrange
        _httpTest.RespondWithJson(new { });

        // Act
        await _sut.GetAddressByCepAsync("12345678");

        // Assert
        _httpTest.ShouldHaveCalled("*/12345678/json");
    }
}