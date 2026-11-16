// Copyright 2026 Spellbound Studio Inc.

using System;
using System.Collections.Generic;

namespace Spellbound.Modifiers {
    [Serializable]
    public abstract class ContributionSpecification : ModifierGrant {
        public abstract IEnumerable<(StatDefinition stat, ContributionType type, Magnitude amount)>
                StatContributions { get; }

        public abstract void Bake(System.Random rng, List<BakedRoll> into);

        public abstract void ApplyBaked(StatBlock stats, uint sourceId, BakedRoll[] baked);

        public sealed override void Roll(
            System.Random rng, uint sourceId, List<BakedRoll> baked, List<RolledModifier> modifiers) =>
                Bake(rng, baked);

        public sealed override void Apply(Modifiable target, uint sourceId, in RolledGrants rolled) =>
                ApplyBaked(target.Stats, sourceId, rolled.baked);

        protected static float BakedValueFor(BakedRoll[] baked, uint statHash) {
            if (baked != null) {
                for (var i = 0; i < baked.Length; i++) {
                    if (baked[i].statHash == statHash)
                        return baked[i].value;
                }
            }

            return 0f;
        }
    }
}
