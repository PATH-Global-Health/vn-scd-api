using Data.Constants;
using Data.DbContexts;
using Data.Entities.SMDEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.LookupServices
{

    public class IndicatorLookupService : LookupServiceBase<Indicator>
    {
        public IndicatorLookupService(IServiceScopeFactory scopeFactory) : base(scopeFactory) { }

        public IEnumerable<Indicator> GetPercentageIndicators()
        {
            return this._Data.Where(_ => _.Type == ReportValueType.PERCENTAGE);
        }

        public IEnumerable<Indicator> GetIntegerIndicators()
        {
            return this._Data.Where(_ => _.Type == ReportValueType.INTEGER);
        }

        public Indicator GetPaymentIndicator()
        {
            var key = "PAYMENT";
            return base.Lookup(key);
        }

        public IEnumerable<Indicator> GetEfficiencyIndicators()
        {
            var key = "PAYMENT";
            yield return base.Lookup(key);
            key = "TX_NEW__1";
            yield return base.Lookup(key);
            key = "PREP_NEW__2";
            yield return base.Lookup(key);
        }

        //public override void Refresh()
        //{
        //    using (var scope = _ScopeFactory.CreateScope())
        //    {
        //        _Dictionary = new Dictionary<string, Indicator>();
        //        _GuidDictionary = new Dictionary<Guid, Indicator>();
        //        // get service instance
        //        AppDbContext dBContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        //        // 
        //        var models = dBContext.Indicators.AsNoTracking().ToList();
        //        foreach (var item in models)
        //        {
        //            if (!_Dictionary.ContainsKey(item.Code))
        //                _Dictionary.Add(item.Code, item);
        //        }

        //        foreach (var item in models)
        //        {
        //            if (!_GuidDictionary.ContainsKey(item.Id))
        //                _GuidDictionary.Add(item.Id, item);
        //        }
        //    }
        //}
    }
}
