using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContentHook.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddOriginalFileNameToTranscript : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginalFileName",
                table: "Transcripts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalFileName",
                table: "Transcripts");
        }
    }
}
