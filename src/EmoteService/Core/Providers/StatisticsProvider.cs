﻿using EmoteService.Core.Entity;
using GrillBot.Core.Database;
using Microsoft.EntityFrameworkCore;

namespace EmoteService.Core.Providers;

public class StatisticsProvider(EmoteServiceContext dbContext) : IStatisticsProvider
{
    public async Task<Dictionary<string, long>> GetTableStatisticsAsync()
    {
        return new Dictionary<string, long>
        {
            { nameof(dbContext.EmoteDefinitions), await dbContext.EmoteDefinitions.LongCountAsync() },
            { nameof(dbContext.EmoteUserStatItems), await dbContext.EmoteUserStatItems.LongCountAsync() }
        };
    }
}
