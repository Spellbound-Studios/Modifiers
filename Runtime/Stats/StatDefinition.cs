// Copyright 2026 Spellbound Studio Inc.

using Spellbound.Core.Authoring;
using Spellbound.Core.Registries;
using UnityEngine;

namespace Spellbound.Modifiers {
    [CreateAssetMenu(menuName = "Spellbound/ModifierLib/Stat Definition")]
    public class StatDefinition : HashedScriptableObject, IAuthoredDefinition {
        [Header("Identity"), SerializeField] private string statName;
        [SerializeField] private string displayName;
        [SerializeField, TextArea] private string description;
        [SerializeField, SpritePreview] private Sprite icon;

        public string StatName => statName;
        public string DisplayName => string.IsNullOrEmpty(displayName) ? statName : displayName;
        public string Description => description;
        public Sprite Icon => icon;

        public void Validate(ValidationReport report) {
            if (string.IsNullOrWhiteSpace(statName))
                report.Error("modifiers.stat-name-empty", "the stat has no name", "statName",
                        "Type the stat's name; it is the key other packages roll and read.");
        }
    }
}