﻿namespace PodcastAI.DTOs
{
    public class ResponseDTO
    {
        public string? Subject { get; set; }
        public string? Size { get; set; }
        public string? Content { get; set; }
        public IFormFile? Audio { get; set; }
        public IFormFile? Image { get; set; }
        public bool? IsPublic { get; set; }

    }
}
