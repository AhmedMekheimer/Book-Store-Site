using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Online_Book_Store.Migrations
{
    /// <inheritdoc />
    public partial class UploadedFileTable_DeleteBehavior_ClientSetNullWithConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_UploadedFiles_SingleOwner",
                table: "UploadedFiles",
                sql: "(IIF(AuthorId IS NULL, 0, 1) + IIF(BookId IS NULL, 0, 1) + IIF(PublishingHouseId IS NULL, 0, 1) + IIF(CategoryId IS NULL, 0, 1)) = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_UploadedFiles_SingleOwner",
                table: "UploadedFiles");
        }
    }
}
