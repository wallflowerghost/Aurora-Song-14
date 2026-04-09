using System.Threading.Tasks;
using Content.Server.Access.Systems;
using Content.Server.Database;
using Content.Server.GameTicking.Events;
using Content.Shared._AS.PersistentSystems;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Server._AS.PersistentSystems;

public sealed class PersonalNoteSystem : EntitySystem
{
    [Dependency] private readonly IdCardSystem _idCard = default!;
    [Dependency] private readonly IServerDbManager _db = default!;
    [Dependency] private readonly ISawmill _sawmill = default!;
    [Dependency] private readonly RecordLogging _logging = default!;

    private int _roundId;

    public override void Initialize()
    {
        SubscribeLocalEvent<RoundStartingEvent>(ev => _roundId = ev.Id);
        base.Initialize();
    }

    public async Task<List<RecordPersonalNote>> GetPersonalNotes(int profileId)
    {
        return await _db.GetPersonalNotes(profileId);
    }

    public async void CreatePersonalNote(ICommonSession session, string title, string note)
    {
        try
        {
            if (session.AttachedEntity is not { } attachedEntity || GetProfileId(attachedEntity) is not { } profileId)
            {
                _sawmill.Warning($"Failed to resolve ID for {session.UserId}");
                return;
            }

            if (title == string.Empty)
                title = "Untitled";
            var newRecord = await _db.AddPersonalNote(session.UserId, profileId, title, note, _roundId);
            _logging.LogPersonalNoteCreated(newRecord);
        }
        catch (Exception e)
        {
            _sawmill.Error($"Failed to create personal note: {e}");
        }
    }

    public async Task<RecordUpdateResult> UpdatePersonalNote(ICommonSession session, int recordId, string? title, string? note)
    {
        var result = new RecordUpdateResult();
        if (session.AttachedEntity is not { } attachedEntity || GetProfileId(attachedEntity) is not { } profileId)
        {
            result.Status = RecordUpdateStatus.Prohibited;
            _logging.LogRecordUpdated(session.UserId, recordId, result);
            return result;
        }

        result = await _db.UpdatePersonalNote(session.UserId, profileId, recordId, title, note);
        _logging.LogRecordUpdated(session.UserId, recordId, result);
        if (result.Status == RecordUpdateStatus.NotFound)
            CreatePersonalNote(session, title ?? "", note ?? "");
        return result;
    }

    public async Task<RecordUpdateStatus> HidePersonalNote(ICommonSession session, int recordId)
    {
        RecordUpdateStatus result;
        if (session.AttachedEntity is not { } attachedEntity || GetProfileId(attachedEntity) is not { } profileId)
        {
            result = RecordUpdateStatus.Prohibited;
            _logging.LogRecordHidden(session.UserId, recordId, result);
            return result;
        }

        result = await _db.HideRecord(session.UserId, recordId, profileId);
        _logging.LogRecordHidden(session.UserId, recordId, result);
        return result;
    }

    private int? GetProfileId(EntityUid uid)
    {
        _idCard.TryFindIdCard(uid, out var idCard);

        return idCard.Comp.ProfileId;
    }
}
