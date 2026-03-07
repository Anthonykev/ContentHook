using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContentHook.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Evaluations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    VideosPerMonth = table.Column<string>(type: "text", nullable: false),
                    MainPlatform = table.Column<string>(type: "text", nullable: false),
                    Experience = table.Column<string>(type: "text", nullable: false),
                    TitleScore = table.Column<int>(type: "integer", nullable: false),
                    HookScore = table.Column<int>(type: "integer", nullable: false),
                    HashtagScore = table.Column<int>(type: "integer", nullable: false),
                    PracticalScore = table.Column<int>(type: "integer", nullable: false),
                    OverallQualityScore = table.Column<int>(type: "integer", nullable: false),
                    PlatformFitScore = table.Column<int>(type: "integer", nullable: false),
                    PlatformInfluenceScore = table.Column<int>(type: "integer", nullable: false),
                    HashtagPlatformScore = table.Column<int>(type: "integer", nullable: false),
                    PlatformOptimizationScore = table.Column<int>(type: "integer", nullable: false),
                    UsabilityScore = table.Column<int>(type: "integer", nullable: false),
                    LayoutScore = table.Column<int>(type: "integer", nullable: false),
                    PresentationScore = table.Column<int>(type: "integer", nullable: false),
                    TimeSavingScore = table.Column<int>(type: "integer", nullable: false),
                    GenerationSpeedScore = table.Column<int>(type: "integer", nullable: false),
                    SupportScore = table.Column<int>(type: "integer", nullable: false),
                    EfficiencyComparisonScore = table.Column<int>(type: "integer", nullable: false),
                    OverallSatisfactionScore = table.Column<int>(type: "integer", nullable: false),
                    ReusabilityScore = table.Column<int>(type: "integer", nullable: false),
                    RecommendationScore = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluations", x => x.Id);
                });

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
                    Tonality = table.Column<string>(type: "text", nullable: false),
                    RegenerationIndex = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Generations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Platform = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    OriginalFileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    VideoStorageKey = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    TranscriptId = table.Column<Guid>(type: "uuid", nullable: true),
                    GenerationId = table.Column<Guid>(type: "uuid", nullable: true),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transcripts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    Language = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OriginalFileName = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transcripts", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Evaluations");

            migrationBuilder.DropTable(
                name: "Generations");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "Transcripts");
        }
    }
}
