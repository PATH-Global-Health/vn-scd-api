using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Constants
{
    public enum WorkingCalendarStatus
    {
        NOT_POST,
        POSTED,
        CANCEL_POST
    }
    public enum PermissionType
    {
        Allow = 1,
        Deny = 0
    }

    public enum FunctionFacility
    {
        TESTING,
        PrEP,
        ART
    }

    public enum StatusTicket : int
    {
        SENT,
        RECEIVED,
        CANCEL
    }


    public enum ReferType
    {
        TESTING,
        PrEP,
        ART
    }

    public enum TypeFacitily
    {
        FACILITY,
        EMPLOYEE
    }

    public enum ReportPeriod
    {
        MONTH,
        QUARTER
    }

    public enum Role
    {
        UNKNOWN,
        SMD_PROJECT,
        SMD_CBO,
        SMD_ADMIN
    }

    public enum ReportValueType
    {
        INTEGER,
        PERCENTAGE,
        MONEY
    }

    public enum ReportGroupByType
    {
        PROJECTOR,
        TIME,
        PROVINCE,
        CBO
    }

    public enum CreatedMethod
    {
        NORMAL,
        IMPORT
    }

    public enum ReadType
    {
        ALL,
        NORMAL_ONLY,
        PAYMENT_ONLY
    }

    public enum IndicatorType
    {
        OTHER,
        POSITIVE,
        NEGATIVE,
        ARV_TRANSPORT,
        TX_NEW,
        ARV_PENDING,
        PREP_TRANSPORT,
        PREP_NEW,
        PREP_PENDING
    }

    public enum AllowInputType
    {
        INDIVIDUAL,
        AGRREGATE
    }

    public enum BaseSortCriteria
    {
        DateCreated,
        FullName
    }

    public enum ItemSort
    {
        DateCreated,
        Name
    }
}
