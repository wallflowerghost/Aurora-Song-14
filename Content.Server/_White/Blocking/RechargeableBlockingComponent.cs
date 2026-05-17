namespace Content.Server._White.Blocking;

[RegisterComponent]
public sealed partial class RechargeableBlockingComponent : Component
{
    // Aurora's Song | As of right now none of these variables actually define or change any behaviour in the system's
    // logic, which we may change in the future but this was the easiest way to make this work with the more updated power
    // and battery systems we've inherited from Upstream(WizDen)

    // [DataField, ViewVariables(VVAccess.ReadWrite)]
    // public float DischargedRechargeRate = 1.33f;
    //
    // [DataField, ViewVariables(VVAccess.ReadWrite)]
    // public float ChargedRechargeRate = 2f;

    // [ViewVariables] //Aurora's Song | This state has been the bane of my existence, good riddance.
    // public bool Discharged;


}
