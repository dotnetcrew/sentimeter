using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentimeter.Shared.Messages.RabbitMQ;

public record AnalyzeVideoMessage(Guid VideoId);
