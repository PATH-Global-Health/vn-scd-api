using Data.Constants;
using Data.Entities;
using Data.Entities.SMDEntities;
using Data.Models;
using Data.Models.CustomModels;
using Data.Models.SMDModels;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Services.Extenstions;
using Services.LookupServices;
using Services.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleManagement.Extensions
{
    public static class MapsterExtensions
    {
        public static void UseMapsterConfig(this IApplicationBuilder app, IndicatorLookupService _indicatorLookup, PackageLookupService _packageLookup)
        {
            TypeAdapterConfig.GlobalSettings.AllowImplicitDestinationInheritance = true;
            TypeAdapterConfig.GlobalSettings.Default.IgnoreNullValues(true);
            #region Exception
            TypeAdapterConfig.GlobalSettings.ForType<Exception, ResultModel>()
                                    .Map(dest => dest.Succeed, src => false)
                                    .Map(dest => dest.ErrorMessage,
                                            src => src.InnerException != null ? src.InnerException.Message : src.Message
                                            //+ Environment.NewLine
                                            //+ src.StackTrace
                                            );
            TypeAdapterConfig.GlobalSettings.ForType<SqlException, ResultModel>()
                                    .Ignore(dest => dest.Data)
                                    .Map(dest => dest.Succeed, src => false)
                                    .Map(dest => dest.ErrorMessage,
                                            src => src.ReturnDuplicateMessage(), src => src.Number == 2601)
                                    .Map(dest => dest.ErrorMessage,
                                            src => src.InnerException != null ? src.InnerException.Message : src.Message
                                            //+ Environment.NewLine
                                            //+ src.StackTrace
                                            );
            #endregion
            #region KPI
            TypeAdapterConfig.GlobalSettings.ForType<KPICreateModel, KPI>()
                                    .AfterMapping(_ => _.FormatColor())
                                    .AfterMapping(_ => _.Validate());
            #endregion
            #region Account
            TypeAdapterConfig.GlobalSettings.ForType<AccountCreateModel, UserCreateModel>()
                                    .Map(dest => dest.UserName, src => src.Username);
            #endregion
            #region Project
            TypeAdapterConfig.GlobalSettings.ForType<ProjectCreateModel, Project>()
                                    .Ignore(dest => dest.Username)
                                    .Map(dest => dest.Phone, src => src.Account.Phone)
                                    .Map(dest => dest.Email, src => src.Account.Email);
            TypeAdapterConfig.GlobalSettings.ForType<Project, SMDUser>()
                                    .Map(dest => dest.UnitId, src => src.Id)
                                    ;
            TypeAdapterConfig.GlobalSettings.ForType<AddAccountModel, SMDUser>()
                                    .Map(dest => dest.UnitId, src => src.ProjectId)
                                    ;
            #endregion
            #region Unit
            TypeAdapterConfig.GlobalSettings.ForType<SMDUnitCreateModel, Unit>()
                                    .Map(dest => dest.Username, src => src.Account.Username);
            TypeAdapterConfig.GlobalSettings.ForType<Unit, SMDUser>()
                                    .Map(dest => dest.UnitId, src => src.Id)
                                    ;
            #endregion
            #region Report
            TypeAdapterConfig.GlobalSettings.ForType<ReportCreateModel, Report>()
                                    .Map(dest => dest.ValueType, src => _indicatorLookup.Lookup(src.IndicatorId).Type)
                                    .Map(dest => dest.PackageCode, src => _packageLookup.Lookup(src.PackageCode).Code, src => _indicatorLookup.Lookup(src.IndicatorId).Type == Data.Constants.ReportValueType.MONEY)
                                    .Map(dest => dest.DateTime, src => src.DateTime.ToReportDatetime())
                                    ;
            TypeAdapterConfig.GlobalSettings.ForType<ReportAggregateModel, Report>()
                                    .Map(dest => dest.DateTime, src => new DateTime(src.Year, src.Month, DateTime.DaysInMonth(src.Year, src.Month)))
                                    .Map(dest => dest.UnitId, src => GetUnitIdFromCBOs(src.CBOCode))
                                    .Map(dest => dest.CreatedMethod, src => Data.Constants.CreatedMethod.IMPORT)
                                    .Map(dest => dest.Province, src => src.PSNU)
                                    .Map(dest => dest.IndicatorId, src => _indicatorLookup.Lookup(src.IndicatorCode).Id)
                                    ;
            TypeAdapterConfig.GlobalSettings.ForType<ReportCreateModelV2, ICollection<Report>>()
                                    .Map(dest => dest, src => src.IndicatorValues.Adapt<ICollection<Report>>())
                                    .AfterMapping((src, dest) => MappingFunc(src, dest))
                                    ;
            TypeAdapterConfig.GlobalSettings.ForType<ReportCreateModelV2, Report>()
                                    .Map(dest => dest.DateTime, src => new DateTime(src.Year, src.Month, DateTime.DaysInMonth(src.Year, src.Month)))
                                    //.Map(dest => dest)
                                    ;
            TypeAdapterConfig.GlobalSettings.ForType<ReportPaymentModel, Report>()
                                    .Map(dest => dest.DateTime, src => src.ReportingPeriod)
                                    .Map(dest => dest.UnitId, src => GetUnitIdFromCBONames(src.CBOName))
                                    .Map(dest => dest.IndicatorId, src => _indicatorLookup.GetPaymentIndicator().Id)
                                    .Map(dest => dest.Period, src => Data.Constants.ReportPeriod.MONTH)
                                    .Map(dest => dest.CreatedMethod, src => Data.Constants.CreatedMethod.IMPORT)
                                    .Map(dest => dest.PackageCode, src => _packageLookup.Lookup(src.PackageCode).Code)
                                    .Map(dest => dest.ValueType, src => Data.Constants.ReportValueType.MONEY)
                                    .Map(dest => dest.Value, src => src.TotalAmount)
                                    .Map(dest => dest.Province, src => src.PSNU)
                                    ;
            TypeAdapterConfig.GlobalSettings.ForType<Report, ReportViewModel>()
                                    .Map(dest => dest.CBOName, src => src.Unit.Name)
                                    ;
            TypeAdapterConfig.GlobalSettings.ForType<Report, ReportExportExcelModel>()
                                    .Map(dest => dest.IndicatorName, src => src.Indicator.Name)
                                    .Map(dest => dest.Month, src => src.DateTime.Month)
                                    .Map(dest => dest.Year, src => src.DateTime.Year)
                                    .Map(dest => dest.Project, src => src.Unit.Project.Name)
                                    .Map(dest => dest.Province, src => src.Province.GetProvinceLabel())
                                    .Map(dest => dest.CBOName, src => src.Unit.Name)
                                    .AfterMapping(_ => _.DoAfterMapping())
                                    ;
            TypeAdapterConfig.GlobalSettings.ForType<Report, Report>()
                                    .Ignore(_ => _.Id)
                                    .Ignore(_ => _.DateCreated)
                                    ;
            TypeAdapterConfig.GlobalSettings.ForType<Report, ReportEfficiencyModel>()
                                    .Map(dest => dest.ProjectName, src => src.Unit.Project.Name)
                                    ;
            TypeAdapterConfig.GlobalSettings.ForType<Report, ReportHistory>()
                                    .Ignore(dest => dest.Id)
                                    ;
            TypeAdapterConfig.GlobalSettings.ForType<ReportIndividualModel, PatientInfo>()
                                    .Map(dest => dest.LayTestingCode, src => src.HIVTestingService.LayTestingCode)
                                    .Map(dest => dest.HTCTestCode, src => src.HIVTestingService.HTCTestCode)
                                    .Map(dest => dest.HTCSite, src => src.HIVTestingService.HTCSite)
                                    .Map(dest => dest.TestResult, src => src.HIVTestingService.TestResult)
                                    .Map(dest => dest.DateOfTesting, src => src.HIVTestingService.ReportDate)
                                    .Map(dest => dest.ClientID, src => src.ReferralService.ClientID)
                                    .Map(dest => dest.FacilityName, src => src.ReferralService.FacilityName)
                                    .Map(dest => dest.ServiceName, src => src.ReferralService.ServiceName)
                                    .Map(dest => dest.DateOfReferral, src => src.ReferralService.ReportDate)
                                    .Map(dest => dest.NewCase, src => src.VerificationResult.NewCase)
                                    .Map(dest => dest.DateOfVerification, src => src.VerificationResult.ReportDate)
                                    .Map(dest => dest.CreatedMethod, src => CreatedMethod.IMPORT)
                                    .Map(dest => dest.CBOCode, src => GetCBOCodeFromCBOName(src.CBOName))
                                    .Map(dest => dest.CBOId, src => GetUnitIdFromCBONames(src.CBOName))
                                    .AfterMapping(dest => dest.FormatResult())
                                    ;
            TypeAdapterConfig.GlobalSettings.ForType<PatientInfoCreateModel, PatientInfo>()
                                    .IgnoreIf((src, dest) => dest.CBOId != Guid.Empty, dest => dest.CBOId)
                                    .Map(dest => dest.CBOId, src => GetUnitIdFromCBOs(src.CBOCode))
                                    .AfterMapping(dest => dest.FormatResult())
                                    ;
            TypeAdapterConfig.GlobalSettings.ForType<PatientInfo, PatientInfo>()
                                    .Ignore(_ => _.Id)
                                    .Ignore(_ => _.DateCreated)
                                    .Ignore(_ => _.PatientInfoHistories)
                                    .AfterMapping(dest => dest.FormatResult())
                                    ;
            TypeAdapterConfig.GlobalSettings.ForType<PatientInfo, PatientInfoHistory>()
                                    .Ignore(_ => _.Id)
                                    ;
            #endregion
            #region Contract
            TypeAdapterConfig.GlobalSettings.ForType<Contract, ContractViewModel>()
                                    .Map(dest => dest.TotalAmount, src => src.ImplementPackage.TotalAmount, src => src.ImplementPackage != null)
                                    .Map(dest => dest.PackageName, src => src.ImplementPackage.Package.Name, src => src.ImplementPackage != null)
                                    .Map(dest => dest.Targets, src => src.ImplementPackage.Targets, src => src.ImplementPackage != null)
                                    ;
            #endregion
            #region ImplementPackage
            TypeAdapterConfig.GlobalSettings.ForType<ImplementPackage, ImplementPackageViewModel>()
                                    .Map(dest => dest.PackageCode, src => src.Package.Code)
                                    ;
            #endregion
            #region Utils
            //TypeAdapterConfig.GlobalSettings.ForType<DateTime, DateTime>()
            //                        .Map(dest => dest, src => new DateTime(src.Year, src.Month, DateTime.DaysInMonth(src.Year, src.Month)))
            //                        //.Map(dest => dest)
            //                        ;
            #endregion
        }

        private static void MappingFunc(ReportCreateModelV2 src, ICollection<Report> dest)
        {
            foreach (var item in dest)
            {
                src.Adapt(item);
            }
        }

        private static string GetCBOCodeFromCBOName(string str)
        {
            str = str.ToUpper();
            var units = GetParameterCBOs().ToList();
            if (units == null)
                throw new Exception($"No parameter CBOs found when mapping.");
            str = str.ToUpper();
            var unit = units.FirstOrDefault(_ => _.Name.ToUpper() == str);
            if (unit == null)
                throw new Exception($"No CBO with code '{str}' found.");
            return unit.Code;
        }

        private static string GetCBOCodeFromCBONameOrCode(string str)
        {
            str = str.ToUpper();
            var units = GetParameterCBOs().ToList();
            if (units == null)
                throw new Exception($"No parameter CBOs found when mapping.");
            var unit = units.FirstOrDefault(_ => _.Code.ToUpper() == str || _.Name.ToUpper() == str);
            if (unit == null)
                throw new Exception($"No CBO with code or name '{str}' found.");
            return unit.Code;
        }

        private static Guid GetUnitIdFromCBOs(string cboCode)
        {
            cboCode = cboCode.ToUpper();
            var units = GetParameterCBOs();
            if (units == null)
                throw new Exception($"No parameter CBOs found when mapping.");
            var unit = units.FirstOrDefault(_ => _.Code.ToUpper() == cboCode);
            if (unit == null)
                throw new Exception($"No CBO with code {cboCode} found.");
            return unit.Id;
        }

        private static Guid GetUnitIdFromCBONames(string cboName)
        {
            cboName = cboName.ToUpper();
            var units = GetParameterCBOs().ToList();
            if (units == null)
                throw new Exception($"No parameter CBOs found when mapping.");
            var unit = units.FirstOrDefault(_ => _.Name.ToUpper().Equals(cboName));
            if (unit == null)
                throw new Exception($"No CBO with name {cboName} found.");
            return unit.Id;
        }

        private static IEnumerable<Unit> GetParameterCBOs()
        {
            var value = Parameters("CBOs");
            if (value != null)
            {
                return (IEnumerable<Unit>)value;
            }
            return null;
        }
        private static string GetParameterUsername(string key)
        {
            var value = Parameters(key);
            if (value != null)
            {
                return (string)value;
            }
            return null;
        }
        private static object Parameters(string key)
        {
            if (MapContext.Current != null)
            {
                if (MapContext.Current.Parameters.ContainsKey(key))
                    return MapContext.Current.Parameters[key];
            }
            return null;
        }
    }
}
