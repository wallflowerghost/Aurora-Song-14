using System.Threading.Tasks;
using Content.Server.Access.Systems;
using Content.Server.Database;
using Content.Server.GameTicking.Events;
using Content.Shared._AS.PersistentSystems;
using Robust.Shared.Player;

namespace Content.Server._AS.PersistentSystems;

public sealed class PersonalNoteAppManager : EntitySystem
{
    [Dependency] private readonly PersonalNoteSystem _record = default!;

    private ISawmill _sawmill = default!;

    public override void Initialize()
    {
        _sawmill = Logger.GetSawmill("record");
        SubscribeLocalEvent<CreatePersonalNoteMessage>(OnCreatePersonalNote);
        SubscribeLocalEvent<UpdatePersonalNoteMessage>(OnSavePersonalNote);
        SubscribeLocalEvent<HidePersonalNoteMessage>(HidePersonalNote);
        base.Initialize();
    }

    private void SendUpdateResult(ICommonSession session, int recordId, RecordUpdateStatus status)
    {
        RaiseNetworkEvent(new RecordUpdateStatusMessage(recordId, status), session);
    }

    private void UpdateNotesInterface(List<RecordPersonalNote> notes)
    {

    }

    private async void OnNoteUIOpen(int profileId)
    {
        try
        {
            var notes = await _record.GetPersonalNotes(profileId);
            UpdateNotesInterface(notes);
        }
        catch (Exception e)
        {
            _sawmill.Error($"Failed to load personal notes: {e}");
        }
    }

    private void OnCreatePersonalNote(CreatePersonalNoteMessage msg, EntitySessionEventArgs args)
    {
        _record.CreatePersonalNote(args.SenderSession, msg.Title, msg.Note);
    }

    private async void OnSavePersonalNote(UpdatePersonalNoteMessage msg, EntitySessionEventArgs args)
    {
        try
        {
            var result = await _record.UpdatePersonalNote(args.SenderSession, msg.RecordId, msg.Title, msg.Note);
            SendUpdateResult(args.SenderSession, msg.RecordId, result.Status);
        }
        catch (Exception e)
        {
            _sawmill.Error($"Failed to update personal note: {e}");
        }
    }

    private async void HidePersonalNote(HidePersonalNoteMessage msg, EntitySessionEventArgs args)
    {
        try
        {

            var result = await _record.HidePersonalNote(args.SenderSession, msg.RecordId);
            SendUpdateResult(args.SenderSession, msg.RecordId, result);
        }
        catch (Exception e)
        {
            _sawmill.Error($"Failed to hide personal note: {e}");
        }
    }
}
