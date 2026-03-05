using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContentHook.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RefactorJobVideoFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VideoFileName",
                table: "Jobs",
                newName: "OriginalFileName");

            migrationBuilder.AddColumn<string>(
                name: "VideoStorageKey",
                table: "Jobs",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoStorageKey",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "OriginalFileName",
                table: "Jobs",
                newName: "VideoFileName");
        }
    }
}
