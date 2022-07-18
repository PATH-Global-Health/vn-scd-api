using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class IntervalCreateModel
    {
        public string From { get; set; }
        public string To { get; set; }
    }

    public class OrderIntervalModel
    {
        public Guid Id { get; set; }
        public bool IsAvailable { get; set; }
        public int AvailableQuantity { get; set; }

    }
}
