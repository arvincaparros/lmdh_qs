using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMDH_QS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDoctorsNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Prescription",
                table: "DoctorNotes",
                newName: "Remarks");

            migrationBuilder.AlterColumn<string>(
                name: "RecordedBy",
                table: "PatientVitalRecord",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PatientId",
                table: "PatientVitalRecord",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "HistoryOfIllness",
                table: "DoctorNotes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HistoryOfIllness",
                table: "DoctorNotes");

            migrationBuilder.RenameColumn(
                name: "Remarks",
                table: "DoctorNotes",
                newName: "Prescription");

            migrationBuilder.AlterColumn<string>(
                name: "RecordedBy",
                table: "PatientVitalRecord",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PatientId",
                table: "PatientVitalRecord",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
