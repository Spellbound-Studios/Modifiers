// Copyright 2026 Spellbound Studio Inc.

using System;
using System.Collections.Generic;

namespace Spellbound.Modifiers {
    [Serializable]
    public abstract class ModifierGrant {
        public abstract bool IsValid { get; }

        public abstract void Roll(
            System.Random rng, uint sourceId, List<BakedRoll> baked, List<RolledModifier> modifiers);

        public abstract void Apply(Modifiable target, uint sourceId, in RolledGrants rolled);
    }
}
