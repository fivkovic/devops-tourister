using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Property.Core.Migrations
{
    /// <inheritdoc />
    public partial class TextPatternIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Properties_Location",
                table: "Properties");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Properties",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Properties",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_Location",
                table: "Properties",
                column: "Location")
                .Annotation("Npgsql:IndexOperators", new[] { "varchar_pattern_ops" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Properties_Location",
                table: "Properties");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Properties",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Properties",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.CreateIndex(
                name: "IX_Properties_Location",
                table: "Properties",
                column: "Location");
        }
    }
}
