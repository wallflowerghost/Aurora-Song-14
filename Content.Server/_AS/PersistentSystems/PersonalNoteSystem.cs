using System.Threading.Tasks;
using Content.Server.Access.Systems;
using Content.Server.Database;
using Content.Server.GameTicking.Events;
using Content.Shared._AS.PersistentSystems;
using Robust.Shared.Player;

namespace Content.Server._AS.PersistentSystems;

public sealed class PersonalNoteSystem : EntitySystem
{
    [Dependency] private readonly IdCardSystem _idCard = default!;
    [Dependency] private readonly IServerDbManager _db = default!;
    [Dependency] private readonly ISawmill _sawmill = default!;

    private int _roundId;

    public override void Initialize()
    {
        SubscribeLocalEvent<RoundStartingEvent>(ev => _roundId = ev.Id);
        SubscribeAllEvent<CreatePersonalNoteMessage>(OnCreatePersonalNote);
        SubscribeAllEvent<UpdatePersonalNoteMessage>(OnUpdatePersonalNote);
        SubscribeAllEvent<HidePersonalNoteMessage>(OnHidePersonalNote);
        base.Initialize();
    }

    private async Task<List<RecordPersonalNote>> GetPersonalNotes(int profileId)
    {
        return await _db.GetPersonalNotes(profileId);
    }

    private async void OnCreatePersonalNote(CreatePersonalNoteMessage msg, EntitySessionEventArgs args)
    {
        try
        {
            if (args.SenderSession.AttachedEntity is not { } attachedEntity)
                return;
            if (GetProfileId(attachedEntity) is not { } profileId)
                return;

            var newRecord = await _db.AddPersonalNote(args.SenderSession.UserId, profileId, msg.Title, msg.Note, _roundId);
        }
        catch (Exception e)
        {
            _sawmill.Error($"Failed to create personal note: {e}");
        }
    }

    private async void OnUpdatePersonalNote(UpdatePersonalNoteMessage msg, EntitySessionEventArgs args)
    {
        try
        {
            if (args.SenderSession.AttachedEntity is not { } attachedEntity)
                return;
            if (GetProfileId(attachedEntity) is not { } profileId)
                return;
            var result = await _db.UpdatePersonalNote(args.SenderSession.UserId, profileId, msg.RecordId, msg.Title, msg.Note);

            SendUpdateResult(args.SenderSession, msg.RecordId, result);
        }
        catch (Exception e)
        {
            _sawmill.Error($"Failed to update personal note: {e}");
        }
    }

    private async void OnHidePersonalNote(HidePersonalNoteMessage msg, EntitySessionEventArgs args)
    {
        try
        {
            if (args.SenderSession.AttachedEntity is not { } attachedEntity)
                return;
            if (GetProfileId(attachedEntity) is not { } profileId)
                return;
            var result = await _db.HideRecord(args.SenderSession.UserId, msg.RecordId, profileId);
            SendUpdateResult(args.SenderSession, msg.RecordId, result);
        }
        catch (Exception e)
        {
            _sawmill.Error($"Failed to hide personal note: {e}");
        }
    }

    private void SendUpdateResult(ICommonSession session, int recordId, RecordUpdateResult result)
    {
        RaiseNetworkEvent(new RecordUpdateStatusMessage(recordId, result), session);
    }

    private int? GetProfileId(EntityUid uid)
    {
        _idCard.TryFindIdCard(uid, out var idCard);

        return idCard.Comp.ProfileId;
    }
}
