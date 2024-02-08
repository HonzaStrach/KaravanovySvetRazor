using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KaravanovySvet.Migrations
{
    /// <inheritdoc />
    public partial class BlogTotalViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalViews",
                table: "Blog",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalViews",
                table: "Blog");
        }
    }
}
