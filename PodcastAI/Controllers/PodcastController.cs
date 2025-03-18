using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using PodcastAI.DTOs;
using PodcastAI.Models;

namespace PodcastAI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PodcastController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly FileService _fileService;

        public PodcastController(AppDbContext db, FileService fileService)
        {
            _db = db;
            _fileService = fileService;

        }
        [HttpPost]
        public async Task<IActionResult> PodcastDetails(ResponseDTO dto)
        {

            string? invoicePath = null;
            if (dto.Audio != null && dto.Audio.Length > 0)
            {
                var result = await _fileService.ProcessReceipt(dto.Audio);

                if (!result.Success)
                    return Ok(new RequestDTO { Message = result.Message });
                invoicePath = result.Data.ToString();
            }
                var podcast = new Podcast
            {
                Subject = dto.Subject,
                Content = dto.Content,
                Size = dto.Size,
                AudioUrl = invoicePath,

            };
            var data = await _db.podcasts.AddAsync(podcast);

            return Ok(new RequestDTO { Success = true });
        }
    }

    public class FileService
    {
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }
        public async Task<RequestDTO> ProcessReceipt(IFormFile file)
        {

            if (file == null || file.Length == 0)
            {
                return new RequestDTO { Message = "The file is invalid." };
            }

            // Check allowed extensions (eg: mp4)
            var allowedExtensions = new[] { ".mp4", ".mp3", ".wav" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                return new RequestDTO { Message = "File extension not supported." };
            }

            // Create a unique file name while preserving the extension.
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            // Final path to save
            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
            var fullPath = Path.Combine(uploadsPath, uniqueFileName);

            // Create the folder if it does not exist
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // save in server

            try
            {
                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {

                return new RequestDTO
                {
                    Message = "Failed to save file"
                };
            }


            return new RequestDTO { Data = uniqueFileName, Success = true };
        }
    }
}
