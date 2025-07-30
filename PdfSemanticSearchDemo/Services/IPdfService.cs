using PdfSemanticSearchDemo.Models;

namespace PdfSemanticSearchDemo.Services;

public interface IPdfService
{
    Task<List<DocumentChunk>> ExtractTextFromPdfAsync(Stream pdfStream, string fileName);
}
