using Data.Entities.SMDEntities;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Services.LookupServices
{
    public class PackageLookupService : LookupServiceBase<Package>
    {
        public PackageLookupService(IServiceScopeFactory scopeFactory) : base(scopeFactory)
        {
        }

        public override Package Lookup(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException("PackageCode");
            return base.Lookup(code);
        }
    }
}
