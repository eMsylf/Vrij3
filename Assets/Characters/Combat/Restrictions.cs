using System;

namespace RanchyRats.Gyrus
{
    [Flags] public enum Restrictions
    {
        None = 0,
        Move = 1,
        Rotate = 2,
        StaminaRecovery = 4
    }
}