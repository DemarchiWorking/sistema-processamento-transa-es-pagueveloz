using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Dominio.DTOs.Request
{
    public class TransacaoRequest
    {
        [JsonPropertyName("operation")]
        [Required(ErrorMessage = "O operação é obrigatório.")]
        public string Operation { get; set; } = string.Empty;

        [JsonPropertyName("account_id")]
        [Required(ErrorMessage = "O id da conta é obrigatório.")]
        public string AccountId { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        [Required(ErrorMessage = "O valor é obrigatório.")]
        public long Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonPropertyName("reference_id")]
        [Required(ErrorMessage = "O Id de referencia é obrigatório [Ex: TXN-01]")]
        public string ReferenceId { get; set; } = string.Empty;

        [JsonPropertyName("metadata")]
        public Dictionary<string, object>? Metadata { get; set; }
    }
}