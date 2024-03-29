﻿using System.ComponentModel.DataAnnotations;

namespace LightGallery.Models;

public class Gallery
{
    [Key]
    public Guid Id { get; set; }
    public User Owner { get; set; }
    public ICollection<GalleryFile> Files { get; set; }
    public ICollection<Tag> Tags { get; set; }
    
    [Required]
    [MinLength(5)]
    [MaxLength(128)]
    public string Title { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public long TotalGallerySize { get; set; } = 0;
    public int LastFolderIndex { get; set; } = 0;
    public int LastFileIndex { get; set; } = 0;
}