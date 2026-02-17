using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PseudocodeEditorAPI.Data;

public class PseudocodeDbContextFactory : IDesignTimeDbContextFactory<PseudocodeDbContext>
{
    public PseudocodeDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PseudocodeDbContext>();

        // Keep design-time simple and local.
        // Runtime configuration uses appsettings + DI in Program.cs.
        optionsBuilder.UseSqlite("Data Source=pseudocode.db");

        return new PseudocodeDbContext(optionsBuilder.Options);
    }
}
