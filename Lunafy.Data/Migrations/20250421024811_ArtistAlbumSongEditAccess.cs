using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lunafy.Data.Migrations
{
    /// <inheritdoc />
    public partial class ArtistAlbumSongEditAccess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlbumEditAccesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AlbumId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CanEdit = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanDelete = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumEditAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlbumEditAccesses_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlbumEditAccesses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ArtistEditAccesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ArtistId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CanEdit = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanDelete = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistEditAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArtistEditAccesses_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtistEditAccesses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SongEditAccesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SongId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CanEdit = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanDelete = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongEditAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SongEditAccesses_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SongEditAccesses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumEditAccesses_AlbumId",
                table: "AlbumEditAccesses",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumEditAccesses_UserId",
                table: "AlbumEditAccesses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistEditAccesses_ArtistId",
                table: "ArtistEditAccesses",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistEditAccesses_UserId",
                table: "ArtistEditAccesses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SongEditAccesses_SongId",
                table: "SongEditAccesses",
                column: "SongId");

            migrationBuilder.CreateIndex(
                name: "IX_SongEditAccesses_UserId",
                table: "SongEditAccesses",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlbumEditAccesses");

            migrationBuilder.DropTable(
                name: "ArtistEditAccesses");

            migrationBuilder.DropTable(
                name: "SongEditAccesses");
        }
    }
}
