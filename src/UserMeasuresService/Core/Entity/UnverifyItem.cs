﻿namespace UserMeasuresService.Core.Entity;

public class UnverifyItem : UserMeasureBase
{
    public DateTime ValidTo { get; set; }
    public long LogSetId { get; set; }
}
