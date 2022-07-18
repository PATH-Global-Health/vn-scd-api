using Data.Constants;
using Data.DbContexts;
using Data.Entities.SMDEntities;
using Data.Models;
using Data.Models.SMDModels;
using Mapster;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Services.Extenstions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.SMDServices
{
    public interface IPackageService
    {
        Task<ResultModel> GetAsync(int pageIndex = 0, int pageSize = int.MaxValue);
        Task<ResultModel> CreateAsync(PackageCreateModel model);
        Task<ResultModel> UpdateAsync(PackageUpdateModel model);
        Task<ResultModel> DeleteAsync(Guid id);
        Task<ResultModel> GetIPackageAsync(Guid? packageId, string province, int pageIndex = 0, int pageSize = int.MaxValue);
        Task<ResultModel> CreateIPackageAsync(ImplementPackageCreateModel model);
        Task<ResultModel> UpdateIPackageAsync(ImplementPackageUpdateModel model);
        Task<ResultModel> GetContractAsync(Guid? cboId, Guid? ipackageId, int pageIndex = 0, int pageSize = int.MaxValue);
        Task<ResultModel> CreateContractAsync(ContractCreateModel model);
        Task<ResultModel> UpdateContractAsync(ContractUpdateModel model);
        Task<ResultModel> SetCurrentContractAsync(SetContractModel model);
        Task<ResultModel> DeleteContractAsync(Guid id);
        Task<ResultModel> GetTargetAsync(Guid? ipackageId, int pageIndex = 0, int pageSize = int.MaxValue);
        Task<ResultModel> CreateTargetAsync(TargetCreateModel model);
        Task<ResultModel> UpdateTargetAsync(TargetUpdateModel model);
    }

    public class PackageService : IPackageService
    {
        private readonly AppDbContext _dbContext;

        public PackageService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResultModel> CreateAsync(PackageCreateModel model)
        {
            var result = new ResultModel();
            try
            {
                var entity = model.Adapt<Package>();
                _dbContext.Packages.Add(entity);
                await _dbContext.SaveChangesAsync();
                result.Succeed = true;
                result.Data = entity.Adapt<PackageViewModel>();
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

        public async Task<ResultModel> CreateContractAsync(ContractCreateModel model)
        {
            var result = new ResultModel();
            try
            {
                var entity = model.Adapt<Contract>();
                var overlap = _dbContext.Contracts.OverlapContract(entity).ToList();
                if (overlap.Count > 0)
                {
                    throw new Exception("Time overlap with existed contract.");
                }
                //var cboTask = _dbContext.Units.FirstOrDefaultAsync(_ => _.Id == model.CBOId);
                //var ipackageTask = _dbContext.ImplementPackages.FirstOrDefaultAsync(_ => _.Id == model.IPackageId);
                //var cbo = await cboTask;
                //var ipackage = await ipackageTask;
                //if (cbo != null)
                //    throw new Exception(ErrorMessages.ID_NOT_FOUND);
                //if (ipackage != null)
                //    throw new Exception(ErrorMessages.ID_NOT_FOUND);
                //if (cbo.Province != ipackage.Province)
                //    throw new Exception(ErrorMessages.PROVINCE_CONFLICTED);
                _dbContext.Contracts.Add(entity);
                await _dbContext.SaveChangesAsync();
                result.Succeed = true;
                result.Data = entity.Adapt<ContractViewModel>();
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

        public async Task<ResultModel> CreateIPackageAsync(ImplementPackageCreateModel model)
        {
            var result = new ResultModel();
            try
            {
                var entity = model.Adapt<ImplementPackage>();
                _dbContext.ImplementPackages.Add(entity);
                await _dbContext.SaveChangesAsync();
                result.Succeed = true;
                result.Data = entity.Adapt<ImplementPackageViewModel>();
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

        public async Task<ResultModel> CreateTargetAsync(TargetCreateModel model)
        {
            var result = new ResultModel();
            try
            {
                var entity = model.Adapt<Target>();
                _dbContext.Targets.Add(entity);
                await _dbContext.SaveChangesAsync();
                result.Succeed = true;
                result.Data = entity.Adapt<TargetViewModel>();
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
                var entity = _dbContext.Packages.Find(id);
                if (entity == null)
                    throw new Exception(ErrorMessages.ID_NOT_FOUND);
                if (entity.BlockChanges)
                    throw new Exception(ErrorMessages.CHANGES_NOT_ALLOWED);
                entity.IsDeleted = true;
                _dbContext.Packages.Update(entity);
                await _dbContext.SaveChangesAsync();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> DeleteContractAsync(Guid id)
        {
            var result = new ResultModel();
            try
            {
                var entity = _dbContext.Contracts.Find(id);
                if (entity == null)
                    throw new Exception(ErrorMessages.ID_NOT_FOUND);
                entity.IsDeleted = true;
                _dbContext.Contracts.Update(entity);
                await _dbContext.SaveChangesAsync();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> GetAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var result = new PagingModel2();
            try
            {
                var filter = _dbContext.Packages.BaseFilter();

                result.PageCount = filter.PageCount(pageSize);
                result.Data = await filter.PageData(pageIndex, pageSize)
                                            .BuildAdapter()
                                            .AdaptToTypeAsync<IEnumerable<PackageViewModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> GetContractAsync(Guid? cboId, Guid? ipackageId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var result = new PagingModel2();
            try
            {
                var filter = _dbContext.Contracts.BaseFilter().FilterContract(cboId, ipackageId);
                filter = filter.Include(_ => _.ImplementPackage)
                                .ThenInclude(_ => _.Package)
                                .Include(_ => _.ImplementPackage)
                                .ThenInclude(_ => _.Targets);
                result.PageCount = filter.PageCount(pageSize);
                result.Data = await filter.PageData(pageIndex, pageSize)
                                            .BuildAdapter()
                                            .AdaptToTypeAsync<IEnumerable<ContractViewModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> GetIPackageAsync(Guid? packageId, string province, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var result = new PagingModel2();
            try
            {
                var filter = _dbContext.ImplementPackages.BaseFilter().FilterIPackage(packageId, province);
                filter = filter.Include(_ => _.Package)
                                .Include(_ => _.Targets)
                                .ThenInclude(_ => _.Indicator);
                result.PageCount = filter.PageCount(pageSize);
                result.Data = await filter.PageData(pageIndex, pageSize)
                                            .BuildAdapter()
                                            .AdaptToTypeAsync<IEnumerable<ImplementPackageViewModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> GetTargetAsync(Guid? ipackageId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var result = new PagingModel2();
            try
            {
                var filter = _dbContext.Targets.BaseFilter().FilterTarget(ipackageId);

                result.PageCount = filter.PageCount(pageSize);
                result.Data = await filter.PageData(pageIndex, pageSize)
                                            .BuildAdapter()
                                            .AdaptToTypeAsync<IEnumerable<TargetViewModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> SetCurrentContractAsync(SetContractModel model)
        {
            var result = new ResultModel();
            try
            {
                var contract = await _dbContext.Contracts.BaseFilter()
                                                            .FirstOrDefaultAsync(_ => _.Id == model.ContractId);
                if (contract == null)
                {
                    throw new Exception(ErrorMessages.ID_NOT_FOUND);
                }
                var now = DateTime.Now;
                contract.Start = now;
                contract.IsCurrent = true;
                var oldContracts = await _dbContext.Contracts.BaseFilter().FilterContract(contract.CBOId, null).ToListAsync();
                if (oldContracts != null)
                {
                    foreach (var oldContract in oldContracts)
                    {
                        if (oldContract.Id != contract.Id)
                        {
                            oldContract.IsCurrent = false;
                        }
                    }
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

        public async Task<ResultModel> UpdateAsync(PackageUpdateModel model)
        {
            var result = new ResultModel();
            try
            {
                var entity = _dbContext.Packages.Find(model.Id);
                if (entity == null)
                    throw new Exception(ErrorMessages.ID_NOT_FOUND);
                if (entity.BlockChanges)
                    throw new Exception(ErrorMessages.CHANGES_NOT_ALLOWED);
                model.Adapt(entity);
                _dbContext.Packages.Update(entity);
                await _dbContext.SaveChangesAsync();
                result.Succeed = true;
                result.Data = entity.Adapt<PackageViewModel>();
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

        public async Task<ResultModel> UpdateContractAsync(ContractUpdateModel model)
        {
            var result = new ResultModel();
            try
            {
                var entity = _dbContext.Contracts.Find(model.Id);
                model.Adapt(entity);
                _dbContext.Contracts.Update(entity);
                await _dbContext.SaveChangesAsync();
                result.Succeed = true;
                result.Data = entity.Adapt<PackageViewModel>();
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

        public async Task<ResultModel> UpdateIPackageAsync(ImplementPackageUpdateModel model)
        {
            var result = new ResultModel();
            try
            {
                var entity = _dbContext.ImplementPackages.Find(model.Id);
                if (entity == null)
                    throw new Exception(ErrorMessages.ID_NOT_FOUND);
                if (entity.Contracts != null && entity.Contracts.Count > 0)
                    throw new Exception(ErrorMessages.IPACKAGE_HAS_CONTRACTS);
                model.Adapt(entity);
                _dbContext.ImplementPackages.Update(entity);
                await _dbContext.SaveChangesAsync();
                result.Succeed = true;
                result.Data = entity.Adapt<ImplementPackageViewModel>();
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

        public async Task<ResultModel> UpdateTargetAsync(TargetUpdateModel model)
        {
            var result = new ResultModel();
            try
            {
                var entity = _dbContext.Targets.Find(model.Id);
                if (entity == null)
                    throw new Exception(ErrorMessages.ID_NOT_FOUND);
                if (entity.ImplementPackage.Contracts != null && entity.ImplementPackage.Contracts.Count > 0)
                    throw new Exception(ErrorMessages.IPACKAGE_HAS_CONTRACTS);
                model.Adapt(entity);
                _dbContext.Targets.Update(entity);
                await _dbContext.SaveChangesAsync();
                result.Succeed = true;
                result.Data = entity.Adapt<TargetViewModel>();
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
