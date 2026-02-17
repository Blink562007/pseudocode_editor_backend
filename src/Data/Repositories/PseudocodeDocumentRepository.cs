using Microsoft.EntityFrameworkCore;
using PseudocodeEditorAPI.Models;

namespace PseudocodeEditorAPI.Data.Repositories;

public class PseudocodeDocumentRepository : IPseudocodeDocumentRepository
{
    private readonly Data.PseudocodeDbContext _db;

    public PseudocodeDocumentRepository(Data.PseudocodeDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<PseudocodeDocument>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Documents
            .AsNoTracking()
            .OrderByDescending(d => d.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<PseudocodeDocument?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        return await _db.Documents
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<PseudocodeDocument> CreateAsync(PseudocodeDocument document, CancellationToken cancellationToken = default)
    {
        _db.Documents.Add(document);
        await _db.SaveChangesAsync(cancellationToken);
        return document;
    }

    public async Task<PseudocodeDocument?> UpdateAsync(PseudocodeDocument document, CancellationToken cancellationToken = default)
    {
        var existing = await _db.Documents
            .FirstOrDefaultAsync(d => d.Id == document.Id, cancellationToken);

        if (existing == null)
        {
            return null;
        }

        existing.Title = document.Title;
        existing.Content = document.Content;
        existing.Language = document.Language;
        existing.UpdatedAt = document.UpdatedAt;

        await _db.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return false;
        }

        var existing = await _db.Documents.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        if (existing == null)
        {
            return false;
        }

        _db.Documents.Remove(existing);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
