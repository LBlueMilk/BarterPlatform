using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BarterPlatform.Models
{
    public partial class BarterPlatformContext : DbContext
    {
        public BarterPlatformContext()
        {
        }

        public BarterPlatformContext(DbContextOptions<BarterPlatformContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AdministrativeRegion> AdministrativeRegion { get; set; } = null!;
        public virtual DbSet<AuthenticationRecords> AuthenticationRecords { get; set; } = null!;
        public virtual DbSet<Comment> Comment { get; set; } = null!;
        public virtual DbSet<District> District { get; set; } = null!;
        public virtual DbSet<Employee> Employee { get; set; } = null!;
        public virtual DbSet<Item> Item { get; set; } = null!;
        public virtual DbSet<ManageCommentRecords> ManageCommentRecords { get; set; } = null!;
        public virtual DbSet<ManageItemRecords> ManageItemRecords { get; set; } = null!;
        public virtual DbSet<Member> Member { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=127.0.0.1;Database=BarterPlatform;Trusted_Connection=True;");

                //特定使用者
                //"Data Source = MCSDD11202 - 22; Database = BarterPlatform; TrustServerCertificate = True; User ID = abc; Password = 123"
                //Windows 身份驗證
                //"Data Source=127.0.0.1;Database=BarterPlatform;Trusted_Connection=True;"
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdministrativeRegion>(entity =>
            {
                entity.HasKey(e => e.AdmID)
                    .HasName("PK__Administ__A13DEA8A3338198D");

                entity.Property(e => e.AdmID)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.AdminRegion)
                    .HasMaxLength(6)
                    .IsFixedLength();
            });

            modelBuilder.Entity<AuthenticationRecords>(entity =>
            {
                entity.HasKey(e => e.AuthID)
                    .HasName("PK__Authenti__12C15D335C4C5433");

                entity.Property(e => e.AuthID).ValueGeneratedNever();

                entity.Property(e => e.AuthTime).HasColumnType("datetime");

                entity.Property(e => e.Employee_EmpID)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Member_MemID)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.HasOne(d => d.Employee_Emp)
                    .WithMany(p => p.AuthenticationRecords)
                    .HasForeignKey(d => d.Employee_EmpID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Authentic__Emplo__48CFD27E");

                entity.HasOne(d => d.Member_Mem)
                    .WithMany(p => p.AuthenticationRecords)
                    .HasForeignKey(d => d.Member_MemID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Authentic__Membe__49C3F6B7");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(e => e.ComID)
                    .HasName("PK__Comment__E15F413268AE8BFD");

                entity.Property(e => e.ComID).ValueGeneratedNever();

                entity.Property(e => e.ComContent).HasMaxLength(400);

                entity.Property(e => e.ComTime).HasColumnType("datetime");

                entity.Property(e => e.Member_MemID)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .IsFixedLength();

                // 添加新的外鍵屬性
                entity.Property(e => e.Item_IteID).HasColumnName("Item_IteID");

                // 定義外鍵關係
                entity.HasOne(d => d.Item_ID)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.Item_IteID)
                    .HasConstraintName("FK__Comment__Item_ID__42E1EEFE");

                entity.HasOne(d => d.Member_Mem)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.Member_MemID)
                    .HasConstraintName("FK__Comment__Member___45F365D3");
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.HasKey(e => e.DisID)
                    .HasName("PK__District__E2AA7E64488E578E");

                entity.Property(e => e.DisID)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.DistrictName)
                    .HasMaxLength(8)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmpID)
                    .HasName("PK__Employee__AF2DBA792B1B0FED");

                entity.Property(e => e.EmpID)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.AdminRegion_AdmID)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.BirthDate).HasColumnType("date");

                entity.Property(e => e.District_DisID)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Gender).HasMaxLength(4);

                entity.Property(e => e.HireDate).HasColumnType("datetime");

                entity.Property(e => e.NationalID).HasMaxLength(10);

                entity.Property(e => e.Nickname).HasMaxLength(20);

                entity.Property(e => e.OtherAddress).HasMaxLength(200);

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.PersonalName).HasMaxLength(200);

                entity.Property(e => e.Phone).HasMaxLength(12);

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.Salary).HasColumnType("money");

                entity.Property(e => e.TermDate).HasColumnType("datetime");

                entity.Property(e => e.Username).HasMaxLength(50);

                entity.HasOne(d => d.AdminRegion_Adm)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.AdminRegion_AdmID)
                    .HasConstraintName("FK__Employee__AdminR__3B75D760");

                entity.HasOne(d => d.District_Dis)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.District_DisID)
                    .HasConstraintName("FK__Employee__Distri__3C69FB99");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(e => e.IteID)
                    .HasName("PK__Item__07CED97F09BF8112");

                entity.Property(e => e.IteID).ValueGeneratedNever();

                entity.Property(e => e.IteDesc).HasMaxLength(1000);

                entity.Property(e => e.ItemName).HasMaxLength(50);

                entity.Property(e => e.Member_MemID)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.UploadTime).HasColumnType("datetime");

                entity.HasOne(d => d.Member_Mem)
                    .WithMany(p => p.Item)
                    .HasForeignKey(d => d.Member_MemID)
                    .HasConstraintName("FK__Item__Member_Mem__4316F928");
            });

            modelBuilder.Entity<ManageCommentRecords>(entity =>
            {
                entity.HasKey(e => e.MCRID)
                    .HasName("PK__ManageCo__44480219E5CD5597");

                entity.Property(e => e.MCRID).ValueGeneratedNever();

                entity.Property(e => e.AfterContent).HasMaxLength(400);

                entity.Property(e => e.BeforeContent).HasMaxLength(400);

                entity.Property(e => e.Employee_EmpID)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.ModTime).HasColumnType("datetime");

                entity.HasOne(d => d.Comment_Com)
                    .WithMany(p => p.ManageCommentRecords)
                    .HasForeignKey(d => d.Comment_ComID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ManageCom__Comme__5165187F");

                entity.HasOne(d => d.Employee_Emp)
                    .WithMany(p => p.ManageCommentRecords)
                    .HasForeignKey(d => d.Employee_EmpID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ManageCom__Emplo__5070F446");
            });

            modelBuilder.Entity<ManageItemRecords>(entity =>
            {
                entity.HasKey(e => e.MIRID)
                    .HasName("PK__ManageIt__6ED3C4CB257C7737");

                entity.Property(e => e.MIRID).ValueGeneratedNever();

                entity.Property(e => e.AfterContent).HasMaxLength(400);

                entity.Property(e => e.BeforeContent).HasMaxLength(400);

                entity.Property(e => e.Employee_EmpID)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.ModTime).HasColumnType("datetime");

                entity.HasOne(d => d.Employee_Emp)
                    .WithMany(p => p.ManageItemRecords)
                    .HasForeignKey(d => d.Employee_EmpID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ManageIte__Emplo__4CA06362");

                entity.HasOne(d => d.Item_Ite)
                    .WithMany(p => p.ManageItemRecords)
                    .HasForeignKey(d => d.Item_IteID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ManageIte__Item___4D94879B");
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasKey(e => e.MemID)
                    .HasName("PK__Member__E9341AE294A757FA");

                entity.Property(e => e.MemID)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.AdminRegion_AdmID)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.BirthDate).HasColumnType("date");

                entity.Property(e => e.CreationTime).HasColumnType("datetime");

                entity.Property(e => e.DeletionTime).HasColumnType("datetime");

                entity.Property(e => e.District_DisID)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Gender).HasMaxLength(4);

                entity.Property(e => e.LastLogin).HasColumnType("datetime");

                entity.Property(e => e.NationalID).HasMaxLength(10);

                entity.Property(e => e.Nickname).HasMaxLength(20);

                entity.Property(e => e.OtherAddress).HasMaxLength(200);

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.PersonalName).HasMaxLength(400);

                entity.Property(e => e.Phone).HasMaxLength(12);

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.Username).HasMaxLength(50);

                entity.HasOne(d => d.AdminRegion_Adm)
                    .WithMany(p => p.Member)
                    .HasForeignKey(d => d.AdminRegion_AdmID)
                    .HasConstraintName("FK__Member__AdminReg__3F466844");

                entity.HasOne(d => d.District_Dis)
                    .WithMany(p => p.Member)
                    .HasForeignKey(d => d.District_DisID)
                    .HasConstraintName("FK__Member__District__403A8C7D");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
