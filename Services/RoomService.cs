using AutoMapper;
using Data.DbContexts;
using Data.Entities;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services
{
    public interface IRoomService
    {
        ResultModel Add(RoomCreateModel model);
        ResultModel Update(RoomUpdateModel model);
        ResultModel Get(Guid? id, Guid? unitId, string username);
        ResultModel Delete(Guid id);
        ResultModel GetRoomByORGUnit(Guid unitId);
    }
    public class RoomService : IRoomService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public RoomService(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public ResultModel Add(RoomCreateModel model)
        {
            ResultModel result = new ResultModel();
            try
            {
                Room data = new Room
                {
                    IsDeleted = false,
                    Description = "",
                    UnitId = model.UnitId,
                    Code = model.Code,
                    Name = model.Name
                };

                if (ExistedCode(null, model.Code, model.UnitId))
                {
                    result.ErrorMessage = "Code existed";
                    return result;
                    
                }

                _dbContext.Add(data);
                _dbContext.SaveChanges();

                result.Succeed = true;
                result.Data = _mapper.Map<Room, RoomModel>(data);
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;

            }
            return result;
        }
        public ResultModel Update(RoomUpdateModel model)
        {
            ResultModel result = new ResultModel();
            try
            {
                var room = _dbContext.Rooms.FirstOrDefault(h => h.Id == model.Id);

                if (room != null)
                {
                    if (ExistedCode(model.Id, model.Code, model.UnitId))
                    {
                        result.ErrorMessage = "Code existed";
                        return result;
                    }

                    room.Code = model.Code;
                    room.Name = model.Name;
                    room.UnitId = model.UnitId;

                    room.DateUpdated = DateTime.Now;

                    _dbContext.Update(room);
                    _dbContext.SaveChanges();

                    result.Succeed = true;
                    result.Data = _mapper.Map<Room, RoomModel>(room);
                }
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
        public ResultModel Delete(Guid id)
        {
            ResultModel result = new ResultModel();
            try
            {
                var room = _dbContext.Rooms.FirstOrDefault(h => h.Id == id);
                if (room != null)
                {
                    room.IsDeleted = true;
                    room.DateUpdated = DateTime.Now;

                    _dbContext.Update(room);
                    _dbContext.SaveChanges();

                    result.Succeed = true;
                }
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
        public ResultModel Get(Guid? id, Guid? unitId, string username)
        {
            ResultModel result = new ResultModel();
            try
            {
                var units = _dbContext.Units.Where(_u => _u.Username == username).ToList();
                if (id == null)
                {
                    var rooms = _dbContext.Rooms.Include(_r => _r.Unit).Where(_r => unitId == null || _r.UnitId == unitId).Where(_r => units.Contains(_r.Unit)).ToList();
                    var data = _mapper.Map<List<Room>, List<RoomModel>>(rooms);
                    result.Data = data;
                }
                else
                {
                    var room = _dbContext.Rooms.Include(_r => _r.Unit).Where(h => unitId == null || h.UnitId == unitId)
                                                                      .Where(h => h.Id == id && h.IsDeleted == false)
                                                                      .Where(_r => units.Contains(_r.Unit))
                                                                      .ToList();
                    var data = _mapper.Map<List<Room>, List<RoomModel>>(room);
                    result.Data = data;
                }
                result.Succeed = true;

            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }

        public ResultModel GetRoomByORGUnit(Guid unitId)
        {
            ResultModel result = new ResultModel();
            try
            {
                var units = _dbContext.Units.FirstOrDefault(x => x.Id == unitId);
                if(units == null) throw new Exception("Unit does not exist");

                var rooms = _dbContext.Rooms.Include(_r => _r.Unit).Where(_r =>_r.UnitId == unitId).ToList();
                result.Data = _mapper.Map<List<Room>, List<RoomModel>>(rooms);
                result.Succeed = true;

            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }

        private bool ExistedCode(Guid? id, string code, Guid unitId)
        {
            try
            {
                if (id == null)
                {
                    return _dbContext.Rooms.Any(s => s.Code.ToUpper().Equals(code.ToUpper()) && s.UnitId == unitId);
                }
                else
                {
                    return _dbContext.Rooms.Any(s => s.Code.ToUpper().Equals(code.ToUpper()) && s.Id != id && s.UnitId == unitId);
                }
            }
            catch (Exception)
            {
                return true;
            }
        }
    }
}
