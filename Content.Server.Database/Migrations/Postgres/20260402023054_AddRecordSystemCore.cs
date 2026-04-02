using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Content.Server.Database.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class AddRecordSystemCore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "record_character",
                columns: table => new
                {
                    record_character_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    round_id = table.Column<int>(type: "integer", nullable: true),
                    record_type = table.Column<int>(type: "integer", nullable: false),
                    target_character_id = table.Column<int>(type: "integer", nullable: true),
                    author_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    author_character_id = table.Column<int>(type: "integer", nullable: true),
                    hidden = table.Column<bool>(type: "boolean", nullable: false),
                    deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_record_character", x => x.record_character_id);
                    table.ForeignKey(
                        name: "FK_record_character_player_player_id",
                        column: x => x.author_user_id,
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_record_character_player_player_id1",
                        column: x => x.deleted_by_id,
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_record_character_profile_profile_id",
                        column: x => x.author_character_id,
                        principalTable: "profile",
                        principalColumn: "profile_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_record_character_profile_profile_id1",
                        column: x => x.target_character_id,
                        principalTable: "profile",
                        principalColumn: "profile_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "record_edit",
                columns: table => new
                {
                    record_edit_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    record_character_id = table.Column<int>(type: "integer", nullable: false),
                    field = table.Column<string>(type: "text", nullable: false),
                    old_value = table.Column<string>(type: "text", nullable: true),
                    new_value = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    author_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    author_character_id = table.Column<int>(type: "integer", nullable: true),
                    deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_record_edit", x => x.record_edit_id);
                    table.ForeignKey(
                        name: "FK_record_edit_player_player_id",
                        column: x => x.author_user_id,
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_record_edit_player_player_id1",
                        column: x => x.deleted_by_id,
                        principalTable: "player",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_record_edit_profile_profile_id",
                        column: x => x.author_character_id,
                        principalTable: "profile",
                        principalColumn: "profile_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_record_edit_record_character_record_character_id",
                        column: x => x.record_character_id,
                        principalTable: "record_character",
                        principalColumn: "record_character_id");
                });

            migrationBuilder.CreateTable(
                name: "record_personal_note",
                columns: table => new
                {
                    record_character_id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    note = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_record_personal_note", x => x.record_character_id);
                    table.ForeignKey(
                        name: "FK_record_personal_note_record_character_record_character_id",
                        column: x => x.record_character_id,
                        principalTable: "record_character",
                        principalColumn: "record_character_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_record_character_author_character_id",
                table: "record_character",
                column: "author_character_id");

            migrationBuilder.CreateIndex(
                name: "IX_record_character_author_user_id",
                table: "record_character",
                column: "author_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_record_character_deleted_by_id",
                table: "record_character",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_record_character_record_type_target_character_id_created_at",
                table: "record_character",
                columns: new[] { "record_type", "target_character_id", "created_at" },
                descending: new[] { false, false, true });

            migrationBuilder.CreateIndex(
                name: "IX_record_character_target_character_id",
                table: "record_character",
                column: "target_character_id");

            migrationBuilder.CreateIndex(
                name: "IX_record_edit_author_character_id",
                table: "record_edit",
                column: "author_character_id");

            migrationBuilder.CreateIndex(
                name: "IX_record_edit_author_user_id",
                table: "record_edit",
                column: "author_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_record_edit_deleted_by_id",
                table: "record_edit",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_record_edit_record_character_id_created_at",
                table: "record_edit",
                columns: new[] { "record_character_id", "created_at" },
                descending: new[] { false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "record_edit");

            migrationBuilder.DropTable(
                name: "record_personal_note");

            migrationBuilder.DropTable(
                name: "record_character");
        }
    }
}
