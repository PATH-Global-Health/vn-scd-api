using Data.DbContexts;
using Data.Entities.SMDEntities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.Extenstions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Mapster;
using Data.Entities;

namespace Services.LookupServices
{
    public class SMDUserLookupService
    {
        protected readonly IServiceScopeFactory _ScopeFactory;
        private readonly ConcurrentDictionary<string, SMDUser> _DictionaryUser = new ConcurrentDictionary<string, SMDUser>();
        private readonly ConcurrentDictionary<string, Guid> _DictionaryId = new ConcurrentDictionary<string, Guid>();

        public SMDUserLookupService(IServiceScopeFactory scopeFactory)
        {
            _ScopeFactory = scopeFactory;
            Initialize();
        }

        public SMDUser Add(SMDUser user, AppDbContext dbContext)
        {
            try
            {
                dbContext.SMDUsers.Add(user);
                dbContext.SaveChanges();
                _DictionaryUser.AddOrUpdate(user.Username, user, (username, oldUser) => user);
                _DictionaryId.AddOrUpdate(user.Username, user.UnitId, (username, oldUser) => user.UnitId);
                return user;
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException is SqlException)
                    if ((e.InnerException as SqlException).IsDuplicateKeyException())
                    {
                        var oldUser = dbContext.SMDUsers.FirstOrDefault(_ => _.Username == user.Username);
                        return oldUser;
                    }
            }
            return null;
        }

        public SMDUser Add(SMDUser user)
        {
            using (var scope = _ScopeFactory.CreateScope())
            {
                using (AppDbContext dBContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
                {
                    try
                    {
                        dBContext.SMDUsers.Add(user);
                        dBContext.SaveChanges();
                        _DictionaryUser.AddOrUpdate(user.Username, user, (username, oldUser) => user);
                        _DictionaryId.AddOrUpdate(user.Username, user.UnitId, (username, oldUser) => user.UnitId);
                        return user;
                    }
                    catch (DbUpdateException e)
                    {
                        if (e.InnerException is SqlException)
                            if ((e.InnerException as SqlException).IsDuplicateKeyException())
                            {
                                var oldUser = dBContext.SMDUsers.FirstOrDefault(_ => _.Username == user.Username);
                                return oldUser;
                            }
                    }
                    return null;
                }
            }
        }

        public SMDUser Lookup(string username)
        {
            if (!_DictionaryUser.TryGetValue(username, out SMDUser user))
                user = LookupDatabase(username);
            return user;
        }

        public Guid LookupUnitId(string username)
        {
            if (!_DictionaryId.TryGetValue(username, out Guid id))
            {
                var user = LookupDatabase(username);
                if (user != null)
                    return user.UnitId;
            }
            return id;
        }

        private void Initialize()
        {
            using (var scope = _ScopeFactory.CreateScope())
            {
                using (AppDbContext dBContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
                {
                    var models = dBContext.SMDUsers.Where(_ => !_.IsDeleted).AsNoTracking().ToList();
                    foreach (var item in models)
                    {
                        if (!_DictionaryUser.ContainsKey(item.Username))
                            _DictionaryUser.TryAdd(item.Username, item);
                        if (!_DictionaryId.ContainsKey(item.Username))
                            _DictionaryId.TryAdd(item.Username, item.UnitId);
                    }
                }
            }
        }

        private SMDUser LookupDatabase(string username)
        {
            using (var scope = _ScopeFactory.CreateScope())
            {
                using (AppDbContext dBContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
                {
                    var user = dBContext.SMDUsers.FirstOrDefault(_ => !_.IsDeleted && _.Username == username);
                    if (user != null)
                    {
                        if (!_DictionaryUser.ContainsKey(user.Username))
                            _DictionaryUser.TryAdd(user.Username, user);
                        if (!_DictionaryId.ContainsKey(user.Username))
                            _DictionaryId.TryAdd(user.Username, user.UnitId);
                        return user;
                    }
                    return null;
                }
            }
        }

        public void InitSMDUsers()
        {
            using (var scope = _ScopeFactory.CreateScope())
            {
                using (AppDbContext dBContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
                {
                    var projects = dBContext.Projects.Where(_ => !_.IsDeleted).AsNoTracking().ToList();

                    foreach (var item in projects)
                    {
                        if (!_DictionaryUser.ContainsKey(item.Username))
                        {
                            var user = item.Adapt<SMDUser>();
                            dBContext.SMDUsers.Add(user);
                            _DictionaryUser.TryAdd(item.Username, user);
                        }
                        if (!_DictionaryId.ContainsKey(item.Username))
                        {
                            _DictionaryId.TryAdd(item.Username, item.Id);
                        }
                    }

                    var cbos = dBContext.Units.Where(_ => !_.IsDeleted && _.ProjectId.HasValue).AsNoTracking().ToList();
                    foreach (var item in cbos)
                    {
                        if (!_DictionaryId.ContainsKey(item.Username))
                        {
                            if (!_DictionaryUser.ContainsKey(item.Username))
                            {
                                var user = item.Adapt<SMDUser>();
                                dBContext.SMDUsers.Add(user);
                                _DictionaryUser.TryAdd(item.Username, user);
                            }
                            if (!_DictionaryId.ContainsKey(item.Username))
                            {
                                _DictionaryId.TryAdd(item.Username, item.Id);
                            }
                        }
                    }
                    dBContext.SaveChanges();
                }
            }
        }
    }
}