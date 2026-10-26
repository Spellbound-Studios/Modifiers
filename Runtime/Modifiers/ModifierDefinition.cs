// Copyright 2026 Spellbound Studio Inc.

using System.Collections.Generic;
using Spellbound.Core.Authoring;
using Spellbound.Core.Registries;
using UnityEngine;

namespace Spellbound.Modifiers {
    [CreateAssetMenu(menuName = "Spellbound/ModifierLib/Modifier Definition")]
    public class ModifierDefinition : HashedScriptableObject, IAuthoredDefinition {
        [Header("Identity"), SerializeField] private string modifierName;
        [SerializeField] private string displayName;
        [SerializeField, TextArea] private string description;
        [SerializeField, SpritePreview] private Sprite icon;
        [SerializeReference] private List<ContributionSpecification> contributions = new();

        public string ModifierName => modifierName;
        public string DisplayName => string.IsNullOrEmpty(displayName) ? modifierName : displayName;
        public string Description => description;
        public Sprite Icon => icon;
        public IReadOnlyList<ContributionSpecification> Contributions => contributions;

        public void Validate(ValidationReport report) {
            if (string.IsNullOrWhiteSpace(modifierName))
                report.Error("modifiers.modifier-name-empty", "the modifier has no name", "modifierName",
                        "Type the modifier's name; it identifies the modifier in tables and tooltips.");

            if (contributions == null || contributions.Count == 0) {
                report.Warning("modifiers.contributions-empty", "the modifier changes no stat", "contributions",
                        "Add a contribution, or delete the modifier if it is not needed.");

                return;
            }

            for (var i = 0; i < contributions.Count; i++) {
                var location = "contributions[" + i + "]";
                var contribution = contributions[i];

                if (contribution == null) {
                    report.Error("modifiers.contribution-null", "the contribution is empty", location,
                            "Pick a contribution type for this entry, or remove it.");
                } else if (!contribution.IsValid) {
                    report.Error("modifiers.contribution-invalid", InvalidReason(contribution), location,
                            "Fill in the missing parts of this contribution, or remove it.");
                }
            }
        }

        private static string InvalidReason(ContributionSpecification contribution) {
            var missingStat = false;
            var missingAmount = false;
            var invalidAmount = false;

            foreach (var (stat, _, amount) in contribution.StatContributions) {
                missingStat |= stat == null;
                missingAmount |= amount == null;
                invalidAmount |= amount != null && !amount.IsValid;
            }

            var reasons = new List<string>();

            if (missingStat)
                reasons.Add("a stat is missing");

            if (missingAmount)
                reasons.Add("an amount is missing");

            if (invalidAmount)
                reasons.Add("an amount is not valid");

            return reasons.Count > 0 ? "the contribution is not valid: " + string.Join(", ", reasons)
                    : "the contribution is not valid";
        }

        public RolledModifier Roll(System.Random rng, uint sourceId) {
            var baked = new List<BakedRoll>();

            for (var i = 0; i < contributions.Count; i++)
                contributions[i].Bake(rng, baked);

            return new RolledModifier { modifierHash = Hash, sourceId = sourceId, baked = baked.ToArray() };
        }
    }
}
