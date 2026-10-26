// Copyright (c) Spellbound Studios. All rights reserved.

using System.Reflection;
using NUnit.Framework;
using Spellbound.Modifiers.Editor;
using UnityEngine;

namespace Spellbound.Modifiers.Tests {
    public class DefinitionPreviewerTests {
        private static T Set<T>(T target, string field, object value) {
            typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(target, value);

            return target;
        }

        private static StatDefinition Stat(string statName, string displayName = null, string description = null) {
            var stat = ScriptableObject.CreateInstance<StatDefinition>();
            Set(stat, "statName", statName);
            Set(stat, "displayName", displayName);
            Set(stat, "description", description);

            return stat;
        }

        private static ModifierDefinition Modifier(
            string modifierName, params ContributionSpecification[] contributions) {
            return Set(Definitions.Create(contributions), "modifierName", modifierName);
        }

        [Test]
        public void ThePreviewOfAStatNamesItAndListsItsDescription() {
            var preview = new StatDefinitionPreviewer().Preview(Stat("Strength", "Might", "Hits harder."));

            Assert.AreEqual("Might (Strength)", preview.Summary);
            Assert.IsEmpty(preview.Pictures);
            Assert.AreEqual(new[] { "Hits harder." }, preview.Lines);
        }

        [Test]
        public void TheLineOfAStatWithoutADescriptionIsLeftOut() {
            var preview = new StatDefinitionPreviewer().Preview(Stat("Strength"));

            Assert.AreEqual("Strength (Strength)", preview.Summary);
            Assert.IsEmpty(preview.Lines);
        }

        [Test]
        public void ThePreviewOfAModifierHasOneLinePerContribution() {
            var strength = Stat("Strength");
            var modifier = Modifier(
                "Mighty",
                Definitions.Range(strength, ContributionType.Flat, 2, 2),
                Definitions.Range(strength, ContributionType.Increased, 1, 5),
                Definitions.Single(null, ContributionType.More, null));
            var preview = new ModifierDefinitionPreviewer().Preview(modifier);

            Assert.AreEqual("Mighty: 3 contributions", preview.Summary);
            Assert.IsEmpty(preview.Pictures);
            Assert.AreEqual(
                new[] { "Strength Flat 2", "Strength Increased 1 to 5", "no stat More no amount" }, preview.Lines);
        }

        [Test]
        public void ADerivedAmountNamesItsSourceStat() {
            var strength = Stat("Strength");
            var intelligence = Stat("Intelligence");
            var range = Definitions.Range(strength, ContributionType.Flat, 2, 2, 0f, intelligence);
            var modifier = Modifier("Scaling", range);
            var preview = new ModifierDefinitionPreviewer().Preview(modifier);

            Assert.AreEqual("Scaling: 1 contribution", preview.Summary);
            Assert.AreEqual(new[] { "Strength Flat 2 per 1 Intelligence" }, preview.Lines);
        }

        [Test]
        public void ANullContributionGetsItsOwnLine() {
            var modifier = Modifier("Holey", new ContributionSpecification[] { null });
            var preview = new ModifierDefinitionPreviewer().Preview(modifier);

            Assert.AreEqual(new[] { "contributions[0] is null" }, preview.Lines);
        }
    }
}
