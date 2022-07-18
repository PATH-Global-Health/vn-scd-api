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
    public interface IitemService
    {
        Task<ResultModel> AddItem(ItemCreateModel model);
        Task<ResultModel> GetItems(PagingParam<ItemSort> paginationModel, string name);
        Task<ResultModel> GetItemById(Guid id);
        Task<ResultModel> UpdateItem(Guid id, ItemUpdateModel model);
        Task<ResultModel> DeleteItem(Guid id);
    }

    public class ItemService: IitemService
    {

        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        public ItemService(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<ResultModel> AddItem(ItemCreateModel model)
        {
            ResultModel result = new ResultModel();
            try
            {
                var item = _mapper.Map<ItemCreateModel, Item>(model);
                _dbContext.Add(item);
                await _dbContext.SaveChangesAsync();

                result.Data = item.Id;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;
        }

        public async Task<ResultModel> GetItems(PagingParam<ItemSort> paginationModel, string name)
        {
            ResultModel result = new ResultModel();
            try
            {
                var itemQuery = _dbContext.Item.Where(x => !x.IsDeleted);
                if (!string.IsNullOrEmpty(name))
                {
                    itemQuery = itemQuery.Where(m => m.Name.ToUpper().Contains(name.ToUpper()));
                }

                var paging = new PagingModel(paginationModel.PageIndex, paginationModel.PageSize, itemQuery.Count());

                itemQuery = itemQuery.GetWithSorting(paginationModel.SortKey.ToString(), paginationModel.SortOrder);
                itemQuery = itemQuery.GetWithPaging(paginationModel.PageIndex, paginationModel.PageSize);

                var viewModels = await _mapper.ProjectTo<ItemViewModel>(itemQuery).ToListAsync();
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

        public async Task<ResultModel> GetItemById(Guid id)
        {
            ResultModel result = new ResultModel();
            try
            {
                var item = await _dbContext.Item.Where(r => r.Id == id && !r.IsDeleted).FirstOrDefaultAsync();

                if (item == null)
                {
                    result.ErrorMessage = ErrorMessages.ID_NOT_FOUND;
                    return result;
                }
                var model = _mapper.Map<Item, ItemViewModel>(item);
                result.Succeed = true;
                result.Data = model;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> UpdateItem(Guid id,ItemUpdateModel model)
        {
            ResultModel result = new ResultModel();
            var item = await _dbContext.Item.Where(r => r.Id == id && !r.IsDeleted).FirstOrDefaultAsync();
            if (item == null)
            {
                result.ErrorMessage = ErrorMessages.ID_NOT_FOUND;
                return result;
            }
            try
            {
                item = _mapper.Map(model, item);
                item.DateUpdated = DateTime.Now;
                await _dbContext.SaveChangesAsync();

                result.Succeed = true;
                result.Data = _mapper.Map<Item, ItemViewModel>(item);
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;

        }

        public async Task<ResultModel> DeleteItem(Guid id)
        {
            ResultModel result = new ResultModel();
            var item = await _dbContext.Item.Where(r => r.Id == id && !r.IsDeleted).FirstOrDefaultAsync();
            if (item == null)
            {
                result.ErrorMessage = ErrorMessages.ID_NOT_FOUND;
                return result;
            }
            try
            {
                item.IsDeleted = true;
                item.DateUpdated = DateTime.Now;
                await _dbContext.SaveChangesAsync();

                result.Succeed = true;
                result.Data = item.Id;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }

            return result;

        }


    }
}
