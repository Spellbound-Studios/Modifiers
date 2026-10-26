// Copyright (c) Spellbound Studios. All rights reserved.

using System.Collections.Generic;
using System.Globalization;
using Spellbound.Core.Authoring.Editor;
using UnityEditor;

namespace Spellbound.Modifiers.Editor {
    public sealed class ModifierDefinitionPreviewer : DefinitionPreviewer<ModifierDefinition> {
        protected override DefinitionPreview Preview(ModifierDefinition definition) {
            var pictures = new List<string>();
            var lines = new List<string>();

            if (definition.Icon != null) {
                var path = AssetDatabase.GetAssetPath(definition.Icon);

                if (!string.IsNullOrEmpty(path))
                    pictures.Add(path);
            }

            var contributions = definition.Contributions;
            var count = contributions?.Count ?? 0;

            for (var i = 0; i < count; i++) {
                if (contributions[i] == null) {
                    lines.Add("contributions[" + i + "] is null");

                    continue;
                }

                foreach (var (stat, type, amount) in contributions[i].StatContributions)
                    lines.Add((stat != null ? stat.StatName : "no stat") + " " + type + " " + AmountText(amount));
            }

            var summary = definition.DisplayName + ": " + count + (count == 1 ? " contribution" : " contributions");

            return new DefinitionPreview(summary, pictures, lines);
        }

        private static string AmountText(Magnitude amount) {
            switch (amount) {
                case null:
                    return "no amount";
                case FixedMagnitude fixedMagnitude:
                    return Number(fixedMagnitude.Value(0f));
                case RolledMagnitude rolled:
                    return Number(rolled.Min) + " to " + Number(rolled.Max);
                case DerivedMagnitude derived:
                    return AmountText(derived.Amount) + " per " + derived.PerPoints + " "
                            + (derived.Source != null ? derived.Source.StatName : "no stat");
                default:
                    var name = amount.GetType().Name;

                    return name.EndsWith("Magnitude") ? name.Substring(0, name.Length - "Magnitude".Length) : name;
            }
        }

        private static string Number(float value) => value.ToString("0.##", CultureInfo.InvariantCulture);
    }
}
