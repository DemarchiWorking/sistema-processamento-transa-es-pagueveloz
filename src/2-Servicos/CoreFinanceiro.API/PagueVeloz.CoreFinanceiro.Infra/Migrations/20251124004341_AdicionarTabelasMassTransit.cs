using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PagueVeloz.CoreFinanceiro.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarTabelasMassTransit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Financeiro");

            migrationBuilder.EnsureSchema(
                name: "Infra");

            migrationBuilder.CreateSequence(
                name: "ContaSeq",
                schema: "Financeiro");

            migrationBuilder.CreateTable(
                name: "Contas",
                schema: "Financeiro",
                columns: table => new
                {
                    AccountId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SaldoDisponivelEmCentavos = table.Column<long>(type: "bigint", nullable: false),
                    SaldoReservadoEmCentavos = table.Column<long>(type: "bigint", nullable: false),
                    LimiteDeCreditoEmCentavos = table.Column<long>(type: "bigint", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    row_version = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contas", x => x.AccountId);
                });

            migrationBuilder.CreateTable(
                name: "Domain_OutboxMessages",
                schema: "Infra",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EventType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Payload = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    ProcessedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Attempts = table.Column<int>(type: "integer", nullable: false),
                    NextAttemptDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Error = table.Column<string>(type: "text", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Domain_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MT_InboxState",
                schema: "Infra",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConsumerId = table.Column<Guid>(type: "uuid", nullable: false),
                    LockId = table.Column<Guid>(type: "uuid", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    Received = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReceiveCount = table.Column<int>(type: "integer", nullable: false),
                    ExpirationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Consumed = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Delivered = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MT_InboxState", x => x.Id);
                    table.UniqueConstraint("AK_MT_InboxState_MessageId_ConsumerId", x => new { x.MessageId, x.ConsumerId });
                });

            migrationBuilder.CreateTable(
                name: "MT_OutboxState",
                schema: "Infra",
                columns: table => new
                {
                    OutboxId = table.Column<Guid>(type: "uuid", nullable: false),
                    LockId = table.Column<Guid>(type: "uuid", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Delivered = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MT_OutboxState", x => x.OutboxId);
                });

            migrationBuilder.CreateTable(
                name: "TransacoesProcessadas",
                schema: "Financeiro",
                columns: table => new
                {
                    ReferenceId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<string>(type: "text", nullable: false),
                    DestinationAccountId = table.Column<string>(type: "text", nullable: true),
                    TransactionId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    MessageType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Details = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransacoesProcessadas", x => x.ReferenceId);
                });

            migrationBuilder.CreateTable(
                name: "Movimentos",
                schema: "Financeiro",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    AccountId = table.Column<string>(type: "character varying(50)", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Value = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Coin = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    ReferenceId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TransactionId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    MetadadosJson = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movimentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movimentos_ContaId",
                        column: x => x.AccountId,
                        principalSchema: "Financeiro",
                        principalTable: "Contas",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transacoes",
                schema: "Financeiro",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContaId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Valor = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    ReferenceId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    Metadata = table.Column<string>(type: "text", nullable: true),
                    FinalBalanceInCents = table.Column<long>(type: "bigint", nullable: false),
                    FinalReservedBalanceInCents = table.Column<long>(type: "bigint", nullable: false),
                    FinalAvailableBalanceInCents = table.Column<long>(type: "bigint", nullable: false),
                    IsReversed = table.Column<bool>(type: "boolean", nullable: false),
                    ReversalReferenceId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transacoes_Contas_ContaId",
                        column: x => x.ContaId,
                        principalSchema: "Financeiro",
                        principalTable: "Contas",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MT_OutboxMessage",
                schema: "Infra",
                columns: table => new
                {
                    SequenceNumber = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EnqueueTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SentTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Headers = table.Column<string>(type: "text", nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    InboxMessageId = table.Column<Guid>(type: "uuid", nullable: true),
                    InboxConsumerId = table.Column<Guid>(type: "uuid", nullable: true),
                    OutboxId = table.Column<Guid>(type: "uuid", nullable: true),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    MessageType = table.Column<string>(type: "text", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: true),
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: true),
                    InitiatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    RequestId = table.Column<Guid>(type: "uuid", nullable: true),
                    SourceAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DestinationAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ResponseAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    FaultAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ExpirationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MT_OutboxMessage", x => x.SequenceNumber);
                    table.ForeignKey(
                        name: "FK_MT_OutboxMessage_MT_InboxState_InboxMessageId_InboxConsumer~",
                        columns: x => new { x.InboxMessageId, x.InboxConsumerId },
                        principalSchema: "Infra",
                        principalTable: "MT_InboxState",
                        principalColumns: new[] { "MessageId", "ConsumerId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MT_OutboxMessage_MT_OutboxState_OutboxId",
                        column: x => x.OutboxId,
                        principalSchema: "Infra",
                        principalTable: "MT_OutboxState",
                        principalColumn: "OutboxId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Domain_OutboxMessages_NextAttemptDateUtc",
                schema: "Infra",
                table: "Domain_OutboxMessages",
                column: "NextAttemptDateUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Domain_OutboxMessages_ProcessedOn",
                schema: "Infra",
                table: "Domain_OutboxMessages",
                column: "ProcessedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Domain_OutboxMessages_ProcessedOn_NextAttemptDateUtc",
                schema: "Infra",
                table: "Domain_OutboxMessages",
                columns: new[] { "ProcessedOn", "NextAttemptDateUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Movimentos_AccountId",
                schema: "Financeiro",
                table: "Movimentos",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Movimentos_ReferenceId",
                schema: "Financeiro",
                table: "Movimentos",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Movimentos_TransactionId",
                schema: "Financeiro",
                table: "Movimentos",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_MT_InboxState_Delivered",
                schema: "Infra",
                table: "MT_InboxState",
                column: "Delivered");

            migrationBuilder.CreateIndex(
                name: "IX_MT_OutboxMessage_EnqueueTime",
                schema: "Infra",
                table: "MT_OutboxMessage",
                column: "EnqueueTime");

            migrationBuilder.CreateIndex(
                name: "IX_MT_OutboxMessage_ExpirationTime",
                schema: "Infra",
                table: "MT_OutboxMessage",
                column: "ExpirationTime");

            migrationBuilder.CreateIndex(
                name: "IX_MT_OutboxMessage_InboxMessageId_InboxConsumerId_SequenceNum~",
                schema: "Infra",
                table: "MT_OutboxMessage",
                columns: new[] { "InboxMessageId", "InboxConsumerId", "SequenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MT_OutboxMessage_OutboxId_SequenceNumber",
                schema: "Infra",
                table: "MT_OutboxMessage",
                columns: new[] { "OutboxId", "SequenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MT_OutboxState_Created",
                schema: "Infra",
                table: "MT_OutboxState",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_ContaId",
                schema: "Financeiro",
                table: "Transacoes",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_ContaId_ReferenceId",
                schema: "Financeiro",
                table: "Transacoes",
                columns: new[] { "ContaId", "ReferenceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_ReferenceId",
                schema: "Financeiro",
                table: "Transacoes",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_TransacoesProcessadas_ContaId",
                schema: "Financeiro",
                table: "TransacoesProcessadas",
                column: "AccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Domain_OutboxMessages",
                schema: "Infra");

            migrationBuilder.DropTable(
                name: "Movimentos",
                schema: "Financeiro");

            migrationBuilder.DropTable(
                name: "MT_OutboxMessage",
                schema: "Infra");

            migrationBuilder.DropTable(
                name: "Transacoes",
                schema: "Financeiro");

            migrationBuilder.DropTable(
                name: "TransacoesProcessadas",
                schema: "Financeiro");

            migrationBuilder.DropTable(
                name: "MT_InboxState",
                schema: "Infra");

            migrationBuilder.DropTable(
                name: "MT_OutboxState",
                schema: "Infra");

            migrationBuilder.DropTable(
                name: "Contas",
                schema: "Financeiro");

            migrationBuilder.DropSequence(
                name: "ContaSeq",
                schema: "Financeiro");
        }
    }
}
