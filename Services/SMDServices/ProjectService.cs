using Data.Constants;
using Data.DbContexts;
using Data.Entities;
using Data.Entities.SMDEntities;
using Data.Models;
using Data.Models.SMDModels;
using Mapster;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Services.Extenstions;
using Services.LookupServices;
using Services.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Services.SMDServices
{
    public interface IProjectService
    {
        Task<ResultModel> GetByUsernameAsync(CustomUser user);
        Task<ResultModel> GetAsync(string searchValue, int pageIndex = 0, int pageSize = int.MaxValue);
        Task<ResultModel> CreateAsync(ProjectCreateModel model);
        Task<ResultModel> UpdateAsync(ProjectUpdateModel model);
        Task<ResultModel> DeleteAsync(Guid id);
        Task<ResultModel> GetUnitAsync(string searchValue, int pageIndex = 0, int pageSize = int.MaxValue);
        Task<ResultModel> GetUnitByUsernameAsync(CustomUser user);
        Task<ResultModel> GetUnitsInProjectAsync(string searchValue, Guid projectId, int pageIndex = 0, int pageSize = int.MaxValue);
        Task<ResultModel> GetUnitsInProjectByUsernameAsync(string searchValue, CustomUser user, int pageIndex = 0, int pageSize = int.MaxValue);
        Task<ResultModel> CreateUnitByProjectAsync(SMDUnitCreateModel model);
        Task<ResultModel> UpdateUnitInProjectAsync(SMDUnitUpdateModel model);
        Task<ResultModel> DeleteUnitInProjectAsync(Guid id);
        Task<ResultModel> AddAccountToProject(AddAccountModel model, CustomUser user);
    }

    public class ProjectService : SMDBaseService, IProjectService
    {
        private readonly AppDbContext _dbContext;
        private readonly IProducer _producer;

        public ProjectService(SMDUserLookupService userLookupService, AppDbContext dbContext, IProducer producer) : base(userLookupService)
        {
            _dbContext = dbContext;
            _producer = producer;
        }

        public async Task<ResultModel> AddAccountToProject(AddAccountModel model, CustomUser user)
        {
            var result = new ResultModel();
            using (var transaction = _dbContext.Database.BeginTransaction())
                try
                {
                    if (user == null || user.Role != Role.SMD_ADMIN)
                        throw new Exception(ErrorMessages.ROLE_NOT_SUITABLE);
                    var newUser = model.Adapt<SMDUser>();
                    _UserLookupService.Add(newUser, _dbContext);
                    var account = model.Adapt<CBOCreateModel>();
                    account.GroupName = Role.SMD_PROJECT.Adapt<string>();
                    var createAccountResult = _producer.CreateAccount(account);
                    if (!createAccountResult.Succeed)
                    {
                        throw new Exception("Create Account Fail: " + createAccountResult.ErrorMessage);
                    }

                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();
                    result.Succeed = true;
                }
                catch (DbUpdateException dbue)
                {
                    if (dbue.InnerException is SqlException)
                        (dbue.InnerException as SqlException).Adapt(result);
                    else
                        dbue.Adapt(result);
                }
                catch (Exception e)
                {
                    e.Adapt(result);
                }
            return result;
        }

        public async Task<ResultModel> CreateAsync(ProjectCreateModel model)
        {
            var result = new ResultModel();
            using (var transaction = _dbContext.Database.BeginTransaction())
                try
                {
                    // create project
                    var entity = model.Adapt<Project>();
                    _dbContext.Projects.Add(entity);
                    // create user for project
                    var user = model.Adapt<SMDUser>();
                    _UserLookupService.Add(user, _dbContext);
                    await _dbContext.SaveChangesAsync();
                    // create account with user module
                    var account = model.Account.Adapt<CBOCreateModel>();
                    account.GroupName = Role.SMD_PROJECT.Adapt<string>();
                    var createAccountResult = _producer.CreateAccount(account);
                    if (!createAccountResult.Succeed)
                    {
                        throw new Exception("Create Account Fail: " + createAccountResult.ErrorMessage);
                    }
                    transaction.Commit();
                    result.Succeed = true;
                    result.Data = entity.Adapt<ProjectViewModel>();
                }
                catch (DbUpdateException dbue)
                {
                    if (dbue.InnerException is SqlException)
                        (dbue.InnerException as SqlException).Adapt(result);
                    else
                        dbue.Adapt(result);
                }
                catch (Exception e)
                {
                    e.Adapt(result);
                }
            return result;
        }

        public async Task<ResultModel> CreateUnitByProjectAsync(SMDUnitCreateModel model)
        {
            var result = new ResultModel();
            using (var transaction = _dbContext.Database.BeginTransaction())
                try
                {
                    var project = _dbContext.Projects.BaseFilter().FirstOrDefault(_ => _.Id == model.ProjectId);
                    if (project == null)
                        throw new Exception(ErrorMessages.ID_NOT_FOUND);
                    // create cbo
                    var entity = model.Adapt<Unit>();
                    entity.AllowInputType = project.AllowInputType;
                    _dbContext.Units.Add(entity);
                    await _dbContext.SaveChangesAsync();
                    // create account for cbo
                    var account = model.Account.Adapt<CBOCreateModel>();
                    account.GroupName = Role.SMD_CBO.Adapt<string>();
                    var createAccountResult = _producer.CreateAccount(account);
                    if (!createAccountResult.Succeed)
                    {
                        throw new Exception("Create Account Fail: " + createAccountResult.ErrorMessage);
                    }
                    //
                    transaction.Commit();
                    result.Succeed = true;
                    result.Data = entity.Adapt<SMDUnitViewModel>();
                }
                catch (DbUpdateException dbue)
                {
                    if (dbue.InnerException is SqlException)
                        (dbue.InnerException as SqlException).Adapt(result);
                    else
                        dbue.Adapt(result);
                }
                catch (Exception e)
                {
                    e.Adapt(result);
                }
            return result;
        }

        public async Task<ResultModel> DeleteAsync(Guid id)
        {
            var result = new ResultModel();
            try
            {
                // delete project
                var entity = _dbContext.Projects.Find(id);
                entity.IsDeleted = true;
                _dbContext.Projects.Update(entity);
                // delete cbos in project
                var cbos = _dbContext.Units.GetCBOsInProject(id);
                if (cbos != null)
                {
                    foreach (var cbo in cbos)
                    {
                        cbo.IsDeleted = true;
                    }
                    _dbContext.Units.UpdateRange(cbos);
                }
                await _dbContext.SaveChangesAsync();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> DeleteUnitInProjectAsync(Guid id)
        {
            var result = new ResultModel();
            try
            {
                var entity = _dbContext.Units.Find(id);
                entity.IsDeleted = true;
                _dbContext.Units.Update(entity);
                await _dbContext.SaveChangesAsync();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> GetAsync(string searchValue, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var result = new PagingModel2();
            try
            {
                var filter = _dbContext.Projects.BaseFilter().Where(_ => searchValue == null || _.Name.Contains(searchValue));

                result.PageCount = filter.PageCount(pageSize);
                result.Data = await filter.PageData(pageIndex, pageSize)
                                            .BuildAdapter()
                                            .AdaptToTypeAsync<IEnumerable<ProjectViewModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> GetByUsernameAsync(CustomUser user)
        {
            var result = new ResultModel();
            try
            {
                var filter = _dbContext.Projects.BaseFilter().FirstOrDefault(_ => _.Id == user.UnitId);

                result.Data = await filter.BuildAdapter().AdaptToTypeAsync<ProjectViewModel>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> GetUnitAsync(string searchValue, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var result = new PagingModel2();
            try
            {
                var filter = _dbContext.Units.BaseFilter().Where(_ => _.ProjectId.HasValue).Where(_ => searchValue == null || _.Name.Contains(searchValue));

                result.PageCount = filter.PageCount(pageSize);
                result.Data = await filter.PageData(pageIndex, pageSize)
                                            .BuildAdapter()
                                            .AdaptToTypeAsync<IEnumerable<SMDUnitViewModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> GetUnitByUsernameAsync(CustomUser user)
        {
            var result = new ResultModel();
            try
            {
                var filter = _dbContext.Units.BaseFilter().Where(_ => _.Id == user.UnitId);
                result.Data = await filter.FirstOrDefault()
                                            .BuildAdapter()
                                            .AdaptToTypeAsync<SMDUnitViewModel>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> GetUnitsInProjectAsync(string searchValue, Guid projectId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var result = new PagingModel2();
            try
            {
                var filter = _dbContext.Units.BaseFilter().FilterUnit(projectId, searchValue);
                result.PageCount = filter.PageCount(pageSize);
                result.Data = await filter.PageData(pageIndex, pageSize)
                                            .BuildAdapter()
                                            .AdaptToTypeAsync<IEnumerable<SMDUnitViewModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> GetUnitsInProjectByUsernameAsync(string searchValue, CustomUser user, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var result = new PagingModel2();
            try
            {
                var filter = _dbContext.Units.BaseFilter().FilterUnit(user.UnitId, searchValue);
                result.PageCount = filter.PageCount(pageSize);
                result.Data = await filter.PageData(pageIndex, pageSize)
                                            .BuildAdapter()
                                            .AdaptToTypeAsync<IEnumerable<SMDUnitViewModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> UpdateAsync(ProjectUpdateModel model)
        {
            var result = new ResultModel();
            try
            {
                var entity = _dbContext.Projects.Find(model.Id);
                if (entity == null)
                    throw new Exception(ErrorMessages.ID_NOT_FOUND);
                if (entity.AllowInputType != model.AllowInputType)
                {
                    var cbos = _dbContext.Units.GetCBOsInProject(model.Id);
                    if (cbos != null)
                    {
                        foreach (var cbo in cbos)
                        {
                            cbo.AllowInputType = model.AllowInputType;
                        }
                    }
                    _dbContext.Units.UpdateRange(cbos);
                }
                model.Adapt(entity);
                _dbContext.Projects.Update(entity);
                await _dbContext.SaveChangesAsync();
                result.Succeed = true;
                result.Data = entity.Adapt<ProjectViewModel>();
            }
            catch (DbUpdateException dbue)
            {
                if (dbue.InnerException is SqlException)
                    (dbue.InnerException as SqlException).Adapt(result);
                else
                    dbue.Adapt(result);
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> UpdateUnitInProjectAsync(SMDUnitUpdateModel model)
        {
            var result = new ResultModel();
            try
            {
                var entity = _dbContext.Units.Find(model.Id);
                model.Adapt(entity);
                _dbContext.Units.Update(entity);
                await _dbContext.SaveChangesAsync();
                result.Succeed = true;
                result.Data = entity.Adapt<SMDUnitViewModel>();
            }
            catch (DbUpdateException dbue)
            {
                if (dbue.InnerException is SqlException)
                    (dbue.InnerException as SqlException).Adapt(result);
                else
                    dbue.Adapt(result);
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }
    }
}
