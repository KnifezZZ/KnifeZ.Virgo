using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebDemo.Model.Migrations
{
    public partial class up1230 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Blogs",
                table: "Blogs");

            migrationBuilder.RenameTable(
                name: "Blogs",
                newName: "Blog");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Blog",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BlogCategoryID",
                table: "Blog",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Blog",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSinglePage",
                table: "Blog",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "Blog",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Keywords",
                table: "Blog",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Blog",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Blog",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VisitCount",
                table: "Blog",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Blog",
                table: "Blog",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "BlogCategories",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Icon = table.Column<string>(type: "TEXT", nullable: true),
                    Url = table.Column<string>(type: "TEXT", nullable: true),
                    Sort = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreateBy = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdateBy = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogCategories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BlogCategories_BlogCategories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "BlogCategories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BlogClassifications",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    FrameworkUserID = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreateBy = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdateBy = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogClassifications", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BlogClassifications_FrameworkUsers_FrameworkUserID",
                        column: x => x.FrameworkUserID,
                        principalTable: "FrameworkUsers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlogClassificationMiddles",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "TEXT", nullable: false),
                    BlogId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BlogClassificationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreateBy = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdateBy = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogClassificationMiddles", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BlogClassificationMiddles_Blog_BlogId",
                        column: x => x.BlogId,
                        principalTable: "Blog",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogClassificationMiddles_BlogClassifications_BlogClassificationId",
                        column: x => x.BlogClassificationId,
                        principalTable: "BlogClassifications",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Blog_BlogCategoryID",
                table: "Blog",
                column: "BlogCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_BlogCategories_ParentId",
                table: "BlogCategories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogClassificationMiddles_BlogClassificationId",
                table: "BlogClassificationMiddles",
                column: "BlogClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogClassificationMiddles_BlogId",
                table: "BlogClassificationMiddles",
                column: "BlogId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogClassifications_FrameworkUserID",
                table: "BlogClassifications",
                column: "FrameworkUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Blog_BlogCategories_BlogCategoryID",
                table: "Blog",
                column: "BlogCategoryID",
                principalTable: "BlogCategories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blog_BlogCategories_BlogCategoryID",
                table: "Blog");

            migrationBuilder.DropTable(
                name: "BlogCategories");

            migrationBuilder.DropTable(
                name: "BlogClassificationMiddles");

            migrationBuilder.DropTable(
                name: "BlogClassifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Blog",
                table: "Blog");

            migrationBuilder.DropIndex(
                name: "IX_Blog_BlogCategoryID",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "BlogCategoryID",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "IsSinglePage",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "Keywords",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "VisitCount",
                table: "Blog");

            migrationBuilder.RenameTable(
                name: "Blog",
                newName: "Blogs");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Blogs",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Blogs",
                table: "Blogs",
                column: "ID");
        }
    }
}
