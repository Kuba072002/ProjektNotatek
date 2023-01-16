using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjektNotatek.Migrations
{
    /// <inheritdoc />
    public partial class EncryptedMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Notes",
                newName: "Password");

            migrationBuilder.AddColumn<bool>(
                name: "IsEncrypted",
                table: "Notes",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEncrypted",
                table: "Notes");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Notes",
                newName: "UserId");
        }
    }
}
