using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lunafy.Data.Migrations
{
    /// <inheritdoc />
    public partial class ArtistProfilePicture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProfilePictureId",
                table: "Artists",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Artists_ProfilePictureId",
                table: "Artists",
                column: "ProfilePictureId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Artists_Pictures_ProfilePictureId",
                table: "Artists",
                column: "ProfilePictureId",
                principalTable: "Pictures",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artists_Pictures_ProfilePictureId",
                table: "Artists");

            migrationBuilder.DropIndex(
                name: "IX_Artists_ProfilePictureId",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "ProfilePictureId",
                table: "Artists");
        }
    }
}
