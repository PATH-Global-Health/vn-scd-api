using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Constants
{
    public static class ErrorMessages
    {
        public static string ID_NOT_FOUND = "Id not found.";

        public static string CALENDAR_POSTED = "Calendar already posted.";
        public static string CALENDAR_CANCELED = "Calendar already cancel.";

        public static string DUPLICATE_CODE = "Code existed.";

        #region SMD
        public static string IPACKAGE_HAS_CONTRACTS = "There is/are contract(s) sign to this package.";
        public static string PROVINCE_CONFLICTED = "Package's province and CBO's province is not the same.";
        public static string CONTRACT_ENDED = "Contract not found.";
        public static string INVALID_IMPORT_INDICATOR = "Indicator invalid or not allowed by this kind of import: ";
        public static string NOT_ASSOCIATED_CBO = "CBO not associated with this account: ";
        public static string CHANGES_NOT_ALLOWED = "This entity is base entity and does not allow changes.";
        public static string FILE_NOT_FOUND = "File not found.";
        public static string ROLE_NOT_SUITABLE = "Account role not allow to do this action.";
        public static string CBO_NOT_FOUND = "CBO not found.";
        public static string INPUT_NOT_ALLOW = "This CBO is not allow to use this input method.";
        public static string DUPLICATE_DATA = "Data duplicate.";
        #endregion
    }
}
