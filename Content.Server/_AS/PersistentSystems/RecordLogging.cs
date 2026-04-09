using Content.Server.Administration.Logs;
using Content.Server.Database;
using Content.Shared._AS.PersistentSystems;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Robust.Shared.Network;

namespace Content.Server._AS.PersistentSystems;

public abstract class RecordLogging
{
    [Dependency] private readonly IAdminLogManager _adminLog = default!;
    [Dependency] private readonly ISawmill _sawmill = default!;

    private LogStringHandler LogRecordCreated(LogStringHandler logStringHandler, RecordCharacter record)
    {
        _sawmill.Info($"Record {record.RecordType}:{record.Id} created.");
        logStringHandler.AppendFormatted($"{record.AuthorUserId} created {record.RecordType}: {record.Id}.\n");
        return logStringHandler;
    }

    public void LogPersonalNoteCreated(RecordPersonalNote record)
    {
        var adminLog = new LogStringHandler();
        adminLog = LogRecordCreated(adminLog, record.RecordCharacter);
        adminLog.AppendFormatted($"Title: {record.Title}\n" +
                                 $"Body: {record.Note}");
        _adminLog.Add(LogType.RecordCreate, LogImpact.Medium, ref adminLog);
    }

    public void LogRecordUpdated(NetUserId senderSessionUserId, int recordId, RecordUpdateResult result)
    {
        var adminLog = new LogStringHandler();
        switch (result.Status)
        {
            case RecordUpdateStatus.NoChange:
                _sawmill.Info($"Record {recordId} updated but no changes were made.");
                break;

            case RecordUpdateStatus.Updated:
                _sawmill.Info($"Record {recordId} updated.");
                adminLog.AppendFormatted($"{senderSessionUserId} updated record {recordId}.");
                foreach (var edit in result.Edits ?? [])
                {
                    adminLog.AppendFormatted($"\n({edit.UpdateId}) {edit.Field}: {edit.OldValue} -> {edit.NewValue}");
                }
                _adminLog.Add(LogType.RecordEdit, LogImpact.Medium, ref adminLog);
                break;

            case RecordUpdateStatus.Prohibited:
                _sawmill.Warning($"Attempted to update record {recordId} but it was prohibited.");
                adminLog.AppendFormatted($"{senderSessionUserId} attempted to update record {recordId} as  but it was prohibited.");
                _adminLog.Add(LogType.RecordEdit, LogImpact.High, ref adminLog);
                break;

            case RecordUpdateStatus.NotFound:
                _sawmill.Error($"Attempted update on non-existent record {recordId}.");
                break;

            case RecordUpdateStatus.Failed:
                _sawmill.Error($"Failed to update record {recordId}.");
                break;

            default:
                _sawmill.Error($"Unhandled record update status {result.Status}.");
                break;
        }
    }

    public void LogRecordHidden(NetUserId senderSessionUserId, int recordId, RecordUpdateStatus status)
    {
        var adminLog = new LogStringHandler();
        switch (status)
        {
            case RecordUpdateStatus.Updated:
                _sawmill.Info($"Record {recordId} was hidden.");
                adminLog.AppendFormatted($"{senderSessionUserId} hid record {recordId}.");
                _adminLog.Add(LogType.RecordHide, LogImpact.Medium, ref adminLog);
                break;

            case RecordUpdateStatus.NoChange:
                _sawmill.Warning($"Attempted to hide record {recordId} but it was already hidden.");
                adminLog.AppendFormatted($"{senderSessionUserId} attempted to hide record {recordId} but it was already hidden.");
                _adminLog.Add(LogType.RecordEdit, LogImpact.High, ref adminLog);
                break;

            case RecordUpdateStatus.Prohibited:
                _sawmill.Warning($"Attempted to hide record {recordId} but it was prohibited.");
                adminLog.AppendFormatted($"{senderSessionUserId} attempted to hide record {recordId} but it was prohibited.");
                _adminLog.Add(LogType.RecordEdit, LogImpact.High, ref adminLog);
                break;

            case RecordUpdateStatus.NotFound:
                _sawmill.Error($"Attempted hide on non-existent record {recordId}.");
                break;

            case RecordUpdateStatus.Failed:
                _sawmill.Error($"Failed to hide record {recordId}.");
                break;

            default:
                _sawmill.Error($"Unhandled record update status {status}.");
                break;
        }
    }

