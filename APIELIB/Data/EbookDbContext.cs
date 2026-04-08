using Microsoft.EntityFrameworkCore;
using APIELIB.Models;

namespace APIELIB.Data;

/// <summary>
/// DbContext chính cho hệ thống thư viện số
/// </summary>
public class EbookDbContext : DbContext
{
    public EbookDbContext(DbContextOptions<EbookDbContext> options) : base(options)
    {
    }

    public DbSet<Collection> Collections { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<ItemXml> ItemXmls { get; set; }
    public DbSet<MetaDataValue> MetaDataValues { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Topic> Topics { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cấu hình quan hệ Item - ItemXml (1-1)
        modelBuilder.Entity<Item>()
            .HasOne(i => i.ItemXml)
            .WithOne(ix => ix.Item)
            .HasForeignKey<ItemXml>(ix => ix.Id);

        // Cấu hình quan hệ Item - Collection (N-1)
        modelBuilder.Entity<Item>()
            .HasOne(i => i.Collection)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.CollectionId)
            .IsRequired(false);

        // Cấu hình quan hệ Item - Subject (N-1)
        modelBuilder.Entity<Item>()
            .HasOne(i => i.Subject)
            .WithMany(s => s.Items)
            .HasForeignKey(i => i.SubjectId)
            .IsRequired(false);

        // Cấu hình quan hệ Item - Topic (N-1)
        modelBuilder.Entity<Item>()
            .HasOne(i => i.Topic)
            .WithMany(t => t.Items)
            .HasForeignKey(i => i.TopicId)
            .IsRequired(false);

        // Cấu hình quan hệ MetaDataValue - Item (N-1)
        modelBuilder.Entity<MetaDataValue>()
            .HasOne(m => m.Item)
            .WithMany(i => i.MetaDataValues)
            .HasForeignKey(m => m.ItemId)
            .IsRequired(false);

        // Map cột value_UnSign vào property ValueUnSign
        modelBuilder.Entity<MetaDataValue>()
            .Property(m => m.ValueUnSign)
            .HasColumnName("value_UnSign");
    }
}
