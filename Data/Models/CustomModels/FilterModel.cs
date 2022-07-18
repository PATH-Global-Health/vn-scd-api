using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models.CustomModels
{
    public interface IFilterModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
