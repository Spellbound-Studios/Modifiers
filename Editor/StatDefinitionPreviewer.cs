// Copyright (c) Spellbound Studios. All rights reserved.

using System.Collections.Generic;
using Spellbound.Core.Authoring.Editor;
using UnityEditor;

namespace Spellbound.Modifiers.Editor {
    public sealed class StatDefinitionPreviewer : DefinitionPreviewer<StatDefinition> {
        protected override DefinitionPreview Preview(StatDefinition definition) {
            var pictures = new List<string>();
            var lines = new List<string>();

            if (definition.Icon != null) {
                var path = AssetDatabase.GetAssetPath(definition.Icon);

                if (!string.IsNullOrEmpty(path))
                    pictures.Add(path);
            }

            if (!string.IsNullOrEmpty(definition.Description))
                lines.Add(definition.Description);

            return new DefinitionPreview(definition.DisplayName + " (" + definition.StatName + ")", pictures, lines);
        }
    }
}
