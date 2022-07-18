using Data.DbContexts;
using Data.Entities.SMDEntities;
using Data.Models.SMDModels;
using Mapster;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.Extenstions
{
    public static class PatientInfoExtensions
    {
        public static IQueryable<PatientInfo> FilterPatientInfo(this IQueryable<PatientInfo> data, PatientInfoFilterModel model)
        {
            if (!string.IsNullOrEmpty(model.TestResult))
            {
                data = data.Where(_ => _.TestResult == model.TestResult);
            }
            if (model.DateUpdatedFrom != null)
                data = data.Where(_ => _.UpdatedDate >= model.DateUpdatedFrom);
            if (model.DateUpdatedTo != null)
                data = data.Where(_ => _.UpdatedDate <= model.DateUpdatedTo);
            if (model.PSNUs != null && model.PSNUs.Count() > 0)
                data = data.Where(_ => model.PSNUs.Contains(_.PSNU));
            if (model.ReferralServices != null && model.ReferralServices.Count() > 0)
                data = data.Where(_ => model.ReferralServices.Contains(_.ServiceName));
            if (model.DateTimes != null && model.DateTimes.Count() > 0)
            {
                var datetimes = model.DateTimes.ToReportDateTime();
                data = data.Where(_ => datetimes.Contains(_.ReportingPeriod));
            }
            return data;
        }

        public static EntityEntry<PatientInfo> UpdatePatientInfo(this AppDbContext context, PatientInfo oldEntity, PatientInfo newEntity, string username)
        {
            // create new history
            PatientInfoHistory history = oldEntity.Adapt<PatientInfoHistory>();
            history.PatientInfoId = oldEntity.Id;
            context.PatientInfoHistories.Add(history);
            // update report
            newEntity.CreateBy = username;
            newEntity.DateUpdated = DateTime.Now;
            newEntity.Adapt(oldEntity);
            return context.PatientInfos.Update(oldEntity);
        }
    }
}
