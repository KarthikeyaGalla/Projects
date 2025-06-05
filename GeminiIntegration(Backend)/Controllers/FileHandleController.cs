using GeminiIntegration.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Xceed.Words.NET;
using System.Text;
//using iText.Kernel.Pdf;
//using iText.Kernel.Pdf.Canvas.Parser;
using GeminiIntegration.Interface;
using GeminiIntegration.ModelServices;
using UglyToad.PdfPig;  
using UglyToad.PdfPig.Content;
using Azure.Data.Tables;

[ApiController]
[Route("api/[controller]")]
public class FilesHandleController : ControllerBase
{
    private readonly ITableStorage<ChatHistory> _chatHistoryTable;
    private readonly IGeminiService _geminiService;

    public FilesHandleController(Func<ITableStorage<ChatHistory>> chatHistoryFactory, IGeminiService geminiService)
    {
        _chatHistoryTable = chatHistoryFactory();
        _geminiService = geminiService;
    }


    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var filePath = Path.Combine("UploadedFiles", file.FileName);
        Directory.CreateDirectory("UploadedFiles");

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Ok($"File {file.FileName} uploaded successfully.");
    }

    [HttpPost("read-content")]
    public async Task<IActionResult> ReadFileContent([FromBody] FileDownloadRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FileName))
            return BadRequest("Filename is required.");

        var filePath = Path.Combine("UploadedFiles", request.FileName);
        if (!System.IO.File.Exists(filePath))
            return NotFound("File not found.");

        string extension = Path.GetExtension(filePath).ToLower();
        string content;

        try
        {
            switch (extension)
            {
                case ".txt":
                    content = System.IO.File.ReadAllText(filePath);
                    break;
                case ".pdf":
                    using (var pdf = UglyToad.PdfPig.PdfDocument.Open(filePath))
                    {
                        content = string.Join("\n", pdf.GetPages().Select(p => p.Text));
                    }
                    break;
                default:
                    return BadRequest("Unsupported file format.");
            }

            string prompt = string.IsNullOrWhiteSpace(request.Prompt)
                ? "Summarize the following content:"
                : request.Prompt;

            string fullPrompt = $"{prompt}\n\n{content}";
            string result = await _geminiService.GenerateContentAsync(fullPrompt);

            var chatHistory = new ChatHistory
            {
                Request = prompt,
                Response = result
            };

            await _chatHistoryTable.UpsertAsync(chatHistory);

            return Ok(new { FileName = request.FileName, Result = result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error reading or processing file: {ex.Message}");
        }
    }
}