using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Online_Book_Store.Migrations
{
    /// <inheritdoc />
    public partial class RemovedMethodFromUploadedFileTable_BooksDeleteBehavior_Cascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UploadedFiles_Authors_AuthorId",
                table: "UploadedFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UploadedFiles_Books_BookId",
                table: "UploadedFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UploadedFiles_Categories_CategoryId",
                table: "UploadedFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UploadedFiles_PublishingHouses_PublishingHouseId",
                table: "UploadedFiles");

            migrationBuilder.AddForeignKey(
                name: "FK_UploadedFiles_Authors_AuthorId",
                table: "UploadedFiles",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UploadedFiles_Books_BookId",
                table: "UploadedFiles",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UploadedFiles_Categories_CategoryId",
                table: "UploadedFiles",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UploadedFiles_PublishingHouses_PublishingHouseId",
                table: "UploadedFiles",
                column: "PublishingHouseId",
                principalTable: "PublishingHouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UploadedFiles_Authors_AuthorId",
                table: "UploadedFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UploadedFiles_Books_BookId",
                table: "UploadedFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UploadedFiles_Categories_CategoryId",
                table: "UploadedFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UploadedFiles_PublishingHouses_PublishingHouseId",
                table: "UploadedFiles");

            migrationBuilder.AddForeignKey(
                name: "FK_UploadedFiles_Authors_AuthorId",
                table: "UploadedFiles",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UploadedFiles_Books_BookId",
                table: "UploadedFiles",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UploadedFiles_Categories_CategoryId",
                table: "UploadedFiles",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UploadedFiles_PublishingHouses_PublishingHouseId",
                table: "UploadedFiles",
                column: "PublishingHouseId",
                principalTable: "PublishingHouses",
                principalColumn: "Id");
        }
    }
}
