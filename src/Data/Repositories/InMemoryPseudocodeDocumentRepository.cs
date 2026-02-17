using PseudocodeEditorAPI.Models;

namespace PseudocodeEditorAPI.Data.Repositories;

/// <summary>
/// Simple in-memory repository intended for unit tests.
/// </summary>
public class InMemoryPseudocodeDocumentRepository : IPseudocodeDocumentRepository
{
    private readonly object _lock = new();
    private readonly List<PseudocodeDocument> _documents = new();

    public Task<IReadOnlyList<PseudocodeDocument>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult<IReadOnlyList<PseudocodeDocument>>(_documents
                .OrderByDescending(d => d.UpdatedAt)
                .ToList());
        }
    }

    public Task<PseudocodeDocument?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var document = _documents.FirstOrDefault(d => d.Id == id);
            return Task.FromResult(document);
        }
    }

    public Task<PseudocodeDocument> CreateAsync(PseudocodeDocument document, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _documents.Add(document);
            return Task.FromResult(document);
        }
    }

    public Task<PseudocodeDocument?> UpdateAsync(PseudocodeDocument document, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var existing = _documents.FirstOrDefault(d => d.Id == document.Id);
            if (existing == null)
            {
                return Task.FromResult<PseudocodeDocument?>(null);
            }

            existing.Title = document.Title;
            existing.Content = document.Content;
            existing.Language = document.Language;
            existing.UpdatedAt = document.UpdatedAt;

            return Task.FromResult<PseudocodeDocument?>(existing);
        }
    }

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var existing = _documents.FirstOrDefault(d => d.Id == id);
            if (existing == null)
            {
                return Task.FromResult(false);
            }

            _documents.Remove(existing);
            return Task.FromResult(true);
        }
    }
}
