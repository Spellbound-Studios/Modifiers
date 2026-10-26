// Copyright (c) Spellbound Studios. All rights reserved.

using NUnit.Framework;
using Spellbound.Core.Authoring.Editor;
using UnityEngine;

namespace Spellbound.Modifiers.Tests {
    public class DefinitionValidationRunnerIntegrationTests {
        [Test]
        public void TheRunnerValidatesAModifierThroughItsOwnValidation() {
            var stat = ScriptableObject.CreateInstance<StatDefinition>();
            var definition = Definitions.Create(Definitions.Range(stat, ContributionType.Flat, 1, 1));
            var run = new DefinitionValidationRunner().ValidateAsset(definition);

            Assert.AreEqual(new[] { "ModifierDefinition (Modifiers)" }, run.OwnValidations);
            Assert.AreEqual("modifiers.modifier-name-empty", run.Report.Findings[0].Code);
        }
    }
}
