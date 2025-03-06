﻿using GrillBot.Core.Database.Repository;
using GrillBot.Core.Managers.Performance;
using RubbergodService.Core.Entity;

namespace RubbergodService.Core.Repository;

public class RubbergodServiceRepository : RepositoryBase<RubbergodServiceContext>
{
    public RubbergodServiceRepository(RubbergodServiceContext context, ICounterManager counterManager) : base(context, counterManager)
    {
        Karma = new KarmaRepository(Context, counterManager);
        Statistics = new StatisticsRepository(Context, counterManager);
    }

    public KarmaRepository Karma { get; }
    public StatisticsRepository Statistics { get; }
}
