using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace ConsoleApp1
{
    internal class MyMeter : Meter
    {
        public MyMeter(MeterOptions options) : base(options)
        {
        }

        public MyMeter(string name) : base(name)
        {
        }

        public MyMeter(string name, string? version) : base(name, version)
        {
        }

        public MyMeter(string name, string? version, IEnumerable<KeyValuePair<string, object?>>? tags, object? scope = null) : base(name, version, tags, scope)
        {
        }
        

        //
        // Summary:
        //     Create a metrics Counter object.
        //
        // Parameters:
        //   name:
        //     The instrument name. Cannot be null.
        //
        //   unit:
        //     Optional instrument unit of measurements.
        //
        //   description:
        //     Optional instrument description.
        //
        // Type parameters:
        //   T:
        //     The numerical type of the measurement.
        //
        // Returns:
        //     A new counter.
        public MyUpDownCounter<T> CreateMyCounter<T>(string name, string? unit = null, string? description = null) where T : struct
        {
            return new MyUpDownCounter<T>(this, name, unit, description);
        }
    }

    internal class MyUpDownCounter<T> : Instrument<T> where T : struct
    {
        internal MyUpDownCounter(Meter meter, string name, string? unit, string? description) : base(meter, name, unit, description)
        {
            _counters = new Dictionary<long, UpDownCounter<T>>();
        }

        public void Add(long ndc, T value)
        {
            _counters[ndc].Add(value);
        }

        private Dictionary<long, UpDownCounter<T>> _counters;
    }

    internal class Program
    {
        static TProfile SwtichTest<TProfile>(TProfile profile) where TProfile : Profile
        {
            return profile switch
            {
                AProfile aProfile => (TProfile)(object)aProfile.GenerateAProfile(),
                BProfile bProfile => (TProfile)(object)bProfile.GenerateBProfile(),
                CProfile cProfile => (TProfile)(object)cProfile.GenerateCProfile(),
                _ => throw new InvalidCastException("Invalid profile type")
            };
        }
        static void Foo(Bar b, Barrier barrier, Meter meter)
        {
            object o = null;
            if (o is int)
            {
                int i = (int)o;
                Console.WriteLine($"{i}");
            }

            if (b == null)
                Console.WriteLine("bb2 is null");
            else if (barrier == null)
                Console.WriteLine("fosh");

            Console.WriteLine("bb2 is null");
            Bar bb2 = b ?? new Bar(12, 34);
            Bar bb = b ?? throw new Exception("b is null");
            _ = b?.A;
            _ = barrier?.CurrentPhaseNumber;
            _ = meter?.Name;
        }
        static void Main(string[] args)
        {
            var m = new MyMeter(new MeterOptions("test"));
            var incommingCallCounter = m.CreateMyCounter<int>("testCounter");
            incommingCallCounter.Add(123, 1);

            var outgoingCallCounter = m.CreateMyCounter<int>("testCounter");
            outgoingCallCounter.Add(123, 1);


            Console.WriteLine("Hello, World!");

            // PolymorphicMultiRelationshipTest();

            using (var db = new TestDbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Add(new Foo(bar: new Bar(55, 65), name: "hh", nickName: "kk") );

                db.SaveChanges();
            }


            using (var db = new TestDbContext())
            {
                List<Foo> foos = db.Foos.ToList();
                foos.ForEach(F => Console.WriteLine($"foo Name: {F.Name}"));
                foos.ForEach(F => Console.WriteLine($"foo LastName: {F.IName}"));
                foos.ForEach(F => Console.WriteLine($"foo NickName: {F.NickName}"));
                foos.ForEach(F => Console.WriteLine($"foo IName: {F.IName}"));
                foos.ForEach(F => Console.WriteLine($"foo Bar: {F.Bar}"));
            }

            TestViewModel testViewModel = new TestViewModel();
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

    public class TestViewModel
    {
        [SetsRequiredMembers]
        public TestViewModel(string myProperty = "", string myProperty1 = "")
        {
            MyProperty = myProperty;
            MyProperty1 = myProperty1;
        }

        public required string MyProperty { get; set; }

        public required string MyProperty1 { get; set; }
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

        public AProfile GenerateAProfile()
        {
            return new AProfile();
        }
    }

    public class BProfile : Profile
    {
        public BProfile() { }
        public virtual Pool? Pool { get; set; }

        public BProfile GenerateBProfile()
        {
            return new BProfile();
        }
    }

    public class CProfile : Profile
    {
        public CProfile() { }
        // CProfile does not have the Pool property

        public CProfile GenerateCProfile()
        {
            return new CProfile();
        }
    }
    #endregion

    public class Foo
    {
        public Foo(string name = DEFAULT_NAME, string nickName = DEFAULT_NAME)
            : this(null, name, DEFAULT_NAME, nickName, DEFAULT_NAME)
        { }


        public Foo(string name = DEFAULT_NAME, string lastName = DEFAULT_NAME, string nickName = DEFAULT_NAME, string iName = DEFAULT_NAME)
        {
            Name = name ?? DEFAULT_NAME;
            LastName = lastName ?? DEFAULT_NAME;
            NickName = nickName ?? DEFAULT_NAME;
            IName = iName ?? DEFAULT_NAME;
        }

        public Foo(Bar bar, string name = DEFAULT_NAME, string nickName = DEFAULT_NAME)
            : this(bar, name, DEFAULT_NAME, nickName, DEFAULT_NAME)
        { }

        public Foo(Bar bar, string name = DEFAULT_NAME, string lastName = DEFAULT_NAME, string nickName = DEFAULT_NAME, string iName = DEFAULT_NAME)
        {
            Bar = bar;
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

        public virtual Bar Bar { get; }

        public const string DEFAULT_NAME = "Default";
    }

    public class Bar
    {
        public Bar(int a, int b)
        {
            A = a;
            B = b;
        }

        public int A { get; }
        public int B { get; }

        public int C { get; set; }

        public override string ToString()
        {
            return $"A: {A}, B: {B}";
        }
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
            modelBuilder.Entity<Foo>().OwnsOne(F => F.Bar).MapAllReadonlyProperty();
            modelBuilder.Entity<Foo>().OwnsOne(F => F.Bar).Ignore(B => B.C);
            modelBuilder.Entity<Foo>().MapAllReadonlyProperty();
            modelBuilder.Entity<Foo>().Ignore(F => F.IName);



            modelBuilder.Entity<AProfile>().HasOne(P => P.Pool).WithOne(Q => Q.ORMAProfileAccessor).HasForeignKey<Pool>("AProfileId").IsRequired(false);
            modelBuilder.Entity<BProfile>().HasOne(P => P.Pool).WithOne(Q => Q.ORMBProfileAccessor).HasForeignKey<Pool>("BProfileId").IsRequired(false);

            modelBuilder.Entity<Pool>().Ignore(P => P.Profile);
        }
    }

    public static class ModelBuilderExtensions
    {
        public static void MapAllReadonlyProperty<TOwnerEntity, TDependentEntity>(this OwnedNavigationBuilder<TOwnerEntity, TDependentEntity> builder) where TOwnerEntity : class where TDependentEntity : class
        {
            MapAllReadonlyProperty<TDependentEntity>(builder.Metadata.DeclaringEntityType, builder);
        }

        public static void MapAllReadonlyProperty<T>(this EntityTypeBuilder<T> builder) where T : class
        {
            MapAllReadonlyProperty<T>(builder.Metadata, builder);
        }

        private static void MapAllReadonlyProperty<T>(IMutableEntityType entityType, Microsoft.EntityFrameworkCore.Infrastructure.IInfrastructure<IConventionEntityTypeBuilder> builder)
        {
            var ignores = entityType.GetIgnoredMembers();
            var navigations = entityType.GetNavigations().Select(n => n.Name);
            IEnumerable<PropertyInfo> properties = from property in typeof(T).GetProperties()
                                                   where property.CanWrite == false
                                                   && property.GetCustomAttribute<NotMappedAttribute>() == null
                                                   && !ignores.Any(ignoreProperty => ignoreProperty == property.Name)
                                                   && !navigations.Contains(property.Name)
                                                   select property;
            foreach (var property in properties)
            {
                builder.Instance.Property(property.PropertyType, property.Name);
            }
        }
    }
}