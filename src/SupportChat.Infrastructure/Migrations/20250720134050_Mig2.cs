using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ChatSessions_AssignedAgentId",
                table: "ChatSessions",
                column: "AssignedAgentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatSessions_Agents_AssignedAgentId",
                table: "ChatSessions",
                column: "AssignedAgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatSessions_Agents_AssignedAgentId",
                table: "ChatSessions");

            migrationBuilder.DropIndex(
                name: "IX_ChatSessions_AssignedAgentId",
                table: "ChatSessions");
        }
    }
}
