using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PagueVeloz.CoreFinanceiro.Dominio.DTOs.Request
{
    public class TransacaoFinanceiraRequest
    {
        [JsonPropertyName("operation")]
        [Required(ErrorMessage = "O operação é obrigatório.")]
        public string Operation { get; set; } = string.Empty;

        [JsonPropertyName("origin_account_id")]
        [Required(ErrorMessage = "O id da conta remetente é obrigatório.")]
        public string OriginAccountId { get; set; } = string.Empty;

        [JsonPropertyName("destination_account_id")]
        [Required(ErrorMessage = "O id da conta destino é obrigatório.")]
        public string DestinationAccountId { get; set; } = string.Empty;

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
