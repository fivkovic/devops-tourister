using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Property.Core.Migrations
{
    /// <inheritdoc />
    public partial class V4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Slots_SlotId",
                table: "Reservations");

            migrationBuilder.AlterColumn<Guid>(
                name: "SlotId",
                table: "Reservations",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Slots_SlotId",
                table: "Reservations",
                column: "SlotId",
                principalTable: "Slots",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Slots_SlotId",
                table: "Reservations");

            migrationBuilder.AlterColumn<Guid>(
                name: "SlotId",
                table: "Reservations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Slots_SlotId",
                table: "Reservations",
                column: "SlotId",
                principalTable: "Slots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