    public void LogRecordUnhidden(NetUserId senderSessionUserId, int recordId, RecordUpdateStatus status)
    {
        var adminLog = new LogStringHandler();
        switch (status)
        {
            case RecordUpdateStatus.Updated:
                _sawmill.Info($"Record {recordId} was unhidden.");
                adminLog.AppendFormatted($"{senderSessionUserId} unhid record {recordId}.");
                _adminLog.Add(LogType.RecordHide, LogImpact.Medium, ref adminLog);
                break;

            case RecordUpdateStatus.NoChange:
                _sawmill.Warning($"Attempted to unhide record {recordId} but it was not hidden.");
                adminLog.AppendFormatted($"{senderSessionUserId} attempted to unhide record {recordId} but it was not hidden.");
                _adminLog.Add(LogType.RecordEdit, LogImpact.Medium, ref adminLog);
                break;

            case RecordUpdateStatus.Prohibited:
                _sawmill.Warning($"Attempted to unhide record {recordId} but it was prohibited.");
                adminLog.AppendFormatted($"{senderSessionUserId} attempted to unhide record {recordId} but it was prohibited.");
                _adminLog.Add(LogType.RecordEdit, LogImpact.Extreme, ref adminLog);
                break;

            case RecordUpdateStatus.NotFound:
                _sawmill.Error($"Attempted unhide on non-existent record {recordId}.");
                break;

            case RecordUpdateStatus.Failed:
                _sawmill.Error($"Failed to unhide record {recordId}.");
                break;

            default:
                _sawmill.Error($"Unhandled record update status {status}.");
                break;
        }
    }

    public void LogRecordDeleted(NetUserId senderSessionUserId, int recordId, RecordUpdateStatus status)
    {
        var adminLog = new LogStringHandler();
        switch (status)
        {
            case RecordUpdateStatus.Updated:
                _sawmill.Info($"Record {recordId} was deleted.");
                adminLog.AppendFormatted($"{senderSessionUserId} deleted record {recordId}.");
                _adminLog.Add(LogType.RecordHide, LogImpact.High, ref adminLog);
                break;

            case RecordUpdateStatus.NoChange:
                _sawmill.Warning($"Attempted to delete record {recordId} but it was already deleted.");
                break;

            case RecordUpdateStatus.Prohibited:
                _sawmill.Warning($"Attempted to delete record {recordId} but it was prohibited.");
                adminLog.AppendFormatted($"{senderSessionUserId} attempted to delete record {recordId} but it was prohibited.");
                _adminLog.Add(LogType.RecordEdit, LogImpact.Extreme, ref adminLog);
                break;

            case RecordUpdateStatus.NotFound:
                _sawmill.Error($"Attempted delete on non-existent record {recordId}.");
                break;

            case RecordUpdateStatus.Failed:
                _sawmill.Error($"Failed to delete record {recordId}.");
                break;

            default:
                _sawmill.Error($"Unhandled record update status {status}.");
                break;
        }
    }

    public void LogRecordRestored(NetUserId senderSessionUserId, int recordId, RecordUpdateStatus status)
    {
        var adminLog = new LogStringHandler();
        switch (status)
        {
            case RecordUpdateStatus.Updated:
                _sawmill.Info($"Record {recordId} was restored.");
                adminLog.AppendFormatted($"{senderSessionUserId} restored record {recordId}.");
                _adminLog.Add(LogType.RecordHide, LogImpact.High, ref adminLog);
                break;

            case RecordUpdateStatus.NoChange:
                _sawmill.Warning($"Attempted to restore record {recordId} but it wasn't deleted.");
                break;

            case RecordUpdateStatus.Prohibited:
                _sawmill.Warning($"Attempted to restore record {recordId} but it was prohibited.");
                adminLog.AppendFormatted($"{senderSessionUserId} attempted to delete record {recordId} but it was prohibited.");
                _adminLog.Add(LogType.RecordEdit, LogImpact.Extreme, ref adminLog);
                break;

            case RecordUpdateStatus.NotFound:
                _sawmill.Error($"Attempted restore on non-existent record {recordId}.");
                break;

            case RecordUpdateStatus.Failed:
                _sawmill.Error($"Failed to restore record {recordId}.");
                break;

            default:
                _sawmill.Error($"Unhandled record update status {status}.");
                break;
        }
    }
}
