using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContentHook.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTonalityToGeneration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tonality",
                table: "Generations",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tonality",
                table: "Generations");
        }
    }
}
