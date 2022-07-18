using System;
using System.Collections.Concurrent;
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
    public interface IProfileLinkService
    {
        ResultModel GetAll(string username);
        ResultModel RegisterCustomerByQR(Guid employeeId, string employeeName, Guid profileId);
    }
    public class ProfileLinkService:IProfileLinkService
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;

        public ProfileLinkService(IMapper mapper, AppDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        #region MyRegion
        public ResultModel GetAll(string username)
        {
            ResultModel result = new ResultModel();

            try
            {
                var profileLinks = _dbContext.ProfileLinks.Where(x => x.IsDeleted == false).ToList();
                result.Data = _mapper.Map<List<ProfileLinks>,List<ProfileLinkViewModel>>(profileLinks);
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }
        #endregion

        public ResultModel RegisterCustomerByQR(Guid employeeId,string employeeName,Guid profileId)
        {
            ResultModel result = new ResultModel();

            try
            {
                var empId = _dbContext.Doctors.FirstOrDefault(x => x.UserId == employeeId.ToString()).Id;
                var unitId = _dbContext.UnitDoctors.FirstOrDefault(x => x.DoctorId == empId && x.IsDeleted == false).Id;
                var profileLinkses = new ConcurrentQueue<ProfileLinks>();
                profileLinkses.Enqueue(new ProfileLinks()
                {
                    LinkTo = employeeId,
                    Type = TypeFacitily.EMPLOYEE,
                    ProfileId = profileId
                });
                profileLinkses.Enqueue(new ProfileLinks()
                {
                    LinkTo = unitId,
                    Type = TypeFacitily.FACILITY,
                    ProfileId = profileId
                });

                _dbContext.AddRange(profileLinkses);
                _dbContext.SaveChanges();
                result.Data = profileId;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }


    }
}
