using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMDH_QS.Migrations
{
    /// <inheritdoc />
    public partial class AddNewColumnsInDoctorsNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DischargeDate",
                table: "DoctorNotes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "DischargeTime",
                table: "DoctorNotes",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Disposition",
                table: "DoctorNotes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DischargeDate",
                table: "DoctorNotes");

            migrationBuilder.DropColumn(
                name: "DischargeTime",
                table: "DoctorNotes");

            migrationBuilder.DropColumn(
                name: "Disposition",
                table: "DoctorNotes");
        }
    }
}
