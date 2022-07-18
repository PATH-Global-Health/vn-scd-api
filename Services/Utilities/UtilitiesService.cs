using Data.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.Utilities
{
    public interface IUtilitiesService
    {
        void FixDeletedProject();
        bool FakeData();
    }

    public class UtilitiesService : IUtilitiesService
    {
        private readonly AppDbContext _dbContext;

        public UtilitiesService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool FakeData()
        {
            var projects = _dbContext.Projects.Where(p => p.IsDeleted == false);
            var i = 0;
            foreach (var project in projects)
            {
                project.Name = "Project " + (++i);
                project.Code = "PRJ" + (i);
                _dbContext.Projects.Update(project);
            };
            var cbo = _dbContext.Units.Where(p => p.IsDeleted == false && p.ProjectId != null);
            i = 0;
            foreach(var c in cbo)
            {
                c.Name = "CBO " + (++i);
                c.Code = "C" + (i);
                _dbContext.Units.Update(c);
            }
            _dbContext.SaveChanges();
            return true;
        }

        public void FixDeletedProject()
        {
            var deletedProjects = _dbContext.Projects.Where(_ => _.IsDeleted);
            if (deletedProjects.Any())
            {
                var projectIds = deletedProjects.Select(_ => _.Id).ToList();
                var unDeletedCbos = _dbContext.Units.Where(_ => !_.IsDeleted && _.ProjectId.HasValue && projectIds.Contains(_.ProjectId.Value));
                if (unDeletedCbos.Any())
                {
                    foreach (var cbo in unDeletedCbos)
                    {
                        cbo.IsDeleted = true;
                    }
                    _dbContext.Units.UpdateRange(unDeletedCbos);
                }
                _dbContext.SaveChanges();
            }
        }
    }
}
