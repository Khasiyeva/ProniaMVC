using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronia.Models
{
    public class Slider
    {
        public int Id { get; set; }
        [Required, StringLength(25, ErrorMessage = "Maximum length can be 25")]
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        [StringLength(maximumLength: 100)]
        public string? ImgUrl { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
