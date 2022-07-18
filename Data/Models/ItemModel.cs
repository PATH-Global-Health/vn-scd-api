using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class ItemModel
    {
        public string Name { get; set; }
    }

    public class ItemCreateModel: ItemModel
    {
        
    }

    public class ItemViewModel : ItemModel
    {
        public Guid Id { get; set; }
    }

    public class ItemUpdateModel : ItemModel
    {
    }


}
