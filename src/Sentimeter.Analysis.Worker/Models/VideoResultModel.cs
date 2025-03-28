using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentimeter.Analysis.Worker.Models
{
    public class VideoResultModel
    {
        [Required]
        public Guid VideoId { get; set; } 

        [Required]
        public string Result { get; set; } = string.Empty;

        [Required]
        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;

    }
}
