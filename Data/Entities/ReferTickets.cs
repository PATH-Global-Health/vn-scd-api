using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Data.Constants;

namespace Data.Entities
{
    public class ReferTickets:BaseEntity
    {
        public ReferType Type { get; set; } 
        public StatusTicket Status { get; set; } // vừa tạo- đã chuyển
        public DateTime ReferDate { get; set; } // ngày chuyển (chọn)
        public DateTime ReceivedDate { get; set; } // ngày nhận (chọn)
        public string Note { get; set; }
        public string EmployeeId { get; set; }
        public Guid? FromUnitId { get; set; }
        [ForeignKey("FromUnitId")]
        public virtual Unit FromUnit { get; set; }

        public Guid? ToUnitId { get; set; }
        [ForeignKey("ToUnitId")]
        public virtual Unit ToUnit { get; set; }

        public Guid? ProfileId { get; set; }
        [ForeignKey("ProfileId")]
        public virtual Profile Profile { get; set; }

    }
}
