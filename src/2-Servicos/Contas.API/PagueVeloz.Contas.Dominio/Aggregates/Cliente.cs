using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagueVeloz.Contas.Dominio.Aggregates
{

    public class Cliente
    {
        public string Id { get; private set; } = string.Empty;
        public string Nome { get; private set; } = string.Empty;

        private Cliente() { }

        public static Cliente Criar(string id, string nome)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("id do cliente nao pode ser nulo ou vazio.", nameof(id));

            return new Cliente
            {
                Id = id,
                Nome = string.IsNullOrWhiteSpace(nome) ? "Cliente Anônimo" : nome 
            };
        }
    }
}