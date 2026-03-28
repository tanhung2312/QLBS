using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QLBS.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ShippingAddress",
                table: "OrderTable",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ReceiverPhone",
                table: "OrderTable",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ReceiverName",
                table: "OrderTable",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "DistrictID",
                table: "OrderTable",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GhnOrderCode",
                table: "OrderTable",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProvinceID",
                table: "OrderTable",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingFee",
                table: "OrderTable",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "WardCode",
                table: "OrderTable",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "RoleId", "Description", "RoleName" },
                values: new object[,]
                {
                    { 1, null, "Admin" },
                    { 2, null, "Customer" },
                    { 3, null, "Staff" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "RoleId",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "DistrictID",
                table: "OrderTable");

            migrationBuilder.DropColumn(
                name: "GhnOrderCode",
                table: "OrderTable");

            migrationBuilder.DropColumn(
                name: "ProvinceID",
                table: "OrderTable");

            migrationBuilder.DropColumn(
                name: "ShippingFee",
                table: "OrderTable");

            migrationBuilder.DropColumn(
                name: "WardCode",
                table: "OrderTable");

            migrationBuilder.AlterColumn<string>(
                name: "ShippingAddress",
                table: "OrderTable",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "ReceiverPhone",
                table: "OrderTable",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "ReceiverName",
                table: "OrderTable",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);
        }
    }
}
