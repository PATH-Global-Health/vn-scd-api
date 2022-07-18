using Data.DbContexts;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.LookupServices
{
    public interface ILookupService<T> where T : BaseCodeEntity
    {
        ICollection<T> GetData();
        T Lookup(string key);
        T Lookup(Guid key);
        void Refresh();
    }

    public abstract class LookupServiceBase<T> : ILookupService<T> where T : BaseCodeEntity
    {
        protected readonly IServiceScopeFactory _ScopeFactory;
        protected ICollection<T> _Data;
        protected IDictionary<string, T> _Dictionary;
        protected IDictionary<Guid, T> _GuidDictionary;

        public LookupServiceBase(IServiceScopeFactory scopeFactory)
        {
            _ScopeFactory = scopeFactory;
            Refresh();
        }

        public ICollection<T> GetData()
        {
            return _Data;
        }

        public virtual T Lookup(string key)
        {
            T value;
            if (_Dictionary.TryGetValue(key, out value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        public T Lookup(Guid key)
        {
            T value;
            if (_GuidDictionary.TryGetValue(key, out value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        public void Refresh()
        {
            using (var scope = _ScopeFactory.CreateScope())
            {
                _Dictionary = new Dictionary<string, T>();
                _GuidDictionary = new Dictionary<Guid, T>();
                _Data = new List<T>();
                using (AppDbContext dBContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
                {
                    var dbSet = dBContext.Set<T>();
                    var models = dbSet.Where(_ => !_.IsDeleted).AsNoTracking().ToList();
                    _Data = models;
                    foreach (var item in models)
                    {
                        if (!_Dictionary.ContainsKey(item.Code))
                            _Dictionary.Add(item.Code, item);
                    }

                    foreach (var item in models)
                    {
                        if (!_GuidDictionary.ContainsKey(item.Id))
                            _GuidDictionary.Add(item.Id, item);
                    }
                }
            }
        }
    }
}
