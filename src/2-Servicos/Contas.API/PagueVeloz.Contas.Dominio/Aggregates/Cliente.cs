using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Contas.Dominio.Aggregates
{
    ///<summary>
    ///Agregado 'Cliente'.
    ///Representa o dono das contas. Neste contexto, ele é simples,
    ///mas serve como ponto de validação.
    ///</summary>
    public class Cliente
    {
        ///<summary>
        ///id unico do cliente.
        ///</summary>
        public string Id { get; private set; } = string.Empty;

        ///<summary>
        ///nome do cliente.
        ///</summary>
        public string Nome { get; private set; } = string.Empty;

        //construtor privado
        private Cliente() { }

        //metodo de fabrica para criacao
        public static Cliente Criar(string id, string nome)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("id do cliente nao pode ser nulo ou vazio.", nameof(id));

            return new Cliente
            {
                Id = id,
                Nome = string.IsNullOrWhiteSpace(nome) ? "cliente anonimo" : nome
            };
        }
    }
}