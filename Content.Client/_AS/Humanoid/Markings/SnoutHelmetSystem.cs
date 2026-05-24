// Originally from https://github.com/DeltaV-Station/Delta-v/pull/3875, Edited by snezshiba with permission

using Content.Shared.Body;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;

namespace Content.Shared._AS.Humanoid.Markings;

public sealed class SnoutHelmetSystem : EntitySystem
{
    [Dependency] private readonly SharedVisualBodySystem _visualBody = default!;

    private const HumanoidVisualLayers MarkingToQuery = HumanoidVisualLayers.Head;
    private const int MaximumMarkingCount = 0;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SnoutHelmetComponent, ComponentStartup>(OnComponentStartup);
    }

    private void OnComponentStartup(EntityUid uid, SnoutHelmetComponent component, ComponentStartup args)
    {
        if (!_visualBody.TryGatherMarkingsData(uid,
                [component.Layer],
                out _,
                out _,
                out var applied))
        {
            return;
        }

        if (!applied.TryGetValue(component.Organ, out var markingsSet))
            return;

        var markings = markingsSet[component.Layer];

        foreach (var marking in markings)
        {
            // Cannot figure out how to dynamically set custom race
            var markingLower = marking.MarkingId.ToLower();
            if (markingLower.Contains("vulp"))
            {
                component.AlternateHelmet = "vulpkanin";
            }
            else if (markingLower.Contains("feroxi") || markingLower.Contains("synth"))
            {
                component.AlternateHelmet = "feroxi";
            }
        }
    }
}
