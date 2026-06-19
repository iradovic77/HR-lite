namespace CodebookService.DTOs;

/// <summary>Rezultat operacije fizičkog brisanja zapisa.</summary>
public record DeleteResult(bool Found, bool HasReferences);
