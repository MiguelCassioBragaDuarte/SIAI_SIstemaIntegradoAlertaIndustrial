using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiProcessamento.Migrations
{
    /// <inheritdoc />
    public partial class AlertaManualNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alertas_LeiturasSensor_LeituraSensorId",
                table: "Alertas");

            migrationBuilder.AlterColumn<int>(
                name: "LeituraSensorId",
                table: "Alertas",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Alertas_LeiturasSensor_LeituraSensorId",
                table: "Alertas",
                column: "LeituraSensorId",
                principalTable: "LeiturasSensor",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alertas_LeiturasSensor_LeituraSensorId",
                table: "Alertas");

            migrationBuilder.AlterColumn<int>(
                name: "LeituraSensorId",
                table: "Alertas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Alertas_LeiturasSensor_LeituraSensorId",
                table: "Alertas",
                column: "LeituraSensorId",
                principalTable: "LeiturasSensor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
