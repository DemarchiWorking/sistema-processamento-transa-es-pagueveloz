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
    public class ContaTests_ReservaCaptura
    {
        [Fact]
        public void Reserve_DeveMoverSaldoDeDisponivelParaReservado()
        {
            //arrange
            var conta = Conta.CriarNova("ACC-01", "BRL");
            conta.Credit(1000, "TX-DEP");

            //act
            conta.Reserve(300, "TX-RES-01");

            //assert
            conta.SaldoDisponivelEmCentavos.Should().Be(700);
            conta.SaldoReservadoEmCentavos.Should().Be(300);
            conta.SaldoTotal.Should().Be(1000);
        }

        [Fact]
        public void Capture_DeveConsumirSaldoReservado()
        {
            //arrange
            var conta = Conta.CriarNova("ACC-01", "BRL");
            conta.Credit(1000, "TX-DEP");
            conta.Reserve(300, "TX-RES-01");

            //act
            conta.Capture(300, "TX-CAP-01");

            //asert
            conta.SaldoReservadoEmCentavos.Should().Be(0);
            conta.SaldoDisponivelEmCentavos.Should().Be(700);
            conta.SaldoTotal.Should().Be(700);
        }

        [Fact]
        public void Capture_DeveFalhar_SeNaoHouverReservaSuficiente()
        {
            //arrange
            var conta = Conta.CriarNova("ACC-01", "BRL");
            conta.Credit(1000, "TX-DEP");
            conta.Reserve(100, "TX-RES-01");

            //act
            Action act = () => conta.Capture(200, "TX-FAIL");

            //assert
            act.Should().Throw<DomainException>()
               .WithMessage("*Saldo reservado insuficiente*");
        }
    }
}