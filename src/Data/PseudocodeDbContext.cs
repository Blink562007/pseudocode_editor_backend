using Microsoft.EntityFrameworkCore;
using PseudocodeEditorAPI.Models;

namespace PseudocodeEditorAPI.Data;

public class PseudocodeDbContext : DbContext
{
    public PseudocodeDbContext(DbContextOptions<PseudocodeDbContext> options)
        : base(options)
    {
    }

    public DbSet<PseudocodeDocument> Documents => Set<PseudocodeDocument>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var documents = modelBuilder.Entity<PseudocodeDocument>();

        documents.ToTable("PseudocodeDocuments");
        documents.HasKey(d => d.Id);

        documents.Property(d => d.Id)
            .HasMaxLength(36);

        documents.Property(d => d.Title)
            .HasMaxLength(200);

        documents.Property(d => d.Language)
            .HasMaxLength(50);

        documents.Property(d => d.Content)
            .IsRequired();

        documents.HasIndex(d => d.UpdatedAt);
    }
}
