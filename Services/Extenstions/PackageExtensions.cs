using Data.Entities.SMDEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.Extenstions
{
    public static class PackageExtensions
    {
        public static IQueryable<Contract> FilterContract(this IQueryable<Contract> data, Guid? cboId, Guid? packageId)
        {
            if (cboId != null)
                data = data.Where(x => x.CBOId == cboId);
            if (packageId != null)
                data = data.Where(x => x.ImplementPackage.PackageId == packageId);
            return data;
        }

        public static IQueryable<ImplementPackage> FilterIPackage(this IQueryable<ImplementPackage> data, Guid? packageId, string province)
        {
            if (packageId != null)
                data = data.Where(x => x.PackageId == packageId);
            if (province != null)
                data = data.Where(x => x.Province == province);
            return data;
        }

        public static IQueryable<Target> FilterTarget(this IQueryable<Target> data, Guid? ipackageId)
        {
            if (ipackageId != null)
                data = data.Where(x => x.IPackageId == ipackageId);
            return data;
        }

        public static IQueryable<Contract> OverlapContract(this IQueryable<Contract> data, Contract contract)
        {
            data = data.BaseFilter().Where(x => x.CBOId == contract.CBOId)
                                .Where(x => !(x.Start >= contract.End || x.End <= contract.Start));
            return data;
        }
    }
}
