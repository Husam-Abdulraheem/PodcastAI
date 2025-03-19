using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PodcastAI.Migrations
{
    /// <inheritdoc />
    public partial class SpecialPod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Special",
                table: "podcasts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Special",
                table: "podcasts");
        }
    }
}
