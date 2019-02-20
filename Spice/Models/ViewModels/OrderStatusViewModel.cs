using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.ViewModels
{
    public class OrderStatusViewModel
    {
        public string OrderPlaced { get; set; }
        public string OrderKitchen { get; set; }
        public string OrderReady { get; set; }
        public string OrderComplete { get; set; }
    }
}
