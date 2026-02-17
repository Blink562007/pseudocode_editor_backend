using PseudocodeEditorAPI.Models;

namespace PseudocodeEditorAPI.Data.Repositories;

public interface IPseudocodeDocumentRepository
{
    Task<IReadOnlyList<PseudocodeDocument>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PseudocodeDocument?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<PseudocodeDocument> CreateAsync(PseudocodeDocument document, CancellationToken cancellationToken = default);
    Task<PseudocodeDocument?> UpdateAsync(PseudocodeDocument document, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
