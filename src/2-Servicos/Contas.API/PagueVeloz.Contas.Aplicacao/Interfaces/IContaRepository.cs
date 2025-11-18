using PagueVeloz.Contas.Dominio.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Contas.Aplicacao.Interfaces
{
    ///<summary>
    ///abstracao do repositorio de contas.
    ///</summary>
    public interface IContaRepository
    {
        ///<summary>
        ///adiciona uma nova conta ao contexto [Unit of Work].
        ///</summary>
        void Adicionar(Conta conta);
    }
}