using Data.Constants;
using Data.DbContexts;
using Data.Entities.SMDEntities;
using Data.Models;
using Mapster;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Services.Extenstions;
using Services.LookupServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.SMDServices
{
    public interface IIndicatorService
    {
        Task<PagingModel2> GetIndicatorsAsync(string searchValue, int pageIndex = 0, int pageSize = int.MaxValue);
        Task<ResultModel> CreateIndicatorAsync(IndicatorCreateModel model);
        Task<ResultModel> UpdateIndicatorAsync(IndicatorUpdateModel model);
        Task<ResultModel> DeleteIndicatorAsync(Guid id);
        Task<PagingModel2> GetKPIsAsync(Guid indicatorId, int pageIndex = 0, int pageSize = int.MaxValue);
        Task<ResultModel> CreateKPIAsync(KPICreateModel model);
        Task<ResultModel> UpdateKPIAsync(KPIUpdateModel model);
        Task<ResultModel> DeleteKPIAsync(Guid id);
    }

    public class IndicatorService : IIndicatorService
    {
        private readonly AppDbContext _dbContext;
        private readonly IndicatorLookupService _indicatorLookup;

        public IndicatorService(AppDbContext dbContext, IndicatorLookupService indicatorLookup)
        {
            _dbContext = dbContext;
            _indicatorLookup = indicatorLookup;
        }

        public async Task<ResultModel> CreateIndicatorAsync(IndicatorCreateModel model)
        {
            var result = new ResultModel();
            try
            {
                var entity = model.Adapt<Indicator>();
                _dbContext.Indicators.Add(entity);
                await _dbContext.SaveChangesAsync();
                result.Succeed = true;
                result.Data = entity.Adapt<IndicatorViewModel>();
                _indicatorLookup.Refresh();
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

        public async Task<ResultModel> CreateKPIAsync(KPICreateModel model)
        {
            var result = new ResultModel();
            try
            {
                var entity = model.Adapt<KPI>();
                _dbContext.KPIs.Add(entity);
                await _dbContext.SaveChangesAsync();
                result.Succeed = true;
                result.Data = entity.Adapt<KPIViewModel>();
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> DeleteIndicatorAsync(Guid id)
        {
            var result = new ResultModel();
            try
            {
                var entity = _dbContext.Indicators.Find(id);
                if (entity == null)
                    throw new Exception(ErrorMessages.ID_NOT_FOUND);
                if (entity.BlockChanges)
                    throw new Exception(ErrorMessages.CHANGES_NOT_ALLOWED);
                entity.IsDeleted = true;
                _dbContext.Indicators.Update(entity);
                await _dbContext.SaveChangesAsync();
                result.Succeed = true;
                _indicatorLookup.Refresh();
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> DeleteKPIAsync(Guid id)
        {
            var result = new ResultModel();
            try
            {
                var entity = _dbContext.KPIs.Find(id);
                if (entity == null)
                    throw new Exception(ErrorMessages.ID_NOT_FOUND);
                entity.IsDeleted = true;
                _dbContext.KPIs.Update(entity);
                await _dbContext.SaveChangesAsync();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<PagingModel2> GetIndicatorsAsync(string searchValue, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var result = new PagingModel2();
            try
            {
                var filter = _dbContext.Indicators.BaseFilter().Where(_ => searchValue == null || _.Name.Contains(searchValue));

                result.PageCount = filter.PageCount(pageSize);
                result.Data = await filter.PageData(pageIndex, pageSize)
                                            .BuildAdapter()
                                            .AdaptToTypeAsync<IEnumerable<IndicatorViewModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<PagingModel2> GetKPIsAsync(Guid indicatorId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var result = new PagingModel2();
            try
            {
                var filter = _dbContext.KPIs.BaseFilter().FilterKPI(indicatorId);

                result.PageCount = filter.PageCount(pageSize);
                result.Data = await filter.PageData(pageIndex, pageSize)
                                            .BuildAdapter()
                                            .AdaptToTypeAsync<IEnumerable<KPIViewModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public async Task<ResultModel> UpdateIndicatorAsync(IndicatorUpdateModel model)
        {
            var result = new ResultModel();
            try
            {
                var entity = _dbContext.Indicators.Find(model.Id);
                if (entity == null)
                    throw new Exception(ErrorMessages.ID_NOT_FOUND);
                if (entity.BlockChanges)
                    throw new Exception(ErrorMessages.CHANGES_NOT_ALLOWED);
                model.Adapt(entity);
                _dbContext.Indicators.Update(entity);
                await _dbContext.SaveChangesAsync();
                result.Succeed = true;
                result.Data = entity.Adapt<IndicatorViewModel>();
                _indicatorLookup.Refresh();
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

        public async Task<ResultModel> UpdateKPIAsync(KPIUpdateModel model)
        {
            var result = new ResultModel();
            try
            {
                var entity = _dbContext.KPIs.Find(model.Id);
                model.Adapt(entity);
                _dbContext.KPIs.Update(entity);
                await _dbContext.SaveChangesAsync();
                result.Succeed = true;
                result.Data = entity.Adapt<KPIViewModel>();
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }
    }
}
