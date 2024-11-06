﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AxaGuilDEv.Ecotag.Server.Groups.Database.Users;

public class UsersRepository
{
    public const string NameIdentifierAlreadyExists = "NameIdentifierAlreadyExists";
    private readonly IMemoryCache _cache;
    private readonly GroupContext _groupsContext;

    public UsersRepository(GroupContext groupsContext, IMemoryCache cache)
    {
        _groupsContext = groupsContext;
        _cache = cache;
    }

    public async Task<List<UserDataModel>> GetAllUsersAsync()
    {
        var userModelEnum = await _groupsContext.Users.ToListAsync();
        var resultList = userModelEnum.Select(userModel => userModel.ToListUserDataModel()).ToList();
        return resultList;
    }

    public async Task<UserDataModelWithGroups> GetUserByNameIdentifierWithGroupIdsAsync(string nameIdentity)
    {
        var user = await _groupsContext.Users.AsNoTracking().Include(user => user.GroupUsers)
            .FirstOrDefaultAsync(u => u.NameIdentifier == nameIdentity.ToLower());
        return user?.ToUserDataModelWithGroups();
    }

    public async Task<UserDataModel> GetUserByNameIdentifierAsync(string nameIdentifier)
    {
        var cacheEntry = await _cache.GetOrCreateAsync($"GetUserByNameIdentifierAsync({nameIdentifier})", async entry =>
        {
            var user = await _groupsContext.Users.AsNoTracking().Include(user => user.GroupUsers)
                .FirstOrDefaultAsync(u => u.NameIdentifier == nameIdentifier.ToLower());
            entry.AbsoluteExpirationRelativeToNow =
                user == null ? TimeSpan.FromMilliseconds(1) : TimeSpan.FromHours(1);
            entry.SlidingExpiration = TimeSpan.FromMinutes(1);
            return user;
        });

        return cacheEntry?.ToUserDataModel();
    }

    public async Task<ResultWithError<string, ErrorResult>> CreateUserAsync(string email, string nameIdentifier)
    {
        var commandResult = new ResultWithError<string, ErrorResult>();
        var nameIdentifierLowerCase = nameIdentifier.ToLower();
        var emailToLower = email.ToLower();
        var existingUser = await _groupsContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == emailToLower);
        if (existingUser != null)
        {
            existingUser.NameIdentifier = nameIdentifier;
            await _groupsContext.SaveChangesAsync();
            return new ResultWithError<string, ErrorResult>() { Data = existingUser.Id.ToString() };
        }
        
        var userModel = new UserModel
        {
            Email = email.ToLower(),
            NameIdentifier = nameIdentifierLowerCase
        };
        _groupsContext.Users.Add(userModel);
        try
        {
            await _groupsContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            commandResult.Error = new ErrorResult { Key = NameIdentifierAlreadyExists };
            return commandResult;
        }

        commandResult.Data = userModel.Id.ToString();
        return commandResult;
    }
}