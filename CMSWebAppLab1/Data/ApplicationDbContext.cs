using CMSWebAppLab1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CMSWebAppLab1.Data
{
    public partial class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cinema> Cinemas { get; set; }

        public virtual DbSet<Hall> Halls { get; set; }

        public virtual DbSet<Movie> Movies { get; set; }

        public virtual DbSet<Person> Persons { get; set; }

        public virtual DbSet<PersonsToMovie> PersonsToMovies { get; set; }

        public virtual DbSet<Session> Sessions { get; set; }

        public virtual DbSet<Ticket> Tickets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
            => optionsBuilder.UseSqlServer("Data Source=MYROSLAVBOBYL1DCB;Initial Catalog=CinemaDB;Integrated Security=True;Encrypt=False;Trust Server Certificate=True");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cinema>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("Cinemas$PrimaryKey");

                entity.HasIndex(e => e.Address, "Cinemas$Address").IsUnique();

                entity.HasIndex(e => e.CinemaName, "Cinemas$CinemaName").IsUnique();

                entity.Property(e => e.Address).HasMaxLength(255);
                entity.Property(e => e.CinemaName).HasMaxLength(255);
            });

            modelBuilder.Entity<Hall>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("Halls$PrimaryKey");

                entity.Property(e => e.HallName).HasMaxLength(255);
                entity.Property(e => e.MaxPlaces).HasDefaultValue(30);

                entity.HasOne(d => d.Cinema).WithMany(p => p.Halls)
                    .HasForeignKey(d => d.CinemaId)
                    .HasConstraintName("Halls$CinemasHalls");
            });

            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("Movies$PrimaryKey");

                entity.HasIndex(e => e.Title, "Movies$Title").IsUnique();

                entity.Property(e => e.Duration).HasDefaultValue((short)90);
                entity.Property(e => e.Genre).HasMaxLength(255);
                entity.Property(e => e.ReleaseYear).HasDefaultValue(1914);
                entity.Property(e => e.Title).HasMaxLength(255);
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("Persons$PrimaryKey");

                entity.HasIndex(e => e.Id, "Persons$MoviePersonId").IsUnique();

                entity.HasIndex(e => e.PersonName, "Persons$PersonName").IsUnique();

                entity.Property(e => e.PersonName).HasMaxLength(255);
                entity.Property(e => e.TookPartAs).HasMaxLength(255);
            });

            modelBuilder.Entity<PersonsToMovie>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PersonsToMovies$PrimaryKey");

                entity.HasIndex(e => e.MovieId, "PersonsToMovies$PersonToMovieMovieId");

                entity.HasIndex(e => e.PersonId, "PersonsToMovies$PersonToMoviePersonId");

                entity.HasOne(d => d.Movie).WithMany(p => p.PersonsToMovies)
                    .HasForeignKey(d => d.MovieId)
                    .HasConstraintName("PersonsToMovies$MoviesPersonsToMovies");

                entity.HasOne(d => d.Person).WithMany(p => p.PersonsToMovies)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PersonsToMovies$PersonsPersonsToMovies");
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("Sessions$PrimaryKey");

                entity.HasIndex(e => e.HallId, "Sessions$SessionsHallId");

                entity.HasIndex(e => e.MovieId, "Sessions$SessionsMovieId");

                entity.Property(e => e.Price).HasColumnType("money");
                entity.Property(e => e.StartTime).HasPrecision(0);

                entity.HasOne(d => d.Hall).WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.HallId)
                    .HasConstraintName("Sessions$HallsSessions");

                entity.HasOne(d => d.Movie).WithMany(p => p.Sessions)
                    .HasForeignKey(d => d.MovieId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Sessions$MoviesSessions");
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("Tickets$PrimaryKey");

                entity.HasIndex(e => e.SeatNumber, "Tickets$Place");

                entity.HasIndex(e => e.SessionId, "Tickets$TicketsSessionId");

                entity.Property(e => e.SoldTime).HasPrecision(0);

                entity.HasOne(d => d.Session).WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.SessionId)
                    .HasConstraintName("Tickets$SessionsTickets");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
