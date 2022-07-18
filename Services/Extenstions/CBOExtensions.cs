using Data.Constants;
using Data.DbContexts;
using Data.Entities;
using Data.Entities.SMDEntities;
using Data.Models.SMDModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Extenstions
{
    public static class CBOExtensions
    {
        public static async Task<Unit> GetUnitByCodeAsync(this IQueryable<Unit> data, string code)
        {
            return await data.BaseFilter().FirstOrDefaultAsync(x => x.Code == code);
        }

        public static async Task<Unit> GetCBOByUsernameAsync(this IQueryable<Unit> data, string username)
        {
            var cbo = await data.BaseFilter().FirstOrDefaultAsync(x => x.Username == username);
            if (cbo == null)
            {
                throw new Exception("Current Account is not a CBO.");
            }
            return cbo;
        }

        public static IQueryable<Unit> GetCBOsByUsernameAsync(this IQueryable<Unit> data, string username)
        {
            var cbos = data.BaseFilter().Where(x => x.Username == username);
            if (cbos == null != cbos.Count() <= 0)
            {
                throw new Exception("Current Account is not a CBO.");
            }
            return cbos;
        }

        public static IQueryable<Unit> GetCBOsInProject(this IQueryable<Unit> data, string projectUsername)
        {
            var cbos = data.BaseFilter().Where(x => x.Project.Username == projectUsername);
            if (cbos == null != cbos.Count() <= 0)
            {
                throw new Exception("Current Account is not a Project Manager or this project does not have any CBOs.");
            }
            return cbos;
        }

        public static IQueryable<string> GetCBOCodesInProject(this IQueryable<Unit> data, string projectUsername)
        {
            return data.GetCBOsInProject(projectUsername).Select(x => x.Code);
        }

        public static IQueryable<Unit> GetCBOsInProject(this IQueryable<Unit> data, Guid projectId)
        {
            var cbos = data.BaseFilter().Where(x => x.ProjectId == projectId);
            return cbos;
        }

        public static IEnumerable<ReportAggregateModel> FindConflictReport(this IEnumerable<ReportAggregateModel> reports, string cboCode)
        {
            return reports.Where(_ => _.CBOCode != cboCode);
        }

        public static IEnumerable<ReportAggregateModel> FindConflictReport(this IEnumerable<ReportAggregateModel> reports, IEnumerable<string> cboCodes)
        {
            return reports.Where(_ => !cboCodes.Contains(_.CBOCode));
        }

        public static Project FindByUsername(this IQueryable<Project> data, string username)
        {
            var result = data.FirstOrDefault(_ => _.Username == username);
            if (result == null)
                throw new Exception("Current Account is not a Project Manager");
            return result;
        }

        public static Project FindById(this IQueryable<Project> data, Guid id)
        {
            var result = data.FirstOrDefault(_ => _.Id == id);
            if (result == null)
                throw new Exception("Current Account is not a Project Manager");
            return result;
        }

        public static List<Unit> GetCBOsByRole(this AppDbContext _dbContext, CustomUser user)
        {
            switch (user.Role)
            {
                case Role.SMD_CBO:
                    return _dbContext.Units.GetCBOsByUsernameAsync(user.Username).AsNoTracking().ToList();
                case Role.SMD_PROJECT:
                    return _dbContext.Units.GetCBOsInProject(user.UnitId).AsNoTracking().ToList();
                case Role.SMD_ADMIN:
                    throw new Exception(ErrorMessages.ROLE_NOT_SUITABLE);
                default:
                    throw new Exception(ErrorMessages.ROLE_NOT_SUITABLE);
            }
        }
    }
}
