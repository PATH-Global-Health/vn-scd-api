using Data.Entities;
using Data.Entities.SMDEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.Extenstions
{
    public static class PagingExtensions
    {
        public static int PageCount<T>(this IEnumerable<T> data, int pageSize)
        {
            return (int)Math.Ceiling(data.Count() / (double)pageSize);
        }

        public static IQueryable<T> PageData<T>(this IQueryable<T> data, int pageIndex, int pageSize) where T : class
        {
            return data.Skip(pageIndex * pageSize).Take(pageSize).AsNoTracking();
        }

        public static IEnumerable<T> PageData<T>(this IEnumerable<T> data, int pageIndex, int pageSize) where T : class
        {
            return data.Skip(pageIndex * pageSize).Take(pageSize);
        }

        public static IQueryable<T> BaseFilter<T>(this IQueryable<T> data) where T : BaseEntity
        {
            return data.Where(_ => !_.IsDeleted);
        }

        public static IQueryable<KPI> FilterKPI(this IQueryable<KPI> data, Guid indicatorId)
        {
            return data.Where(_ => _.IndicatorId == indicatorId);
        }

        public static IQueryable<Unit> FilterUnit(this IQueryable<Unit> data, Guid projectId, string searchValue)
        {
            if (searchValue != null)
            {
                data = data.Where(_ => _.Name.Contains(searchValue));
            }
            data = data.Where(_ => _.ProjectId == projectId);
            return data;
        }

        public static IQueryable<Unit> FilterUnit(this IQueryable<Unit> data, string projectUsername, string searchValue)
        {
            if (searchValue != null)
            {
                data = data.Where(_ => _.Name.Contains(searchValue));
            }
            data = data.Where(_ => _.Project.Username == projectUsername);
            return data;
        }

        public static IQueryable<PatientInfo> FilterPatientInfo(this IQueryable<PatientInfo> data, ICollection<string> cboNames)
        {
            if (cboNames != null)
                data = data.Where(_ => cboNames.Contains(_.CBOName));
            return data;
        }
    }
}
