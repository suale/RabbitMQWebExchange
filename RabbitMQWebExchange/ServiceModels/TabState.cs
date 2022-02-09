using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RabbitMQWebExchange.ServiceModels
{
    public class TabState
    {
        public int LastDataNumber { get; set; }
        public string ClientGuid { get; set; }
        public string TabGuid { get; set; }
        public List<RabbitMessage> LastChartList { get; set; }
    }
}