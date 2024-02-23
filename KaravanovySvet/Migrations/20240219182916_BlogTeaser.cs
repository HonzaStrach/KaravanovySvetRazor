using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KaravanovySvet.Migrations
{
    /// <inheritdoc />
    public partial class BlogTeaser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Teaser",
                table: "Blog",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Teaser",
                table: "Blog");
        }
    }
}
