using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentimeter.DataRetrieval.Worker.Akka.Messages;

public class ChangeServiceStatusMessage
{
    public ChangeServiceStatusMessage(bool enabled)
    {
        Enabled = enabled;
    }

    public bool Enabled { get; }

    public override string ToString()
    {
        return $"[Enabled: {Enabled}]";
    }
}