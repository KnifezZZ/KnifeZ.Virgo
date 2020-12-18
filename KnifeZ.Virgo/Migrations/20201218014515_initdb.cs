using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KnifeZ.Virgo.Migrations
{
    public partial class initdb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActionLogs",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ActionName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ITCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ActionUrl = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ActionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<double>(type: "float", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LogType = table.Column<int>(type: "int", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionLogs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FileAttachments",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileExt = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Length = table.Column<long>(type: "bigint", nullable: false),
                    UploadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsTemprory = table.Column<bool>(type: "bit", nullable: false),
                    SaveFileMode = table.Column<int>(type: "int", nullable: true),
                    GroupName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FileData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileAttachments", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FrameworkDomains",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DomainName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DomainAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DomainPort = table.Column<int>(type: "int", nullable: true),
                    EntryUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrameworkDomains", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FrameworkGroups",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GroupRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrameworkGroups", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FrameworkRoles",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RoleRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrameworkRoles", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PersistedGrants",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Expiration = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersistedGrants", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FrameworkUsers",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ITCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Sex = table.Column<int>(type: "int", nullable: true),
                    CellPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomePhone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhotoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsValid = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrameworkUsers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FrameworkUsers_FileAttachments_PhotoId",
                        column: x => x.PhotoId,
                        principalTable: "FileAttachments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FrameworkMenus",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PageName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ActionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModuleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FolderOnly = table.Column<bool>(type: "bit", nullable: false),
                    IsInherit = table.Column<bool>(type: "bit", nullable: false),
                    ClassName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MethodName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DomainId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ShowOnMenu = table.Column<bool>(type: "bit", nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsInside = table.Column<bool>(type: "bit", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ICon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrameworkMenus", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FrameworkMenus_FrameworkDomains_DomainId",
                        column: x => x.DomainId,
                        principalTable: "FrameworkDomains",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FrameworkMenus_FrameworkMenus_ParentId",
                        column: x => x.ParentId,
                        principalTable: "FrameworkMenus",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DataPrivileges",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TableName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RelateId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DomainId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataPrivileges", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DataPrivileges_FrameworkDomains_DomainId",
                        column: x => x.DomainId,
                        principalTable: "FrameworkDomains",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DataPrivileges_FrameworkGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "FrameworkGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DataPrivileges_FrameworkUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "FrameworkUsers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FrameworkUserGroup",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrameworkUserGroup", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FrameworkUserGroup_FrameworkGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "FrameworkGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FrameworkUserGroup_FrameworkUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "FrameworkUsers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FrameworkUserRole",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrameworkUserRole", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FrameworkUserRole_FrameworkRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "FrameworkRoles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FrameworkUserRole_FrameworkUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "FrameworkUsers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SearchConditions",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Condition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VMName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchConditions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SearchConditions_FrameworkUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "FrameworkUsers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FunctionPrivileges",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MenuItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Allowed = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FunctionPrivileges", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FunctionPrivileges_FrameworkMenus_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "FrameworkMenus",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataPrivileges_DomainId",
                table: "DataPrivileges",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_DataPrivileges_GroupId",
                table: "DataPrivileges",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_DataPrivileges_UserId",
                table: "DataPrivileges",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FrameworkMenus_DomainId",
                table: "FrameworkMenus",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_FrameworkMenus_ParentId",
                table: "FrameworkMenus",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_FrameworkUserGroup_GroupId",
                table: "FrameworkUserGroup",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_FrameworkUserGroup_UserId",
                table: "FrameworkUserGroup",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FrameworkUserRole_RoleId",
                table: "FrameworkUserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_FrameworkUserRole_UserId",
                table: "FrameworkUserRole",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FrameworkUsers_ITCode",
                table: "FrameworkUsers",
                column: "ITCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FrameworkUsers_PhotoId",
                table: "FrameworkUsers",
                column: "PhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_FunctionPrivileges_MenuItemId",
                table: "FunctionPrivileges",
                column: "MenuItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchConditions_UserId",
                table: "SearchConditions",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionLogs");

            migrationBuilder.DropTable(
                name: "DataPrivileges");

            migrationBuilder.DropTable(
                name: "FrameworkUserGroup");

            migrationBuilder.DropTable(
                name: "FrameworkUserRole");

            migrationBuilder.DropTable(
                name: "FunctionPrivileges");

            migrationBuilder.DropTable(
                name: "PersistedGrants");

            migrationBuilder.DropTable(
                name: "SearchConditions");

            migrationBuilder.DropTable(
                name: "FrameworkGroups");

            migrationBuilder.DropTable(
                name: "FrameworkRoles");

            migrationBuilder.DropTable(
                name: "FrameworkMenus");

            migrationBuilder.DropTable(
                name: "FrameworkUsers");

            migrationBuilder.DropTable(
                name: "FrameworkDomains");

            migrationBuilder.DropTable(
                name: "FileAttachments");
        }
    }
}
