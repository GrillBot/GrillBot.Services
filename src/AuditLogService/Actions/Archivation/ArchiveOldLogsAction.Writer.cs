﻿using System.Xml.Linq;
using AuditLogService.Core.Entity;
using AuditLogService.Models.Response;
using File = AuditLogService.Core.Entity.File;

namespace AuditLogService.Actions.Archivation;

public partial class ArchiveOldLogsAction
{
    private static ArchivationResult CreateArchive(List<LogItem> items)
    {
        var result = new ArchivationResult();
        var xml = new XElement(
            "AuditLogBackup",
            new XAttribute("CreatedAt", DateTime.UtcNow.ToString("o")),
            new XAttribute("Count", items.Count)
        );

        foreach (var item in items)
            xml.Add(ProcessLogItem(item, result));

        result.Xml = xml.ToString(SaveOptions.DisableFormatting | SaveOptions.OmitDuplicateNamespaces);
        result.ChannelIds = result.ChannelIds.Distinct().ToList();
        result.GuildIds = result.GuildIds.Distinct().ToList();
        result.UserIds = result.UserIds.Distinct().ToList();
        result.Files = result.Files.Distinct().ToList();
        result.ItemsCount = items.Count;
        result.TotalFilesSize = items.SelectMany(o => o.Files).Sum(o => o.Size);

        return result;
    }

    private static XElement ProcessLogItem(LogItem item, ArchivationResult result)
    {
        var xml = new XElement(
            "Item",
            new XAttribute("Id", item.Id.ToString()),
            new XAttribute("CreatedAt", item.CreatedAt.ToString("o")),
            new XAttribute("Type", item.Type.ToString())
        );

        if (!string.IsNullOrEmpty(item.GuildId))
        {
            result.GuildIds.Add(item.GuildId);
            xml.Add(new XAttribute("GuildId", item.GuildId));
        }

        if (!string.IsNullOrEmpty(item.UserId))
        {
            result.UserIds.Add(item.UserId);
            xml.Add(new XAttribute("UserId", item.UserId));
        }

        if (!string.IsNullOrEmpty(item.ChannelId))
        {
            result.ChannelIds.Add(item.ChannelId);
            xml.Add(new XAttribute("ChannelId", item.ChannelId));
        }

        if (!string.IsNullOrEmpty(item.DiscordId))
            xml.Add(new XAttribute("DiscordId", item.DiscordId));

        foreach (var file in item.Files)
            xml.Add(ProcessFile(file, result));

        var dataXml = ProcessData(item, result);
        if (dataXml is not null) xml.Add(dataXml);

        return xml;
    }

    private static XElement ProcessFile(File file, ArchivationResult result)
    {
        result.Files.Add(file.Filename);

        var xml = new XElement(
            "File",
            new XAttribute("Id", file.Id),
            new XAttribute("Filename", file.Filename),
            new XAttribute("Size", file.Size)
        );

        if (!string.IsNullOrEmpty(file.Extension))
            xml.Add(new XAttribute("Extension", file.Extension));
        return xml;
    }
}
