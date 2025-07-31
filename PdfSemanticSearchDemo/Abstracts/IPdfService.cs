using PdfSemanticSearchDemo.Models;

namespace PdfSemanticSearchDemo.Abstracts;

public interface IPdfService
{
    Task<List<DocumentChunk>> ExtractTextFromPdfAsync(Stream pdfStream, string fileName);
}
