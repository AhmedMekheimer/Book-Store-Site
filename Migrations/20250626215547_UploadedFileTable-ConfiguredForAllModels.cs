using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Online_Book_Store.Migrations
{
    /// <inheritdoc />
    public partial class UploadedFileTableConfiguredForAllModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Picture",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Picture",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Picture",
                table: "Authors");

            migrationBuilder.CreateTable(
                name: "UploadedFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<int>(type: "int", nullable: false),
                    AuthorId = table.Column<int>(type: "int", nullable: true),
                    BookId = table.Column<int>(type: "int", nullable: true),
                    PublishingHouseId = table.Column<int>(type: "int", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadedFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UploadedFiles_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UploadedFiles_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UploadedFiles_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UploadedFiles_PublishingHouses_PublishingHouseId",
                        column: x => x.PublishingHouseId,
                        principalTable: "PublishingHouses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UploadedFiles_AuthorId",
                table: "UploadedFiles",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedFiles_BookId",
                table: "UploadedFiles",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedFiles_CategoryId",
                table: "UploadedFiles",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedFiles_PublishingHouseId",
                table: "UploadedFiles",
                column: "PublishingHouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UploadedFiles");

            migrationBuilder.AddColumn<string>(
                name: "Picture",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Picture",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Picture",
                table: "Authors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
