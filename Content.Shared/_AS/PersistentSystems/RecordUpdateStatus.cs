namespace Content.Shared._AS.PersistentSystems;

public struct RecordUpdateResult()
{
    public RecordUpdateStatus Status = RecordUpdateStatus.NoChange;
    public List<Edit>? Edits;
}

public enum RecordUpdateStatus
{
    Failed, // Generic failure.
    NotFound, // The record was not found.
    Prohibited, // The user is not allowed to edit this record.
    NoChange, // Successful update, but no changes were made.
    Updated, // Successful update, changes applied.
}

public readonly struct Edit(string field, int editId, string? oldValue, string? newValue)
{
    public readonly string Field = field;
    public readonly int UpdateId = editId;
    public readonly string? OldValue = oldValue;
    public readonly string? NewValue = newValue;
}
