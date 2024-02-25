using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Black_Coffee_Cafe.Migrations
{
    /// <inheritdoc />
    public partial class menuitems2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CookingMethod0 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CookingMethod1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Flavor1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Flavor2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Topping0 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Topping1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Topping2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Topping3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuItems");
        }
    }
}
