using NUnit.Framework;
using Spellbound.Core.Authoring;
using UnityEngine;

namespace Spellbound.Modifiers.Tests {
    public class StatDefinitionValidationTests {
        private static StatDefinition StatNamed(string statName) {
            var definition = ScriptableObject.CreateInstance<StatDefinition>();
            Definitions.SetField(definition, "statName", statName);

            return definition;
        }

        [Test]
        public void AnEmptyNameIsOneErrorAtTheNameField() {
            var report = new ValidationReport("stat");
            StatNamed(string.Empty).Validate(report);

            Assert.AreEqual(1, report.Findings.Count);
            Assert.AreEqual("modifiers.stat-name-empty", report.Findings[0].Code);
            Assert.AreEqual(FindingSeverity.Error, report.Findings[0].Severity);
            Assert.AreEqual("statName", report.Findings[0].Location);
        }

        [Test]
        public void ABlankNameIsTheSameError() {
            var report = new ValidationReport("stat");
            StatNamed("   ").Validate(report);

            Assert.AreEqual("modifiers.stat-name-empty", report.Findings[0].Code);
        }

        [Test]
        public void ANamedStatHasNoFindings() {
            var report = new ValidationReport("stat");
            StatNamed("Strength").Validate(report);

            Assert.IsTrue(report.IsEmpty);
        }
    }
}
