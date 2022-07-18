using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models.SMDModels
{
    public class TargetCreateModel
    {
        public Guid IPackageId { get; set; }
        public Guid IndicatorId { get; set; }
        public int Quantity { get; set; }
    }

    public class TargetUpdateModel : TargetCreateModel
    {
        public Guid Id { get; set; }
    }

    public class TargetViewModel : TargetUpdateModel { }
}
