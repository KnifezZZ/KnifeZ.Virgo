﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebDemo.Model;

namespace WebDemo.Model.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("KnifeZ.Virgo.Core.ActionLog", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("ActionName")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ActionTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("ActionUrl")
                        .HasMaxLength(250)
                        .HasColumnType("TEXT");

                    b.Property<string>("CreateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreateTime")
                        .HasColumnType("TEXT");

                    b.Property<double>("Duration")
                        .HasColumnType("REAL");

                    b.Property<string>("IP")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("ITCode")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<int>("LogType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ModuleName")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("Remark")
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("ActionLogs");
                });

            modelBuilder.Entity("KnifeZ.Virgo.Core.DataPrivilege", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CreateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreateTime")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("DomainId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("GroupId")
                        .HasColumnType("TEXT");

                    b.Property<string>("RelateId")
                        .HasColumnType("TEXT");

                    b.Property<string>("TableName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("DomainId");

                    b.HasIndex("GroupId");

                    b.HasIndex("UserId");

                    b.ToTable("DataPrivileges");
                });

            modelBuilder.Entity("KnifeZ.Virgo.Core.FileAttachment", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CreateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreateTime")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("FileData")
                        .HasColumnType("BLOB");

                    b.Property<string>("FileExt")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("TEXT");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("GroupName")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsTemprory")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Length")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Path")
                        .HasColumnType("TEXT");

                    b.Property<int?>("SaveFileMode")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UpdateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UploadTime")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("FileAttachments");
                });

            modelBuilder.Entity("KnifeZ.Virgo.Core.FrameworkDomain", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT")
                        .HasColumnName("DomainAddress");

                    b.Property<string>("CreateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("EntryUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT")
                        .HasColumnName("DomainName");

                    b.Property<int?>("Port")
                        .HasColumnType("INTEGER")
                        .HasColumnName("DomainPort");

                    b.Property<string>("UpdateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("FrameworkDomains");
                });

            modelBuilder.Entity("KnifeZ.Virgo.Core.FrameworkGroup", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CreateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("GroupCode")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("GroupName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("GroupRemark")
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("FrameworkGroups");
                });

            modelBuilder.Entity("KnifeZ.Virgo.Core.FrameworkMenu", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("ActionName")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClassName")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreateTime")
                        .HasColumnType("TEXT");

                    b.Property<int?>("DisplayOrder")
                        .IsRequired()
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("DomainId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("FolderOnly")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ICon")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsInherit")
                        .HasColumnType("INTEGER");

                    b.Property<bool?>("IsInside")
                        .IsRequired()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MethodName")
                        .HasColumnType("TEXT");

                    b.Property<string>("ModuleName")
                        .HasColumnType("TEXT");

                    b.Property<string>("PageName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("ShowOnMenu")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UpdateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Url")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("DomainId");

                    b.HasIndex("ParentId");

                    b.ToTable("FrameworkMenus");
                });

            modelBuilder.Entity("KnifeZ.Virgo.Core.FrameworkRole", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CreateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleCode")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleRemark")
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("FrameworkRoles");
                });

            modelBuilder.Entity("KnifeZ.Virgo.Core.FrameworkUserBase", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Address")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<string>("CellPhone")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("HomePhone")
                        .HasMaxLength(30)
                        .HasColumnType("TEXT");

                    b.Property<string>("ITCode")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsValid")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("PhotoId")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Sex")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UpdateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("ZipCode")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("ITCode")
                        .IsUnique();

                    b.HasIndex("PhotoId");

                    b.ToTable("FrameworkUsers");
                });

            modelBuilder.Entity("KnifeZ.Virgo.Core.FrameworkUserGroup", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CreateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreateTime")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("GroupId");

                    b.HasIndex("UserId");

                    b.ToTable("FrameworkUserGroup");
                });

            modelBuilder.Entity("KnifeZ.Virgo.Core.FrameworkUserRole", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CreateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreateTime")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("FrameworkUserRole");
                });

            modelBuilder.Entity("KnifeZ.Virgo.Core.FunctionPrivilege", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<bool?>("Allowed")
                        .IsRequired()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CreateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreateTime")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("MenuItemId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("RoleId")
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("MenuItemId");

                    b.ToTable("FunctionPrivileges");
                });

            modelBuilder.Entity("KnifeZ.Virgo.Core.PersistedGrant", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Expiration")
                        .HasColumnType("TEXT");

                    b.Property<string>("RefreshToken")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("PersistedGrants");
                });

            modelBuilder.Entity("KnifeZ.Virgo.Core.SearchCondition", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Condition")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("VMName")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("UserId");

                    b.ToTable("SearchConditions");
                });

            modelBuilder.Entity("WebDemo.Model.Blog", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("BlogCategoryId")
                        .HasColumnType("TEXT");

                    b.Property<string>("BodyText")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsSinglePage")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsValid")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Keywords")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Url")
                        .HasColumnType("TEXT");

                    b.Property<int>("VisitCount")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("BlogCategoryId");

                    b.ToTable("Blog");
                });

            modelBuilder.Entity("WebDemo.Model.BlogCategory", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CreateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Icon")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Sort")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UpdateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Url")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("ParentId");

                    b.ToTable("BlogCategories");
                });

            modelBuilder.Entity("WebDemo.Model.BlogClassification", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CreateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreateTime")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("FrameworkUserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("FrameworkUserId");

                    b.ToTable("BlogClassifications");
                });

            modelBuilder.Entity("WebDemo.Model.BlogClassificationMiddle", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("BlogClassificationId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("BlogId")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdateBy")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("BlogClassificationId");

                    b.HasIndex("BlogId");

                    b.ToTable("BlogClassificationMiddles");
                });

            modelBuilder.Entity("KnifeZ.Virgo.Core.DataPrivilege", b =>
                {
                    b.HasOne("KnifeZ.Virgo.Core.FrameworkDomain", "Domain")
                        .WithMany()
                        .HasForeignKey("DomainId");

                    b.HasOne("KnifeZ.Virgo.Core.FrameworkGroup", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("KnifeZ.Virgo.Core.FrameworkUserBase", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Domain");

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("KnifeZ.Virgo.Core.FrameworkMenu", b =>
                {
                    b.HasOne("KnifeZ.Virgo.Core.FrameworkDomain", "Domain")
                        .WithMany()
                        .HasForeignKey("DomainId");

                    b.HasOne("KnifeZ.Virgo.Core.FrameworkMenu", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");

                    b.Navigation("Domain");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("KnifeZ.Virgo.Core.FrameworkUserBase", b =>
                {
                    b.HasOne("KnifeZ.Virgo.Core.FileAttachment", "Photo")
                        .WithMany()
                        .HasForeignKey("PhotoId");

                    b.Navigation("Photo");
                });

            modelBuilder.Entity("KnifeZ.Virgo.Core.FrameworkUserGroup", b =>
                {
                    b.HasOne("KnifeZ.Virgo.Core.FrameworkGroup", "Group")
                        .WithMany("UserGroups")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KnifeZ.Virgo.Core.FrameworkUserBase", "User")
                        .WithMany("UserGroups")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("KnifeZ.Virgo.Core.FrameworkUserRole", b =>
                {
                    b.HasOne("KnifeZ.Virgo.Core.FrameworkRole", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KnifeZ.Virgo.Core.FrameworkUserBase", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("KnifeZ.Virgo.Core.FunctionPrivilege", b =>
                {
                    b.HasOne("KnifeZ.Virgo.Core.FrameworkMenu", "MenuItem")
                        .WithMany("Privileges")
                        .HasForeignKey("MenuItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MenuItem");
                });

            modelBuilder.Entity("KnifeZ.Virgo.Core.SearchCondition", b =>
                {
                    b.HasOne("KnifeZ.Virgo.Core.FrameworkUserBase", "User")
                        .WithMany("SearchConditions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("WebDemo.Model.Blog", b =>
                {
                    b.HasOne("WebDemo.Model.BlogCategory", "BlogCategory")
                        .WithMany()
                        .HasForeignKey("BlogCategoryId");

                    b.Navigation("BlogCategory");
                });

            modelBuilder.Entity("WebDemo.Model.BlogCategory", b =>
                {
                    b.HasOne("WebDemo.Model.BlogCategory", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("WebDemo.Model.BlogClassification", b =>
                {
                    b.HasOne("KnifeZ.Virgo.Core.FrameworkUserBase", "FrameworkUser")
                        .WithMany()
                        .HasForeignKey("FrameworkUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FrameworkUser");
                });

            modelBuilder.Entity("WebDemo.Model.BlogClassificationMiddle", b =>
                {
                    b.HasOne("WebDemo.Model.BlogClassification", "BlogClassification")
                        .WithMany("BlogClassificationMiddle")
                        .HasForeignKey("BlogClassificationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebDemo.Model.Blog", "Blog")
                        .WithMany("BlogClassificationMiddle")
                        .HasForeignKey("BlogId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Blog");

                    b.Navigation("BlogClassification");
                });

            modelBuilder.Entity("KnifeZ.Virgo.Core.FrameworkGroup", b =>
                {
                    b.Navigation("UserGroups");
                });

            modelBuilder.Entity("KnifeZ.Virgo.Core.FrameworkMenu", b =>
                {
                    b.Navigation("Children");

                    b.Navigation("Privileges");
                });

            modelBuilder.Entity("KnifeZ.Virgo.Core.FrameworkRole", b =>
                {
                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("KnifeZ.Virgo.Core.FrameworkUserBase", b =>
                {
                    b.Navigation("SearchConditions");

                    b.Navigation("UserGroups");

                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("WebDemo.Model.Blog", b =>
                {
                    b.Navigation("BlogClassificationMiddle");
                });

            modelBuilder.Entity("WebDemo.Model.BlogCategory", b =>
                {
                    b.Navigation("Children");
                });

            modelBuilder.Entity("WebDemo.Model.BlogClassification", b =>
                {
                    b.Navigation("BlogClassificationMiddle");
                });
#pragma warning restore 612, 618
        }
    }
}
