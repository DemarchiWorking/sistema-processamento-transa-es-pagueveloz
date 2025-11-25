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

    public class ContaTests_Operacao
    {
        [Fact]
        public void Credit_DeveAumentarSaldo_QuandoOperacaoValida()
        {
            //arrange
            var conta = Conta.CriarNova("ACC-01", "BRL");
            long valorCredito = 10000;

            //act
            conta.Credit(valorCredito, "TX-01");

            //assert
            conta.SaldoDisponivelEmCentavos.Should().Be(10000);
            conta.Transacoes.Should().Contain(t => t.ReferenceId == "TX-01");
        }

        [Fact]
        public void Credit_DeveLancarExcecao_SeTransacaoDuplicada()
        {
            //arrange
            var conta = Conta.CriarNova("ACC-01", "BRL");
            conta.Credit(100, "TX-DUPLICADA");

            //act
            Action act = () => conta.Credit(100, "TX-DUPLICADA");

            //assert
            act.Should().Throw<TransacaoJaProcessadaException>();
        }

        [Fact]
        public void Debit_DeveDiminuirSaldo_QuandoHaSaldoSuficiente()
        {
            //arrange
            var conta = Conta.CriarNova("ACC-01", "BRL");
            conta.Credit(20000, "TX-INIT");

            //act
            conta.Debit(5000, "TX-DEBIT-01");

            //assert
            conta.SaldoDisponivelEmCentavos.Should().Be(15000);
        }

        [Fact]
        public void Debit_DeveUsarLimite_QuandoSaldoZero()
        {
            //arrange
            var conta = Conta.CriarNova("ACC-01", "BRL", 50000);

            //act
            conta.Debit(10000, "TX-LIMIT-01");

            //assert
            conta.SaldoDisponivelEmCentavos.Should().Be(-10000);
            conta.PoderDeCompra.Should().Be(40000);
        }

        [Fact]
        public void Debit_NaoDeveProcessar_QuandoExcedePoderDeCompra()
        {
            //arrange
            var conta = Conta.CriarNova("ACC-01", "BRL", 1000);

            //act
            conta.Debit(2000, "TX-FAIL");

            //assert
            conta.SaldoDisponivelEmCentavos.Should().Be(0);
            conta.Transacoes.Should().NotContain(t => t.ReferenceId == "TX-FAIL");
        }

    }
}