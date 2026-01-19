using ConsumerViaCep.Domain.Interfaces;
using ConsumerViaCep.Domain.Models;
using ConsumerViaCep.Infrastructure.Services;
using Flurl.Http;
using Flurl.Http.Testing;
using NUnit.Framework;
using FluentAssertions;

namespace ConsumerViaCep.Tests;

[TestFixture]
public class CepServiceTests
{
    private ICepService _sut;

    [SetUp]
    public void Setup()
    {
        _sut = new CepService();
    }

    [Test]
    public async Task GetAddressByCepAsync_ShouldReturnCorrectData_WhenCepIsValid()
    {
        using var httpTest = new HttpTest();

        // Arrange
        var mockResponse = new ViaCepResponse
        {
            Cep = "01001-000",
            Logradouro = "Praça da Sé",
            Localidade = "São Paulo"
        };
        httpTest.RespondWithJson(mockResponse);

        // Act
        var result = await _sut.GetAddressByCepAsync("01001000");

        // Assert
        result.Should().NotBeNull();
        result.Cep.Should().Be("01001-000");
        result.Localidade.Should().Be("São Paulo");

        httpTest.ShouldHaveCalled("https://viacep.com.br/ws/01001000/json")
                .WithVerb(HttpMethod.Get);
    }

    [Test]
    public async Task GetAddressByCepAsync_ShouldHandleNotFoundError()
    {
        using var httpTest = new HttpTest();

        // Arrange: ViaCep retorna 200 OK com campo "erro: true" para CEPs inexistentes
        httpTest.RespondWithJson(new { erro = true });

        // Act
        var result = await _sut.GetAddressByCepAsync("99999999");

        // Assert
        result.Erro.Should().BeTrue();
    }

    [Test]
    public void GetAddressByCepAsync_ShouldThrowException_WhenCepIsEmpty()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () =>
            await _sut.GetAddressByCepAsync(""));
    }

    [Test]
    public async Task GetAddressByCepAsync_ShouldRetryOnInternalServerError()
    {
        using var httpTest = new HttpTest();

        // Arrange: Simula uma falha 500
        httpTest.RespondWith("Internal Server Error", 500);

        // Act & Assert
        Func<Task> act = async () => await _sut.GetAddressByCepAsync("01001000");
        await act.Should().ThrowAsync<FlurlHttpException>();
    }

    [Test]
    public async Task GetAddressByCepAsync_ShouldHaveCorrectUrlStructure()
    {
        using var httpTest = new HttpTest();
        httpTest.RespondWithJson(new { });

        await _sut.GetAddressByCepAsync("04101300");

        // Verifica se a URL foi montada exatamente como a API exige
        httpTest.ShouldHaveCalled("*/04101300/json");
    }
}