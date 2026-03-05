using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContentHook.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddGenerationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Generations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TranscriptId = table.Column<Guid>(type: "uuid", nullable: false),
                    Platform = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Hook = table.Column<string>(type: "text", nullable: false),
                    Hashtags = table.Column<string>(type: "text", nullable: false),
                    ModelUsed = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PromptVersion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RegenerationIndex = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Generations", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Generations");
        }
    }
}
