using System;
using Labradoratory.Fetch.ChangeTracking;
using Labradoratory.Fetch.Processors;
using Microsoft.EntityFrameworkCore;

namespace Labradoratory.Fetch.EntityFrameworkCore.Test
{
    public class TestEntity : Entity
    {
        public override object[] DecodeKeys(string encodedKeys)
        {
            throw new System.NotImplementedException();
        }

        public override string EncodeKeys()
        {
            throw new System.NotImplementedException();
        }

        public override object[] GetKeys()
        {
            return ToKeys(Id);
        }

        public int Id { get; set; }

        public string StringValue
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public int IntValue
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public double DoubleValue
        {
            get => GetValue<double>();
            set => SetValue(value);
        }

        public DateTimeOffset DateTimeValue
        {
            get => GetValue<DateTimeOffset>();
            set => SetValue(value);
        }

        public TestEntityChild Child
        {
            get => GetValue<TestEntityChild>();
            set => SetValue(value);
        }

        public ChangeTrackingCollection<string> StringCollection
        {
            get => GetValue<ChangeTrackingCollection<string>>();
            set => SetValue(value);
        }
    }

    public class TestEntityChild
    {
        public string StringValue { get; set; }
    }

    public class TestContext : DbContext
    {
        public TestContext(DbContextOptions<TestContext> options)
            : base(options)
        { }

        public DbSet<TestEntity> TestEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestEntity>().HasKey(e => e.Id);
            modelBuilder.Entity<TestEntity>().OwnsOne(e => e.Child);
            modelBuilder.Entity<TestEntity>()
                .Property(e => e.StringCollection)
                .HasConversion(
                    v => string.Join(" | ", v),
                    v => new ChangeTrackingCollection<string>(v.Split(" | ", StringSplitOptions.None)));

            base.OnModelCreating(modelBuilder);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "Test classes")]
    public class TestRepository : EntityFrameworkCoreRepository<TestEntity, TestContext>, IDisposable
    {
        public TestRepository(TestContext context, IProcessorProvider processorProvider)
            : base(context, new ProcessorPipeline(processorProvider))
        { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "Test classes")]
        public void Dispose()
        {
            Context.Dispose();
        }

        public DbContext TestGetContext()
        {
            return Context;
        }
    }
}
