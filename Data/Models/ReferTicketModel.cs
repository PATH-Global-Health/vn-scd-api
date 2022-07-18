using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Data.Constants;
using Data.Entities;

namespace Data.Models
{
    public class ReferTicketModel:BaseEntity
    {
       
    }

    public class ReferTicketCreateModel 
    {
        [Required]
        public Guid ProfileId { get; set; }
        [Required]
        public Guid ToUnitId { get; set; }

        public ReferType Type { get; set; }
        public DateTime ReferDate { get; set; }
        public string Note { get; set; }
    }

    public class ReferTicketViewModel: ReferTicketModel
    {
        public Guid? ProfileId { get; set; }
        public Guid? FromUnitId { get; set; }
        public Guid? ToUnitId { get; set; }
        public StatusTicket Status { get; set; }
        public ReferType Type { get; set; }
        public DateTime ReferDate { get; set; }
        public DateTime ReceivedDate { get; set; } // ngày nhận (chọn)
        public string Note { get; set; }
        public string EmployeeId { get; set; }
        public ProfileViewModel Profile { get; set; }
        public UnitModel FromUnit { get; set; }
        public UnitModel ToUnit { get; set; }
    }

    public class ReceiveTicketModel
    {
        [Required]
        public DateTime ReceivedDate { get; set; }
        [Range(0, 2)]
        [Required]
        public StatusTicket Status { get; set; }
    }

    public class TicketViewModel : ReferTicketModel
    {
        public ReferType Type { get; set; }
        public DateTime ReferDate { get; set; }
        public DateTime ReceivedDate { get; set; } // ngày nhận (chọn)
        public string Note { get; set; }
        public ProfileViewModel Profile { get; set; }
        public UnitModel FromUnit { get; set; }
        public UnitModel ToUnit { get; set; }
    }

    public class TicketEmployeeModel
    {
        public Guid? ProfileId { get; set; }
        public Guid? ToUnitId { get; set; }
        public Guid? FromUnitId { get; set; }
        public string EmployeeId { get; set; }
        public ReferType Type { get; set; }

    }






}
