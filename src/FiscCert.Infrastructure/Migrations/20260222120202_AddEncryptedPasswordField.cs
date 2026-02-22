using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiscCert.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEncryptedPasswordField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EncryptedPassword",
                table: "Certificates",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EncryptedPassword",
                table: "Certificates");
        }
    }
}
