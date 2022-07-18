using Data.Entities.SMDEntities;
using Microsoft.AspNetCore.Mvc;
using ScheduleManagement.Extensions;
using Services.LookupServices;

namespace ScheduleManagement.Controllers.BaseController
{
    public class SMDControllerBase : ControllerBase
    {
        protected readonly SMDUserLookupService _UserLookupService;

        public SMDControllerBase(SMDUserLookupService userLookupService)
        {
            _UserLookupService = userLookupService;
        }

        protected CustomUser GetCustomUser()
        {
            CustomUser user = this.User.GetUser();
            if (user != null && user.Role == Data.Constants.Role.SMD_PROJECT)
                user.UnitId = _UserLookupService.LookupUnitId(user.Username);
            return user;
        }
    }
}
