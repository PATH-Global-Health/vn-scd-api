using Services.LookupServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.SMDServices
{
    public class SMDBaseService
    {
        protected readonly SMDUserLookupService _UserLookupService;

        public SMDBaseService(SMDUserLookupService userLookupService)
        {
            _UserLookupService = userLookupService;
        }
    }
}
