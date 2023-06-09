﻿using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Validation;
using PointsService.Validation;

namespace PointsService.Models;

public class AdminTransactionRequest : IValidatableObject
{
    [Required]
    [StringLength(30)]
    [DiscordId]
    public string GuildId { get; set; } = null!;

    [Required]
    [StringLength(30)]
    [DiscordId]
    public string UserId { get; set; } = null!;

    [Required]
    [Range(1, int.MaxValue)]
    public int Amount { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        => validationContext.GetRequiredService<TransactionRequestValidator>().Validate(this);
}
