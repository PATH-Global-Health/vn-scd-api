using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Constants
{
    public class RabbitQueue
    {
        //Booking
        public const string BOOKING_INTERVAL = "BookingDev1";
        public const string EXIST_EXTERNAL_ID = "IsExistExternalIdDev1";
        public const string ADD_REFER_TICKET = "AddReferTicketDev1";
        public const string SET_STATUS_PROFILE = "SetStatusProfileDev1";

        //User
        public const string CONFIRMED_CUSTOMER = "ConfirmedCustomerStatusDev1"; //ConfirmedCustomerStatus10
        public const string CHECK_CONFIRMED_CUSTOMER = "CheckConfirmedCustomerStatusDev1"; //CheckConfirmedUserQueue
        public const string CREATE_ACCOUNT = "CreateAccountDev1"; //queue4
        public const string DELETE_USER = "DeleteUserDev1"; //DeleteUsersQueue
    }
}
