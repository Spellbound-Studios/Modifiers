// Copyright (c) Spellbound Studios. All rights reserved.

using System.Reflection;
using NUnit.Framework;
using Spellbound.Core.Authoring;
using UnityEngine;

namespace Spellbound.Modifiers.Tests {
    public class ModifierDefinitionValidationTests {
        private static ModifierDefinition Named(string modifierName, params ContributionSpecification[] contributions) {
            var definition = Definitions.Create(contributions);
            typeof(ModifierDefinition).GetField("modifierName", BindingFlags.NonPublic | BindingFlags.Instance)
                    .SetValue(definition, modifierName);

            return definition;
        }

        private static StatDefinition Stat(string statName) {
            var stat = ScriptableObject.CreateInstance<StatDefinition>();
            typeof(StatDefinition).GetField("statName", BindingFlags.NonPublic | BindingFlags.Instance)
                    .SetValue(stat, statName);

            return stat;
        }

        private static ValidationReport Validate(ModifierDefinition definition) {
            var report = new ValidationReport("modifier");
            definition.Validate(report);

            return report;
        }

        [Test]
        public void AnEmptyNameIsAnErrorAtTheNameField() {
            var report = Validate(Named(string.Empty, Definitions.Range(Stat("Strength"), ContributionType.Flat, 1, 1)));

            Assert.AreEqual(1, report.Findings.Count);
            Assert.AreEqual("modifiers.modifier-name-empty", report.Findings[0].Code);
            Assert.AreEqual(FindingSeverity.Error, report.Findings[0].Severity);
            Assert.AreEqual("modifierName", report.Findings[0].Location);
        }

        [Test]
        public void NoContributionsIsAWarning() {
            var report = Validate(Named("Empty"));

            Assert.AreEqual(1, report.Findings.Count);
            Assert.AreEqual("modifiers.contributions-empty", report.Findings[0].Code);
            Assert.AreEqual(FindingSeverity.Warning, report.Findings[0].Severity);
            Assert.AreEqual("contributions", report.Findings[0].Location);
        }

        [Test]
        public void ANullContributionIsAnErrorAtItsIndex() {
            var report = Validate(Named("Holey", new ContributionSpecification[] { null }));

            Assert.AreEqual(1, report.Findings.Count);
            Assert.AreEqual("modifiers.contribution-null", report.Findings[0].Code);
            Assert.AreEqual(FindingSeverity.Error, report.Findings[0].Severity);
            Assert.AreEqual("contributions[0]", report.Findings[0].Location);
        }

        [Test]
        public void AContributionWithoutAStatIsInvalidAndSaysSo() {
            var valid = Definitions.Range(Stat("Strength"), ContributionType.Flat, 1, 1);
            var invalid = Definitions.Single(null, ContributionType.Flat, Definitions.Fixed(1f));
            var report = Validate(Named("Broken", valid, invalid));

            Assert.AreEqual(1, report.Findings.Count);
            Assert.AreEqual("modifiers.contribution-invalid", report.Findings[0].Code);
            Assert.AreEqual(FindingSeverity.Error, report.Findings[0].Severity);
            Assert.AreEqual("contributions[1]", report.Findings[0].Location);
            StringAssert.Contains("a stat is missing", report.Findings[0].Message);
        }

        [Test]
        public void AContributionWithoutAnAmountSaysSo() {
            var invalid = Definitions.Single(Stat("Strength"), ContributionType.Flat, null);
            var report = Validate(Named("Broken", invalid));

            StringAssert.Contains("an amount is missing", report.Findings[0].Message);
        }

        [Test]
        public void AFullyAuthoredModifierHasNoFindings() {
            var report = Validate(Named("Mighty", Definitions.Range(Stat("Strength"), ContributionType.Flat, 1, 5)));

            Assert.IsTrue(report.IsEmpty);
        }
    }
}
