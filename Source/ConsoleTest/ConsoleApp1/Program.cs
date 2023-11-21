using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            using (var db = new TestDbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var aProfile = new AProfile();
                var bProfile = new BProfile();
                var cProfile = new CProfile();

                var pool = new Pool
                {
                    Profile = bProfile
                };

                bProfile.Pool = pool;

                db.Add(aProfile);
                db.Add(bProfile);
                db.Add(cProfile);
                db.Add(pool);

                db.SaveChanges();

                var pool2 = db.Pools.FirstOrDefault();

                if (pool2 != null)
                {
                    Console.WriteLine($"Pool2: {pool2.Id}");
                    Console.WriteLine($"Pool2.Profile: {pool2.Profile?.Id}");
                }
            }

            using (var db = new TestDbContext())
            {
                var pool2 = db.Pools.FirstOrDefault();
                if (pool2 != null)
                {
                    Console.WriteLine($"Pool2: {pool2.Id}");
                    Console.WriteLine($"Pool2.Profile: {pool2.Profile?.Id}");
                }

            }

            string temp = StringTest.temp2;
            string temp2 = StringTest.temp;

        }
    }

    public class StringTest
    {
        public StringTest(string test = "")
        {
            _test = test;
        }

        private string _test;

        public static readonly string temp = "temp";
        public const string temp2 = "temp2";
    }

    public class Pool
    {
        public int Id { get; set; }

        public Profile? Profile
        {
            get
            {
                return ORMAProfileAccessor as Profile ?? ORMBProfileAccessor;
            }

            set
            {
                switch (value)
                {
                    case AProfile aProfile:
                        ORMAProfileAccessor = aProfile;
                        break;
                    case BProfile bProfile:
                        ORMBProfileAccessor = bProfile;
                        break;
                    default:
                        throw new InvalidCastException("Invalid profile type");
                }
            }
        }

        public virtual AProfile? ORMAProfileAccessor { get; set; }
        public virtual BProfile? ORMBProfileAccessor { get; set; }
    }

    public abstract class Profile
    {
        public Profile() { }
        public int Id { get; set; }
    }

    public class AProfile : Profile
    {
        public AProfile() { }
        public virtual Pool? Pool { get; set; }
    }

    public class BProfile : Profile
    {
        public BProfile() { }
        public virtual Pool? Pool { get; set; }
    }

    public class CProfile : Profile
    {
        public CProfile() { }
        // CProfile does not have the Pool property
    }

    internal class TestDbContext : DbContext
    {
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<AProfile> AProfiles { get; set; }
        public DbSet<BProfile> BProfiles { get; set; }
        public DbSet<CProfile> CProfiles { get; set; }
        public DbSet<Pool> Pools { get; set; }

        public string DbPath { get; }

        public TestDbContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "blogging.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Test.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<AProfile>().HasOne(P => P.Pool).WithOne(Q => Q.ORMAProfileAccessor).HasForeignKey<Pool>("AProfileId").IsRequired(false);
            modelBuilder.Entity<BProfile>().HasOne(P => P.Pool).WithOne(Q => Q.ORMBProfileAccessor).HasForeignKey<Pool>("BProfileId").IsRequired(false);

            modelBuilder.Entity<Pool>().Ignore(P => P.Profile);
        }
    }
}