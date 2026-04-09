using Robust.Shared.Serialization;

namespace Content.Shared._AS.PersistentSystems;

[Serializable, NetSerializable]
public sealed class PersonalNotes(List<PersonalNote> notes) : BoundUserInterfaceState
{
    public List<PersonalNote> Notes = notes;
}

[Serializable, NetSerializable]
public sealed class PersonalNote(int recordId, string title, string note, DateTime createdAt)
{
    public int RecordId = recordId;
    public string Title = title;
    public string Note = note;
    public DateTime CreatedAt = createdAt;
}

[Serializable, NetSerializable]
public sealed class CreatePersonalNoteMessage(string title, string note) : BoundUserInterfaceMessage
{
    public string Title = title;
    public string Note = note;
}

[Serializable, NetSerializable]
public sealed class UpdatePersonalNoteMessage(int recordId, string? title, string? note) : BoundUserInterfaceMessage
{
    public int RecordId = recordId;
    public string? Title = title;
    public string? Note = note;
}

[Serializable, NetSerializable]
public sealed class HidePersonalNoteMessage(int recordId) : BoundUserInterfaceMessage
{
    public int RecordId = recordId;
}

[Serializable, NetSerializable]
public sealed class RecordUpdateStatusMessage(int recordId, RecordUpdateStatus status) : EntityEventArgs
{
    public int RecordId = recordId;
    public RecordUpdateStatus Status = status;
}
