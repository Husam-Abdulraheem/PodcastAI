using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PodcastAI.DTOs;
using PodcastAI.Models;
using System.Security.AccessControl;

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







        [HttpPost("podcast-details")]
        public async Task<IActionResult> PodcastDetails([FromForm] ResponseDTO dto)
        {
            if (dto == null)
                return BadRequest(new RequestDTO { Message = "Invalid request data." });

            string? audioPath = null;
            string? imagePath = null;

            if (dto.Audio != null && dto.Audio.Length > 0)
            {
                var resultAudio = await _fileService.ProcessFile(dto.Audio, new[] { ".mp4", ".mp3", ".wav" });
                if (!resultAudio.Success)
                    return BadRequest(new RequestDTO { Message = "Audio upload failed", Data = resultAudio.Message });
                audioPath = resultAudio.Data?.ToString();
            }
            if (dto.Image != null && dto.Image.Length > 0)
            {
                var resultImage = await _fileService.ProcessFile(dto.Image, new[] { ".jpg", ".png", ".jpeg", ".webp" });
                if (!resultImage.Success)
                    return BadRequest(new RequestDTO { Message = "Image upload failed", Data = resultImage.Message });
                imagePath = resultImage.Data?.ToString();
            }



            var podcast = new Podcast
            {
                Subject = dto.Subject,
                Content = dto.Content,
                Size = dto.Size,
                AudioUrl = $"https://www.podcastai.somee.com/uploads/{audioPath}",
                ImageUrl = $"https://www.podcastai.somee.com/uploads/{imagePath}",
                Special = false
            };

            await _db.podcasts.AddAsync(podcast);
            await _db.SaveChangesAsync();

            return Ok(new RequestDTO { Success = true, Message = "Podcast uploaded successfully." });
        }










        [HttpGet("get-special-podcast")]
        public async Task<IActionResult> GetSpecialPodcast()
        {
            var specialPodcast = await _db.podcasts.Where(p => p.Special).ToListAsync();
            if(specialPodcast == null)
            {
                return Ok(new RequestDTO {Message = "There is no special podcast yet", Success = true });
            }
            return Ok(new RequestDTO { Data = specialPodcast, Success = true });
        }








        [HttpGet("get-podcasts")]
        public async Task<IActionResult> GetPodcasts()
        {
            var specialPodcast = await _db.podcasts.Where(p => !p.Special).ToListAsync();
            if (specialPodcast == null)
            {
                return Ok(new RequestDTO { Message = "There is no special podcast yet", Success = true });
            }
            return Ok(new RequestDTO { Data = specialPodcast, Success = true });
        }









        [HttpPost("special-podcast")]
        public async Task<IActionResult> SpecialPodcast([FromForm] ResponseDTO dto)
        {
            if (dto == null)
                return BadRequest(new RequestDTO { Message = "Invalid request data." });

            string? audioPath = null;
            string? imagePath = null;

            if (dto.Audio != null && dto.Audio.Length > 0)
            {
                var resultAudio = await _fileService.ProcessFile(dto.Audio, new[] { ".mp4", ".mp3", ".wav" });
                if (!resultAudio.Success)
                    return BadRequest(new RequestDTO { Message = "Audio upload failed", Data = resultAudio.Message });
                audioPath = resultAudio.Data?.ToString();
            }

            if (dto.Image != null && dto.Image.Length > 0)
            {
                var resultImage = await _fileService.ProcessFile(dto.Image, new[] { ".jpg", ".png", ".jpeg", ".webp" });
                if (!resultImage.Success)
                    return BadRequest(new RequestDTO { Message = "Image upload failed", Data = resultImage.Message });
                imagePath = resultImage.Data?.ToString();
            }


            var podcast = new Podcast
            {
                Subject = dto.Subject,
                Content = dto.Content,
                Size = dto.Size,
                AudioUrl = $"https://www.podcastai.somee.com/uploads/{audioPath}",
                ImageUrl = $"https://www.podcastai.somee.com/uploads/{imagePath}",
                Special = true
            };

            await _db.podcasts.AddAsync(podcast);
            await _db.SaveChangesAsync();

            return Ok(new RequestDTO { Success = true, Message = "Special Podcast uploaded successfully." });
        }





    }










    public class FileService
    {
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<RequestDTO> ProcessFile(IFormFile file, string[] allowedExtensions)
        {
            if (file == null || file.Length == 0)
                return new RequestDTO { Success = false, Message = "The file is invalid." };

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                return new RequestDTO { Success = false, Message = "File extension not supported." };

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
            var fullPath = Path.Combine(uploadsPath, uniqueFileName);

            try
            {
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            catch (Exception)
            {
                return new RequestDTO { Success = false, Message = "Failed to save file." };
            }

            return new RequestDTO { Success = true, Data = uniqueFileName };
        }
    }
}
