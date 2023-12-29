using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            // PolymorphicMultiRelationshipTest();

            using (var db = new TestDbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Add(new Foo(name: "salam", lastName: nameof(Foo.LastName), nickName: nameof(Foo.NickName), iName: nameof(Foo.IName)));
                db.Add(new Foo(name: "hh", nickName: "kk"));

                db.SaveChanges();
            }


            using (var db = new TestDbContext())
            {
                List<Foo> foos = db.Foos.ToList();
                foos.ForEach(F => Console.WriteLine($"foo Name: {F.Name}"));
                foos.ForEach(F => Console.WriteLine($"foo LastName: {F.IName}"));
                foos.ForEach(F => Console.WriteLine($"foo NickName: {F.NickName}"));
                foos.ForEach(F => Console.WriteLine($"foo IName: {F.IName}"));
            }
        }

        private static void PolymorphicMultiRelationshipTest()
        {
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


            throw new NotImplementedException();
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

    #region Pools
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
    #endregion

    public class Foo
    {
        public Foo(string name = DEFAULT_NAME, string nickName = DEFAULT_NAME)
            : this(name, DEFAULT_NAME, nickName, DEFAULT_NAME)
        { }
        public Foo(string name = DEFAULT_NAME, string lastName = DEFAULT_NAME, string nickName = DEFAULT_NAME, string iName = DEFAULT_NAME)
        {
            Name = name ?? DEFAULT_NAME;
            LastName = lastName ?? DEFAULT_NAME;
            NickName = nickName ?? DEFAULT_NAME;
            IName = iName ?? DEFAULT_NAME;
        }

        public int Id { get; set; }
        public string Name { get; }
        public string LastName { get; private set; }
        public string NickName { get; }
        public string IName { get; }

        public const string DEFAULT_NAME = "Default";
    }

    internal class TestDbContext : DbContext
    {
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<AProfile> AProfiles { get; set; }
        public DbSet<BProfile> BProfiles { get; set; }
        public DbSet<CProfile> CProfiles { get; set; }
        public DbSet<Pool> Pools { get; set; }
        public DbSet<Foo> Foos { get; set; }

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

            modelBuilder.Entity<Foo>().HasKey(F => F.Id);
            modelBuilder.Entity<Foo>().Property(F => F.Name);
            modelBuilder.Entity<Foo>().Ignore(F => F.LastName);
            modelBuilder.Entity<Foo>().MapAllReadonlyProperty();
            modelBuilder.Entity<Foo>().Ignore(F => F.IName);



            modelBuilder.Entity<AProfile>().HasOne(P => P.Pool).WithOne(Q => Q.ORMAProfileAccessor).HasForeignKey<Pool>("AProfileId").IsRequired(false);
            modelBuilder.Entity<BProfile>().HasOne(P => P.Pool).WithOne(Q => Q.ORMBProfileAccessor).HasForeignKey<Pool>("BProfileId").IsRequired(false);

            modelBuilder.Entity<Pool>().Ignore(P => P.Profile);
        }
    }

    public static class ModelBuilderExtensions
    {
        public static void MapAllReadonlyProperty<T>(this EntityTypeBuilder<T> builder) where T : class
        {
            var internalProperties = builder.Metadata.GetProperties();
            var ignores = builder.Metadata.GetIgnoredMembers();
            var properties = typeof(T).GetProperties().Where(propertyInfo =>
                propertyInfo.CanWrite == false
                && propertyInfo.GetCustomAttribute<NotMappedAttribute>() == null
                && !ignores.Any(ignoreProperty => ignoreProperty == propertyInfo.Name));
            foreach (var property in properties)
            {
                builder.Property(property.Name);
            }
            internalProperties = builder.Metadata.GetProperties();
        }
    }
}