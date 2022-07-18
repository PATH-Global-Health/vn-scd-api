using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Data.Common.PaginationModel;
using Data.Constants;
using Data.DbContexts;
using Data.Entities;
using Data.Models;
using Data.Utility.Paging;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public interface IitemSourceService
    {
        Task<ResultModel> AddItemSource(ItemSourceCreateModel model);
        Task<ResultModel> GetItemSource(PagingParam<ItemSort> paginationModel, string name);
        Task<ResultModel> GetItemSrcById(Guid id);
        Task<ResultModel> UpdateItemSource(Guid id, ItemSourceUpdateModel model);
        Task<ResultModel> DeleteItemSource(Guid id);
    }



    public class ItemSourceService: IitemSourceService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        public ItemSourceService(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<ResultModel> AddItemSource(ItemSourceCreateModel model)
        {
            ResultModel result = new ResultModel();
            try
            {
                var itemSrc = _mapper.Map<ItemSourceCreateModel, ItemSource>(model);
                _dbContext.Add(itemSrc);
                await _dbContext.SaveChangesAsync();

                result.Data = itemSrc.Id;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> GetItemSource(PagingParam<ItemSort> paginationModel, string name)
        {
            ResultModel result = new ResultModel();
            try
            {
                var itemSrcQuery = _dbContext.ItemSources.Where(x => !x.IsDeleted);
                if (!string.IsNullOrEmpty(name))
                {
                    itemSrcQuery = itemSrcQuery.Where(m => m.Name.ToUpper().Contains(name.ToUpper()));
                }

                var paging = new PagingModel(paginationModel.PageIndex, paginationModel.PageSize, itemSrcQuery.Count());

                itemSrcQuery = itemSrcQuery.GetWithSorting(paginationModel.SortKey.ToString(), paginationModel.SortOrder);
                itemSrcQuery = itemSrcQuery.GetWithPaging(paginationModel.PageIndex, paginationModel.PageSize);

                var viewModels = await _mapper.ProjectTo<ItemSourceViewModel>(itemSrcQuery).ToListAsync();
                paging.Data = viewModels;
                result.Succeed = true;
                result.Data = paging;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetItemSrcById(Guid id)
        {
            ResultModel result = new ResultModel();
            try
            {
                var itemSrc = await _dbContext.ItemSources.Where(r => r.Id == id && !r.IsDeleted).FirstOrDefaultAsync();

                if (itemSrc == null)
                {
                    result.ErrorMessage = ErrorMessages.ID_NOT_FOUND;
                    return result;
                }
                var model = _mapper.Map<ItemSource, ItemSourceViewModel>(itemSrc);
                result.Succeed = true;
                result.Data = model;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> UpdateItemSource(Guid id, ItemSourceUpdateModel model)
        {
            ResultModel result = new ResultModel();
            var itemSrc = await _dbContext.ItemSources.Where(r => r.Id == id && !r.IsDeleted).FirstOrDefaultAsync();
            if (itemSrc == null)
            {
                result.ErrorMessage = ErrorMessages.ID_NOT_FOUND;
                return result;
            }
            try
            {
                itemSrc = _mapper.Map(model, itemSrc);
                itemSrc.DateUpdated = DateTime.Now;
                await _dbContext.SaveChangesAsync();

                result.Succeed = true;
                result.Data = _mapper.Map<ItemSource, ItemSourceViewModel>(itemSrc);
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;

        }

        public async Task<ResultModel> DeleteItemSource(Guid id)
        {
            ResultModel result = new ResultModel();
            var itemSrc = await _dbContext.ItemSources.Where(r => r.Id == id && !r.IsDeleted).FirstOrDefaultAsync();
            if (itemSrc == null)
            {
                result.ErrorMessage = ErrorMessages.ID_NOT_FOUND;
                return result;
            }
            try
            {
                itemSrc.IsDeleted = true;
                itemSrc.DateUpdated = DateTime.Now;
                await _dbContext.SaveChangesAsync();

                result.Succeed = true;
                result.Data = itemSrc.Id;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;

        }
    }
}
