using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Comment
{
    public class UpdateCommentDto
    {
        [Required]
        [MinLength(5, ErrorMessage = "Title must be at least 5 characters long")]
        [MaxLength(280, ErrorMessage = "Title must be at most 50 characters long")]
        public string Title { get; set; } = string.Empty;
        [Required]
        [MinLength(5, ErrorMessage = "Title must be at least 5 characters long")]
        [MaxLength(280, ErrorMessage = "Title must be at most 50 characters long")]
        public string Content { get; set; } = string.Empty;

    }
}