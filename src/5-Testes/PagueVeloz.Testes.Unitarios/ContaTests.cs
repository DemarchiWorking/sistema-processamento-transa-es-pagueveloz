using FluentAssertions;
using PagueVeloz.CoreFinanceiro.Dominio.Entidades;
using PagueVeloz.CoreFinanceiro.Dominio.Enums;
using PagueVeloz.CoreFinanceiro.Dominio.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PagueVeloz.Testes.Unitarios
{
    public class ContaTests
    {
        [Fact]
        public void CriarNova_DeveCriarContaValida_ComParametrosCorretos()
        {
            //arrange
            var accountId = "ACC-001";
            var currency = "BRL";
            long limite = 50000;

            //act
            var conta = Conta.CriarNova(accountId, currency, limite);

            //assert
            conta.Should().NotBeNull();
            conta.Status.Should().Be("active");
            conta.SaldoDisponivelEmCentavos.Should().Be(0);
            conta.LimiteDeCreditoEmCentavos.Should().Be(limite);
            conta.PoderDeCompra.Should().Be(limite); // 0 saldo + 50000 limite
        }

        [Theory]
        [InlineData("", "BRL")]
        [InlineData(null, "BRL")]
        [InlineData("ACC-001", "")]
        public void CriarNova_DeveLancarExcecao_QuandoParametrosInvalidos(string id, string curr)
        {
            //act
            Action act = () => Conta.CriarNova(id, curr);

            //assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CriarNova_DeveLancarExcecao_QuandoLimiteNegativo()
        {
            //act
            Action act = () => Conta.CriarNova("ACC-01", "BRL", -100);

            //assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Limite não pode ser negativo*");
        }
    }
}