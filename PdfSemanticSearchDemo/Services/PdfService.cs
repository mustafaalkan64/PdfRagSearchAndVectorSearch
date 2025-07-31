using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using PdfSemanticSearchDemo.Abstracts;
using PdfSemanticSearchDemo.Models;

namespace PdfSemanticSearchDemo.Services;

public class PdfService : IPdfService
{
    public async Task<List<DocumentChunk>> ExtractTextFromPdfAsync(Stream pdfStream, string fileName)
    {
        var chunks = new List<DocumentChunk>();
        
        using var pdfReader = new PdfReader(pdfStream);
        using var pdfDocument = new PdfDocument(pdfReader);
        
        for (int pageNum = 1; pageNum <= pdfDocument.GetNumberOfPages(); pageNum++)
        {
            var page = pdfDocument.GetPage(pageNum);
            var strategy = new SimpleTextExtractionStrategy();
            var pageText = PdfTextExtractor.GetTextFromPage(page, strategy);
            
            if (!string.IsNullOrWhiteSpace(pageText))
            {
                // Split text into chunks of approximately 500 characters
                var textChunks = SplitTextIntoChunks(pageText, 500);
                
                for (int i = 0; i < textChunks.Count; i++)
                {
                    chunks.Add(new DocumentChunk
                    {
                        Content = textChunks[i],
                        FileName = fileName,
                        PageNumber = pageNum,
                        ChunkIndex = i
                    });
                }
            }
        }
        
        return chunks;
    }
    
    private List<string> SplitTextIntoChunks(string text, int maxChunkSize)
    {
        var chunks = new List<string>();
        var sentences = text.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
        
        var currentChunk = "";
        
        foreach (var sentence in sentences)
        {
            var trimmedSentence = sentence.Trim();
            if (string.IsNullOrEmpty(trimmedSentence)) continue;
            
            if (currentChunk.Length + trimmedSentence.Length + 1 <= maxChunkSize)
            {
                currentChunk += (currentChunk.Length > 0 ? ". " : "") + trimmedSentence;
            }
            else
            {
                if (!string.IsNullOrEmpty(currentChunk))
                {
                    chunks.Add(currentChunk + ".");
                }
                currentChunk = trimmedSentence;
            }
        }
        
        if (!string.IsNullOrEmpty(currentChunk))
        {
            chunks.Add(currentChunk + ".");
        }
        
        return chunks;
    }
}
