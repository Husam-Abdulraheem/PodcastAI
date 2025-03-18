namespace PodcastAI.DTOs
{
    public class RequestDTO
    {
        public string? Message { get; set; }
        public bool Success { get; set; }
        public object? Data { get; set; } = new List<object>();

    }
}
