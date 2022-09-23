using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace mpvv2.DbModels
{
    public partial class mpvContext : DbContext
    {
        public mpvContext()
        {
        }

        public mpvContext(DbContextOptions<mpvContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Carrier> Carriers { get; set; }
        public virtual DbSet<CarrierNameHistory> CarrierNameHistories { get; set; }
        public virtual DbSet<CarrierUrl> CarrierUrls { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<DepartOdis> DepartOdis { get; set; }
        public virtual DbSet<DepartPid> DepartPids { get; set; }
        public virtual DbSet<Depot> Depots { get; set; }
        public virtual DbSet<DepotForCarrierList> DepotForCarrierLists { get; set; }
        public virtual DbSet<Manufacturer> Manufacturers { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Photo> Photos { get; set; }
        public virtual DbSet<PhotoVeh> PhotoVehs { get; set; }
        public virtual DbSet<RegNumHistory> RegNumHistories { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<Stop> Stops { get; set; }
        public virtual DbSet<Street> Streets { get; set; }
        public virtual DbSet<Town> Towns { get; set; }
        public virtual DbSet<TypeDetail> TypeDetails { get; set; }
        public virtual DbSet<VehCarierList> VehCarierLists { get; set; }
        public virtual DbSet<VehNote> VehNotes { get; set; }
        public virtual DbSet<VehPaint> VehPaints { get; set; }
        public virtual DbSet<VehSet> VehSets { get; set; }
        public virtual DbSet<VehSetHistory> VehSetHistories { get; set; }
        public virtual DbSet<VehType> VehTypes { get; set; }
        public virtual DbSet<VehTypeHistory> VehTypeHistories { get; set; }
        public virtual DbSet<VehUpType> VehUpTypes { get; set; }
        public virtual DbSet<VehUrl> VehUrls { get; set; }
        public virtual DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("user id=root;host=localhost;database=mpv", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.4.24-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            modelBuilder.Entity<Carrier>(entity =>
            {
                entity.ToTable("carrier");

                entity.HasIndex(e => e.IdReg, "fk_car_cou");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.IdReg)
                    .HasColumnType("int(6)")
                    .HasColumnName("id_reg");

                entity.Property(e => e.LastUpdate)
                    .HasColumnType("timestamp")
                    .HasColumnName("last_update")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnName("name");

                entity.HasOne(d => d.IdRegNavigation)
                    .WithMany(p => p.Carriers)
                    .HasForeignKey(d => d.IdReg)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_car_reg");
            });

            modelBuilder.Entity<CarrierNameHistory>(entity =>
            {
                entity.ToTable("carrier_name_history");

                entity.HasIndex(e => e.IdCar, "fk_cnh_car");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.DateFrom)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("date_from");

                entity.Property(e => e.DateTo)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("date_to");

                entity.Property(e => e.IdCar)
                    .HasColumnType("int(11)")
                    .HasColumnName("id_car");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnName("name");

                entity.HasOne(d => d.IdCarNavigation)
                    .WithMany(p => p.CarrierNameHistories)
                    .HasForeignKey(d => d.IdCar)
                    .HasConstraintName("fk_car_his");
            });

            modelBuilder.Entity<CarrierUrl>(entity =>
            {
                entity.ToTable("carrier_url");

                entity.HasIndex(e => e.IdCar, "fk_car_url");

                entity.HasIndex(e => e.Url, "unique_cau_url")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.IdCar)
                    .HasColumnType("int(11)")
                    .HasColumnName("id_car");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnName("name");

                entity.Property(e => e.Note)
                    .IsRequired()
                    .HasMaxLength(64)
                    .HasColumnName("note");

                entity.Property(e => e.Position)
                    .HasColumnType("int(2)")
                    .HasColumnName("position");

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnName("url");

                entity.HasOne(d => d.IdCarNavigation)
                    .WithMany(p => p.CarrierUrls)
                    .HasForeignKey(d => d.IdCar)
                    .HasConstraintName("fk_car_url");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("country");

                entity.HasIndex(e => e.Abbreviation, "unique_abbreviation")
                    .IsUnique();

                entity.HasIndex(e => e.Name, "unique_name")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(6)")
                    .HasColumnName("id");

                entity.Property(e => e.Abbreviation)
                    .IsRequired()
                    .HasMaxLength(3)
                    .HasColumnName("abbreviation");

                entity.Property(e => e.Flag)
                    .HasMaxLength(128)
                    .HasColumnName("flag");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<DepartOdis>(entity =>
            {
                entity.ToTable("depart_odis");

                entity.HasIndex(e => e.IdVeh2, "fk_dep_veh2");

                entity.HasIndex(e => e.IdVeh3, "fk_dep_veh3");

                entity.HasIndex(e => new { e.IdVeh, e.Date, e.Line, e.Route }, "unique_route")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(32)")
                    .HasColumnName("id");

                entity.Property(e => e.ActDate)
                    .HasColumnType("timestamp")
                    .HasColumnName("act_date")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.Date)
                    .HasColumnType("date")
                    .HasColumnName("date");

                entity.Property(e => e.Delay)
                    .HasColumnType("int(2)")
                    .HasColumnName("delay");

                entity.Property(e => e.FinalStation)
                    .HasColumnType("int(11)")
                    .HasColumnName("final_station");

                entity.Property(e => e.IdVeh)
                    .IsRequired()
                    .HasMaxLength(36)
                    .HasColumnName("id_veh");

                entity.Property(e => e.IdVeh2)
                    .HasMaxLength(36)
                    .HasColumnName("id_veh2");

                entity.Property(e => e.IdVeh3)
                    .HasMaxLength(36)
                    .HasColumnName("id_veh3");

                entity.Property(e => e.LastStation)
                    .HasColumnType("int(11)")
                    .HasColumnName("last_station");

                entity.Property(e => e.Line)
                    .IsRequired()
                    .HasMaxLength(6)
                    .HasColumnName("line")
                    .UseCollation("utf8_unicode_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.Route)
                    .HasColumnType("int(11)")
                    .HasColumnName("route");

                entity.Property(e => e.StartDate)
                    .HasColumnType("timestamp")
                    .HasColumnName("start_date")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.StartStation)
                    .HasColumnType("int(11)")
                    .HasColumnName("start_station");

                entity.Property(e => e.WasIn).HasColumnName("was_in");

                entity.HasOne(d => d.IdVehNavigation)
                    .WithMany(p => p.DepartOdisIdVehNavigations)
                    .HasForeignKey(d => d.IdVeh)
                    .HasConstraintName("fk_dep_veh");

                entity.HasOne(d => d.IdVeh2Navigation)
                    .WithMany(p => p.DepartOdisIdVeh2Navigations)
                    .HasForeignKey(d => d.IdVeh2)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_dep_veh2");

                entity.HasOne(d => d.IdVeh3Navigation)
                    .WithMany(p => p.DepartOdisIdVeh3Navigations)
                    .HasForeignKey(d => d.IdVeh3)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_dep_veh3");
            });

            modelBuilder.Entity<DepartPid>(entity =>
            {
                entity.ToTable("depart_pid");

                entity.HasIndex(e => new { e.IdVeh, e.Date, e.Route, e.Line }, "unique_route")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(16)")
                    .HasColumnName("id");

                entity.Property(e => e.ActDate)
                    .HasColumnType("timestamp")
                    .HasColumnName("act_date")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.Date)
                    .HasColumnType("date")
                    .HasColumnName("date");

                entity.Property(e => e.Delay)
                    .HasColumnType("int(2)")
                    .HasColumnName("delay");

                entity.Property(e => e.FinalStation)
                    .HasColumnType("int(11)")
                    .HasColumnName("final_station");

                entity.Property(e => e.IdVeh)
                    .IsRequired()
                    .HasMaxLength(36)
                    .HasColumnName("id_veh")
                    .UseCollation("utf8_unicode_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.LastStation)
                    .HasColumnType("int(11)")
                    .HasColumnName("last_station");

                entity.Property(e => e.Line)
                    .IsRequired()
                    .HasMaxLength(6)
                    .HasColumnName("line")
                    .UseCollation("utf8_unicode_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.Route)
                    .HasColumnType("int(11)")
                    .HasColumnName("route");

                entity.Property(e => e.StartDate)
                    .HasColumnType("timestamp")
                    .HasColumnName("start_date");

                entity.Property(e => e.StartStation)
                    .HasColumnType("int(11)")
                    .HasColumnName("start_station");

                entity.Property(e => e.WasIn).HasColumnName("was_in");
            });

            modelBuilder.Entity<Depot>(entity =>
            {
                entity.ToTable("depot");

                entity.HasIndex(e => e.IdCar, "fk_dep_car");

                entity.HasIndex(e => new { e.Name, e.IdCar }, "unique_dep_car")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.IdCar)
                    .HasColumnType("int(11)")
                    .HasColumnName("id_car");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(64)
                    .HasColumnName("name");

                entity.HasOne(d => d.IdCarNavigation)
                    .WithMany(p => p.Depots)
                    .HasForeignKey(d => d.IdCar)
                    .HasConstraintName("fk_dep_car");
            });

            modelBuilder.Entity<DepotForCarrierList>(entity =>
            {
                entity.ToTable("depot_for_carrier_list");

                entity.HasIndex(e => e.IdDep, "fk_dcl_dep");

                entity.HasIndex(e => e.IdVcl, "fk_dcl_vcl");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.DateFrom)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("date_from");

                entity.Property(e => e.DateTo)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("date_to");

                entity.Property(e => e.IdDep)
                    .HasColumnType("int(11)")
                    .HasColumnName("id_dep");

                entity.Property(e => e.IdVcl)
                    .HasColumnType("int(11)")
                    .HasColumnName("id_vcl");
            });

            modelBuilder.Entity<Manufacturer>(entity =>
            {
                entity.ToTable("manufacturer");

                entity.Property(e => e.Id)
                    .HasColumnType("int(6)")
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(16)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.IdM)
                    .HasName("PRIMARY");

                entity.ToTable("messages");

                entity.Property(e => e.IdM)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID_m");

                entity.Property(e => e.Date)
                    .HasColumnType("timestamp")
                    .HasColumnName("date")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.Message1)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("message");
            });

            modelBuilder.Entity<Photo>(entity =>
            {
                entity.ToTable("photo");

                entity.HasIndex(e => e.IdSto, "fk_pho_sto");

                entity.HasIndex(e => e.IdStr, "fk_pho_str");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.IdSto)
                    .HasColumnType("int(11)")
                    .HasColumnName("id_sto");

                entity.Property(e => e.IdStr)
                    .HasColumnType("int(11)")
                    .HasColumnName("id_str");

                entity.Property(e => e.Note)
                    .HasMaxLength(128)
                    .HasColumnName("note");

                entity.Property(e => e.PhotoDate)
                    .HasColumnType("date")
                    .HasColumnName("photo_date");

                entity.Property(e => e.UploadDate)
                    .HasColumnType("timestamp")
                    .HasColumnName("upload_date")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(512)
                    .HasColumnName("url")
                    .HasDefaultValueSql("''");

                entity.HasOne(d => d.IdStoNavigation)
                    .WithMany(p => p.Photos)
                    .HasForeignKey(d => d.IdSto)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_pho_sto");

                entity.HasOne(d => d.IdStrNavigation)
                    .WithMany(p => p.Photos)
                    .HasForeignKey(d => d.IdStr)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_pho_str");
            });

            modelBuilder.Entity<PhotoVeh>(entity =>
            {
                entity.HasKey(e => new { e.IdPho, e.IdVeh })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("photo_veh");

                entity.HasIndex(e => e.IdVeh, "fk_pv_veh");

                entity.Property(e => e.IdPho)
                    .HasColumnType("int(11)")
                    .HasColumnName("id_pho");

                entity.Property(e => e.IdVeh)
                    .HasMaxLength(36)
                    .HasColumnName("id_veh");

                entity.HasOne(d => d.IdPhoNavigation)
                    .WithMany(p => p.PhotoVehs)
                    .HasForeignKey(d => d.IdPho)
                    .HasConstraintName("fk_pho_veh");

                entity.HasOne(d => d.IdVehNavigation)
                    .WithMany(p => p.PhotoVehs)
                    .HasForeignKey(d => d.IdVeh)
                    .HasConstraintName("fk_veh_pho");
            });

            modelBuilder.Entity<RegNumHistory>(entity =>
            {
                entity.ToTable("reg_num_history");

                entity.HasIndex(e => e.IdVeh, "fk_ren_veh");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.DateFrom)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("date_from");

                entity.Property(e => e.DateTo)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("date_to");

                entity.Property(e => e.IdVeh)
                    .IsRequired()
                    .HasMaxLength(36)
                    .HasColumnName("id_veh");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(6)
                    .HasColumnName("name");

                entity.HasOne(d => d.IdVehNavigation)
                    .WithMany(p => p.RegNumHistories)
                    .HasForeignKey(d => d.IdVeh)
                    .HasConstraintName("fk_ren_veh");
            });

            modelBuilder.Entity<Region>(entity =>
            {
                entity.ToTable("region");

                entity.HasIndex(e => e.IdCou, "fk_reg_cou");

                entity.HasIndex(e => new { e.Name, e.IdCou }, "unique_cou_reg")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(6)")
                    .HasColumnName("id");

                entity.Property(e => e.IdCou)
                    .HasColumnType("int(6)")
                    .HasColumnName("id_cou");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnName("name");

                entity.HasOne(d => d.IdCouNavigation)
                    .WithMany(p => p.Regions)
                    .HasForeignKey(d => d.IdCou)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_reg_cou");
            });

            modelBuilder.Entity<Stop>(entity =>
            {
                entity.ToTable("stop");

                entity.HasIndex(e => e.IdReg, "fk_sto_reg");

                entity.HasIndex(e => e.IdStr, "fk_sto_str");

                entity.HasIndex(e => new { e.Name, e.IdReg }, "unique_name_reg")
                    .IsUnique();

                entity.HasIndex(e => new { e.Name, e.IdStr }, "unique_name_street")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.IdReg)
                    .HasColumnType("int(6)")
                    .HasColumnName("id_reg")
                    .HasDefaultValueSql("'8'");

                entity.Property(e => e.IdStr)
                    .HasColumnType("int(11)")
                    .HasColumnName("id_str");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(64)
                    .HasColumnName("name")
                    .HasDefaultValueSql("'Undefined'")
                    .UseCollation("utf8_unicode_ci")
                    .HasCharSet("utf8");

                entity.HasOne(d => d.IdRegNavigation)
                    .WithMany(p => p.Stops)
                    .HasForeignKey(d => d.IdReg)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_sto_reg");

                entity.HasOne(d => d.IdStrNavigation)
                    .WithMany(p => p.Stops)
                    .HasForeignKey(d => d.IdStr)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_sto_str");
            });

            modelBuilder.Entity<Street>(entity =>
            {
                entity.ToTable("street");

                entity.HasIndex(e => e.IdTow, "fk_str_tow");

                entity.HasIndex(e => new { e.Name, e.IdTow }, "unique_tow_str")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.IdTow)
                    .HasColumnType("int(11)")
                    .HasColumnName("id_tow");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnName("name");

                entity.HasOne(d => d.IdTowNavigation)
                    .WithMany(p => p.Streets)
                    .HasForeignKey(d => d.IdTow)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_str_tow");
            });

            modelBuilder.Entity<Town>(entity =>
            {
                entity.ToTable("town");

                entity.HasIndex(e => e.IdReg, "fk_tow_reg");

                entity.HasIndex(e => new { e.Name, e.IdReg }, "unique_tow_reg")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.IdReg)
                    .HasColumnType("int(6)")
                    .HasColumnName("id_reg");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnName("name");

                entity.HasOne(d => d.IdRegNavigation)
                    .WithMany(p => p.Towns)
                    .HasForeignKey(d => d.IdReg)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tow_reg");
            });

            modelBuilder.Entity<TypeDetail>(entity =>
            {
                entity.ToTable("type_detail");

                entity.HasIndex(e => new { e.IdVeh, e.TKey }, "unique_veh_tyd")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.IdVeh)
                    .IsRequired()
                    .HasMaxLength(36)
                    .HasColumnName("id_veh");

                entity.Property(e => e.Position)
                    .HasColumnType("int(2)")
                    .HasColumnName("position");

                entity.Property(e => e.TKey)
                    .IsRequired()
                    .HasMaxLength(16)
                    .HasColumnName("t_key");

                entity.Property(e => e.TValue)
                    .IsRequired()
                    .HasMaxLength(64)
                    .HasColumnName("t_value");

                entity.HasOne(d => d.IdVehNavigation)
                    .WithMany(p => p.TypeDetails)
                    .HasForeignKey(d => d.IdVeh)
                    .HasConstraintName("fk_tyd_veh");
            });

            modelBuilder.Entity<VehCarierList>(entity =>
            {
                entity.ToTable("veh_carier_list");

                entity.HasIndex(e => e.IdVeh, "fk_vcl_veh");

                entity.HasIndex(e => e.IdCar, "fk_vdl_car");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.DateFrom)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("date_from")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.DateFrom2)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("date_from_2")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.DateTo)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("date_to")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.DateTo2)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("date_to_2")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.IdCar)
                    .HasColumnType("int(11)")
                    .HasColumnName("id_car");

                entity.Property(e => e.IdVeh)
                    .IsRequired()
                    .HasMaxLength(36)
                    .HasColumnName("id_veh");

                entity.Property(e => e.State)
                    .HasColumnType("int(1)")
                    .HasColumnName("state");

                entity.HasOne(d => d.IdCarNavigation)
                    .WithMany(p => p.VehCarierLists)
                    .HasForeignKey(d => d.IdCar)
                    .HasConstraintName("fk_car_veh");

                entity.HasOne(d => d.IdVehNavigation)
                    .WithMany(p => p.VehCarierLists)
                    .HasForeignKey(d => d.IdVeh)
                    .HasConstraintName("fk_veh_car");
            });

            modelBuilder.Entity<VehNote>(entity =>
            {
                entity.ToTable("veh_note");

                entity.HasIndex(e => e.IdVeh, "fk_not_veh");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.DateFrom)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("date_from");

                entity.Property(e => e.DateTo)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("date_to");

                entity.Property(e => e.IdVeh)
                    .IsRequired()
                    .HasMaxLength(36)
                    .HasColumnName("id_veh");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("message");

                entity.Property(e => e.Position)
                    .HasColumnType("int(3)")
                    .HasColumnName("position");

                entity.HasOne(d => d.IdVehNavigation)
                    .WithMany(p => p.VehNotes)
                    .HasForeignKey(d => d.IdVeh)
                    .HasConstraintName("fk_veh_not");
            });

            modelBuilder.Entity<VehPaint>(entity =>
            {
                entity.ToTable("veh_paint");

                entity.HasIndex(e => e.IdVeh, "fk_vpa_veh");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.DateFrom)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("date_from");

                entity.Property(e => e.DateTo)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("date_to");

                entity.Property(e => e.IdVeh)
                    .IsRequired()
                    .HasMaxLength(36)
                    .HasColumnName("id_veh");

                entity.Property(e => e.Note)
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnName("note");

                entity.Property(e => e.Paint)
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnName("paint");

                entity.HasOne(d => d.IdVehNavigation)
                    .WithMany(p => p.VehPaints)
                    .HasForeignKey(d => d.IdVeh)
                    .HasConstraintName("fk_veh_pai");
            });

            modelBuilder.Entity<VehSet>(entity =>
            {
                entity.ToTable("veh_set");

                entity.HasIndex(e => e.IdVeh2, "fk_set2_veh");

                entity.HasIndex(e => e.IdVeh3, "fk_set3_veh");

                entity.HasIndex(e => e.IdVeh1, "unique_set1")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.DateFrom)
                    .HasColumnType("timestamp")
                    .HasColumnName("date_from")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.IdVeh1)
                    .IsRequired()
                    .HasMaxLength(36)
                    .HasColumnName("id_veh1");

                entity.Property(e => e.IdVeh2)
                    .IsRequired()
                    .HasMaxLength(36)
                    .HasColumnName("id_veh2");

                entity.Property(e => e.IdVeh3)
                    .HasMaxLength(36)
                    .HasColumnName("id_veh3");

                entity.HasOne(d => d.IdVeh1Navigation)
                    .WithOne(p => p.VehSetIdVeh1Navigation)
                    .HasForeignKey<VehSet>(d => d.IdVeh1)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_set1_veh");

                entity.HasOne(d => d.IdVeh2Navigation)
                    .WithMany(p => p.VehSetIdVeh2Navigations)
                    .HasForeignKey(d => d.IdVeh2)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_set2_veh");

                entity.HasOne(d => d.IdVeh3Navigation)
                    .WithMany(p => p.VehSetIdVeh3Navigations)
                    .HasForeignKey(d => d.IdVeh3)
                    .HasConstraintName("fk_set3_veh");
            });

            modelBuilder.Entity<VehSetHistory>(entity =>
            {
                entity.ToTable("veh_set_history");

                entity.HasIndex(e => e.IdVeh1, "fk_seh_veh1");

                entity.HasIndex(e => e.IdVeh2, "fk_seh_veh2");

                entity.HasIndex(e => e.IdVeh3, "fk_seh_veh3");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.DateFrom)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("date_from");

                entity.Property(e => e.DateTo)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("date_to");

                entity.Property(e => e.IdVeh1)
                    .IsRequired()
                    .HasMaxLength(36)
                    .HasColumnName("id_veh1");

                entity.Property(e => e.IdVeh2)
                    .IsRequired()
                    .HasMaxLength(36)
                    .HasColumnName("id_veh2");

                entity.Property(e => e.IdVeh3)
                    .HasMaxLength(36)
                    .HasColumnName("id_veh3");

                entity.HasOne(d => d.IdVeh1Navigation)
                    .WithMany(p => p.VehSetHistoryIdVeh1Navigations)
                    .HasForeignKey(d => d.IdVeh1)
                    .HasConstraintName("fk_seh_veh1");

                entity.HasOne(d => d.IdVeh2Navigation)
                    .WithMany(p => p.VehSetHistoryIdVeh2Navigations)
                    .HasForeignKey(d => d.IdVeh2)
                    .HasConstraintName("fk_seh_veh2");

                entity.HasOne(d => d.IdVeh3Navigation)
                    .WithMany(p => p.VehSetHistoryIdVeh3Navigations)
                    .HasForeignKey(d => d.IdVeh3)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_seh_veh3");
            });

            modelBuilder.Entity<VehType>(entity =>
            {
                entity.ToTable("veh_type");

                entity.HasIndex(e => e.IdMan, "fk_typ_man");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.IdMan)
                    .HasColumnType("int(6)")
                    .HasColumnName("id_man");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(64)
                    .HasColumnName("name");

                entity.Property(e => e.Traction)
                    .HasColumnType("int(1)")
                    .HasColumnName("traction");

                entity.HasOne(d => d.IdManNavigation)
                    .WithMany(p => p.VehTypes)
                    .HasForeignKey(d => d.IdMan)
                    .HasConstraintName("fk_vet_man");
            });

            modelBuilder.Entity<VehTypeHistory>(entity =>
            {
                entity.HasKey(e => new { e.IdVut, e.IdVeh })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("veh_type_history");

                entity.HasIndex(e => e.IdVeh, "fk_th_veh");

                entity.Property(e => e.IdVut)
                    .HasColumnType("int(11)")
                    .HasColumnName("id_vut");

                entity.Property(e => e.IdVeh)
                    .HasMaxLength(36)
                    .HasColumnName("id_veh");

                entity.Property(e => e.DateFrom)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("date_from");

                entity.Property(e => e.DateTo)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("date_to");

                entity.HasOne(d => d.IdVehNavigation)
                    .WithMany(p => p.VehTypeHistories)
                    .HasForeignKey(d => d.IdVeh)
                    .HasConstraintName("fk_veh_vth");

                entity.HasOne(d => d.IdVutNavigation)
                    .WithMany(p => p.VehTypeHistories)
                    .HasForeignKey(d => d.IdVut)
                    .HasConstraintName("fk_vut_veh");
            });

            modelBuilder.Entity<VehUpType>(entity =>
            {
                entity.ToTable("veh_up_type");

                entity.HasIndex(e => e.IdVet, "fk_vut_vet");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.IdVet)
                    .HasColumnType("int(11)")
                    .HasColumnName("id_vet");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(64)
                    .HasColumnName("name");

                entity.HasOne(d => d.IdVetNavigation)
                    .WithMany(p => p.VehUpTypes)
                    .HasForeignKey(d => d.IdVet)
                    .HasConstraintName("fk_vut_vet");
            });

            modelBuilder.Entity<VehUrl>(entity =>
            {
                entity.ToTable("veh_url");

                entity.HasIndex(e => new { e.IdVeh, e.Name }, "unique_url_name")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.IdVeh)
                    .IsRequired()
                    .HasMaxLength(36)
                    .HasColumnName("id_veh");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnName("name");

                entity.Property(e => e.Note)
                    .HasMaxLength(64)
                    .HasColumnName("note");

                entity.Property(e => e.Position)
                    .HasColumnType("int(2)")
                    .HasColumnName("position");

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnName("url");

                entity.HasOne(d => d.IdVehNavigation)
                    .WithMany(p => p.VehUrls)
                    .HasForeignKey(d => d.IdVeh)
                    .HasConstraintName("fk_veh_url");
            });

            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.ToTable("vehicle");

                entity.HasIndex(e => e.IdReg, "fk_veh_reg");

                entity.HasIndex(e => e.IdVut, "fk_veh_vth");

                entity.HasIndex(e => new { e.IdDep, e.LongRegNum }, "unique_lid_dep")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasMaxLength(36)
                    .HasColumnName("id");

                entity.Property(e => e.AddDate)
                    .HasColumnType("timestamp")
                    .HasColumnName("add_date")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.AirCondition)
                    .HasColumnType("int(1)")
                    .HasColumnName("air_condition");

                entity.Property(e => e.Cond)
                    .HasColumnType("int(11)")
                    .HasColumnName("cond")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Departs)
                    .HasColumnType("int(11)")
                    .HasColumnName("departs");

                entity.Property(e => e.IdDep)
                    .HasColumnType("int(11)")
                    .HasColumnName("id_dep");

                entity.Property(e => e.IdReg)
                    .HasColumnType("int(6)")
                    .HasColumnName("id_reg")
                    .HasDefaultValueSql("'8'");

                entity.Property(e => e.IdVut)
                    .HasColumnType("int(11)")
                    .HasColumnName("id_vut");

                entity.Property(e => e.InfoPanel)
                    .IsRequired()
                    .HasMaxLength(64)
                    .HasColumnName("info_panel")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.LastSeen)
                    .HasColumnType("datetime")
                    .HasColumnName("last_seen");

                entity.Property(e => e.LastUpdate)
                    .HasColumnType("timestamp")
                    .HasColumnName("last_update")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.Listable).HasColumnName("listable");

                entity.Property(e => e.LongRegNum)
                    .IsRequired()
                    .HasMaxLength(11)
                    .HasColumnName("long_reg_num");

                entity.Property(e => e.LowFloor)
                    .HasColumnType("int(1)")
                    .HasColumnName("low_floor")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.ManufacYear)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("manufac_year")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.MiniPicture)
                    .HasColumnType("text")
                    .HasColumnName("mini_picture")
                    .UseCollation("utf8_unicode_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.Part)
                    .HasColumnType("tinyint(4)")
                    .HasColumnName("part");

                entity.Property(e => e.RegNum)
                    .IsRequired()
                    .HasMaxLength(8)
                    .HasColumnName("reg_num")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.SerialNumber)
                    .HasColumnType("int(10)")
                    .HasColumnName("serial_number");

                entity.Property(e => e.Spz)
                    .HasMaxLength(7)
                    .HasColumnName("spz");

                entity.Property(e => e.Vin)
                    .HasMaxLength(20)
                    .HasColumnName("vin");

                entity.Property(e => e.WasInCount)
                    .HasColumnType("int(8)")
                    .HasColumnName("was_in_count");

                entity.Property(e => e.WasInLast)
                    .HasColumnType("timestamp")
                    .HasColumnName("was_in_last");

                entity.HasOne(d => d.IdDepNavigation)
                    .WithMany(p => p.Vehicles)
                    .HasForeignKey(d => d.IdDep)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_veh_dep");

                entity.HasOne(d => d.IdRegNavigation)
                    .WithMany(p => p.Vehicles)
                    .HasForeignKey(d => d.IdReg)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_veh_reg");

                entity.HasOne(d => d.IdVutNavigation)
                    .WithMany(p => p.Vehicles)
                    .HasForeignKey(d => d.IdVut)
                    .HasConstraintName("fk_veh_vut");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
