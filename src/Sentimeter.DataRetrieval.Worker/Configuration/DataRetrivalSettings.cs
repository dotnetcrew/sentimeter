using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentimeter.DataRetrieval.Worker.Configuration;

public class DataRetrivalSettings
{
    public const string JsonSection = "AkkaSettings";

    public bool Enabled = true;

    /// <summary>
    /// Schedule every interval
    /// </summary>
    public int SchedulerIntervalSeconds { get; set; } = 600;

    /// <summary>
    /// Items to consider at each schedule
    /// </summary>
    public int SchedulerWorkingItems { get; set; } = 100;

    /// <summary>
    /// Number of workers for queue
    /// </summary>
    public int WorkersCount { get; set; } = 2;


}
