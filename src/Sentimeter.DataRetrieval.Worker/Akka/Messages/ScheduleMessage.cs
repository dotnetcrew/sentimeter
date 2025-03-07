using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentimeter.DataRetrieval.Worker.Akka.Messages;

public class ScheduleMessage
{
    public int SchedulerWorkingItems { get; }

    public ScheduleMessage(int schedulerWorkingItems)
    {
        SchedulerWorkingItems = schedulerWorkingItems;
    }
}
