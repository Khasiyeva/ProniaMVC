using System.ComponentModel.DataAnnotations;

namespace Pronia.Models
{
    public class Slider
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string ImgUrl { get; set; }
    }
}
