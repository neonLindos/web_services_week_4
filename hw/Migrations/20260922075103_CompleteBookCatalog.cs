using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hw.Migrations;

public partial class CompleteBookCatalog : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "AutorId", table: "Books");
        // Год старых книг неизвестен: его нужно заполнить при редактировании.
        migrationBuilder.AddColumn<int>(name: "Year", table: "Books",
            type: "INTEGER", nullable: false, defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "Year", table: "Books");
        migrationBuilder.AddColumn<int>(name: "AutorId", table: "Books",
            type: "INTEGER", nullable: false, defaultValue: 0);
    }
}
