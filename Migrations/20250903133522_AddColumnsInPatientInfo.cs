using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMDH_QS.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnsInPatientInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "PatientsInformation");

            migrationBuilder.AddColumn<string>(
                name: "Barangay",
                table: "PatientsInformation",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "PatientsInformation",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "PatientsInformation",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Barangay",
                table: "PatientsInformation");

            migrationBuilder.DropColumn(
                name: "City",
                table: "PatientsInformation");

            migrationBuilder.DropColumn(
                name: "Province",
                table: "PatientsInformation");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "PatientsInformation",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
