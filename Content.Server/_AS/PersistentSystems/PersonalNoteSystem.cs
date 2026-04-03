using Content.Server.Access.Systems;
using Content.Server.Preferences.Managers;
using Content.Shared.Preferences;
using Robust.Shared.Player;

namespace Content.Server._AS.PersistentSystems;

public sealed class PersonalNoteSystem : EntitySystem
{
    [Dependency] private readonly IdCardSystem _idCard = default!;

    private int? GetProfileId(EntityUid uid)
    {
        _idCard.TryFindIdCard(uid, out var idCard);

        return idCard.Comp.ProfileId;
    }
}
