using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Data.Constants;
using Data.DbContexts;
using Data.Entities;
using Data.Models;

namespace Services
{

    public interface IReferTicketService
    {
        ResultModel Add(ReferTicketCreateModel model, string username);
        ResultModel RecviveTicket(Guid id, ReceiveTicketModel model);
        ResultModel GetReceivedTicket(string username, Guid? unitId = null, ReferType? status = null);
        ResultModel CreateTicket(TicketEmployeeModel model);
    }
    public class ReferTicketService: IReferTicketService
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;

        public ReferTicketService(IMapper mapper, AppDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        #region Add
        public ResultModel Add(ReferTicketCreateModel model,string username)
        {
            ResultModel result = new ResultModel();
            try
            {
                var ticket = _mapper.Map<ReferTicketCreateModel, ReferTickets>(model);
                var unit = _dbContext
                    .Units
                    .FirstOrDefault(x => x.IsDeleted == false && x.Username == username);
                if (unit == null)
                {
                    throw new Exception("Invalid facility");
                }
                var unitId = unit.Id;
                if (unitId == model.ToUnitId)
                {
                    throw new Exception("Cannot send same facility");
                }
                ticket.FromUnitId = unitId;
                ticket.Status = StatusTicket.SENT;
                _dbContext.Add(ticket);
                _dbContext.SaveChanges();
                result.Data = _mapper.Map<ReferTickets, ReferTicketViewModel>(ticket);
                result.Succeed = true;


            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
               
            }

            return result;
        }
        #endregion


        #region CreateTicket
        public ResultModel CreateTicket(TicketEmployeeModel model)
        {
            ResultModel result = new ResultModel();
            try
            {
                var ExistedTicket = _dbContext.ReferTickets.FirstOrDefault(x =>
                    x.ProfileId == model.ProfileId && x.ToUnitId == model.ToUnitId);
                if (ExistedTicket == null)
                {
                    var ticket = _mapper.Map<TicketEmployeeModel, ReferTickets>(model);
                    ticket.Status = StatusTicket.SENT;
                    _dbContext.Add(ticket);
                    _dbContext.SaveChanges();
                    result.Data = "Successful new creation";
                }
                else
                {
                    result.Data = "User Already has a ticket";
                }
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;

            }

            return result;
        }
        #endregion

        #region Get
        public ResultModel GetReceivedTicket(string username, Guid? unitId = null,ReferType? status=null)
        {
            ResultModel result = new ResultModel();
            try
            {
                var units = _dbContext.Units.Where(x => x.Username == username);
                if (unitId.HasValue)
                {
                    units = units.Where(x => x.Id == unitId.Value);
                }

                var listReferTicket = _dbContext.ReferTickets
                    .Where(x => x.IsDeleted == false && units.Any(y => y.Id == x.ToUnitId));

                if (status.HasValue)
                {
                    listReferTicket = listReferTicket.Where(x =>  x.Type == status);
                }
                result.Data = _mapper.Map<List<ReferTickets>, List<ReferTicketViewModel>>(listReferTicket.ToList());

                result.Succeed = true;

            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }

        #endregion

        #region Recieve

        public ResultModel RecviveTicket(Guid id, ReceiveTicketModel model)
        {
            ResultModel result = new ResultModel();
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                var ticketUpdate = _dbContext.ReferTickets.FirstOrDefault(x => x.Id == id);

                if (ticketUpdate == null)
                {
                    throw new Exception("Invalid Id");
                }


                if (model.Status != StatusTicket.CANCEL && ticketUpdate.ReferDate > model.ReceivedDate)
                {
                    throw new Exception("Invalid Received Date ");
                }



                ticketUpdate.ReceivedDate = model.ReceivedDate;
                ticketUpdate.Status = model.Status;
                _dbContext.Update(ticketUpdate);
                _dbContext.SaveChanges();

                if (model.Status == StatusTicket.RECEIVED)
                {
                    var profileUpdate = new ProfileLinks
                    {
                        LinkTo = ticketUpdate.ToUnitId.Value,
                        ProfileId = ticketUpdate.ProfileId.Value,
                        Type = TypeFacitily.FACILITY
                    };
                    _dbContext.Add(profileUpdate);
                    _dbContext.SaveChanges();
                }
                result.Data = id;
                result.Succeed = true;
                transaction.Commit();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                transaction.Rollback();
            }

            return result;
        }

        #endregion
    }
}
