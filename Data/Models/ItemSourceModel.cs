using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class ItemSourceModel
    {
        public string Name { get; set; }

    }

    public class ItemSourceCreateModel : ItemSourceModel
    {

    }

    public class ItemSourceViewModel : ItemSourceModel
    {
        public Guid Id { get; set; }
    }

    public class ItemSourceUpdateModel : ItemSourceModel
    {
    }

}
