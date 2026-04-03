using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class MakeProfilePreferenceIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_profile_preference_preference_id",
                table: "profile");

            migrationBuilder.AlterColumn<int>(
                name: "preference_id",
                table: "profile",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_profile_preference_preference_id",
                table: "profile",
                column: "preference_id",
                principalTable: "preference",
                principalColumn: "preference_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_profile_preference_preference_id",
                table: "profile");

            migrationBuilder.AlterColumn<int>(
                name: "preference_id",
                table: "profile",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_profile_preference_preference_id",
                table: "profile",
                column: "preference_id",
                principalTable: "preference",
                principalColumn: "preference_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
