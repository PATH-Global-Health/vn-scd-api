using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class UnitType : BaseEntity
    {
        public string Code { get; set; }
        public string TypeName { get; set; }
    }
}
