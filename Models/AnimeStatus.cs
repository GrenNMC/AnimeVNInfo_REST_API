﻿namespace AnimeVnInfoBackend.Models
{
    public class AnimeStatus : Base
    {
        public string? StatusName { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }

        // List of tables connected
        public List<Anime>? Anime { get; set; }
    }
}
