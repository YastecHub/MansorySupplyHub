using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MansorySupplyHub.Migrations
{
    /// <inheritdoc />
    public partial class addedtheinquiries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InquiryHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationUserId1 = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InquiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PhoneNumber = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InquiryHeaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InquiryHeaders_AspNetUsers_ApplicationUserId1",
                        column: x => x.ApplicationUserId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InquiryDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InquiryHeaderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InquiryHeaderId1 = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InquiryDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InquiryDetails_InquiryHeaders_InquiryHeaderId1",
                        column: x => x.InquiryHeaderId1,
                        principalTable: "InquiryHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InquiryDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InquiryDetails_InquiryHeaderId1",
                table: "InquiryDetails",
                column: "InquiryHeaderId1");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryDetails_ProductId",
                table: "InquiryDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryHeaders_ApplicationUserId1",
                table: "InquiryHeaders",
                column: "ApplicationUserId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InquiryDetails");

            migrationBuilder.DropTable(
                name: "InquiryHeaders");
        }
    }
}
