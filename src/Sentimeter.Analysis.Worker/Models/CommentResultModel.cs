using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentimeter.Analysis.Worker.Models;

public class CommentResultModel
{
    [Required]
    public Guid CommentId { get; set; }

    [Required]
    public string Result { get; set; } = string.Empty;

    [Required]
    public DateTime LastUpdate { get; set; } = DateTime.UtcNow;

    public double Score { get; set; } = 0.0;

}
