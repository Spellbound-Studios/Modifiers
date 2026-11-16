// Copyright 2026 Spellbound Studio Inc.

using System.Collections.Generic;
using NUnit.Framework;

namespace Spellbound.Modifiers.Tests {
    public class ResourcePoolTests {
        private static readonly StatId Health = new(11u);
        private static readonly StatId Armor = new(12u);

        private static Modifiable OwnerWithHealth(float maximum) {
            var owner = new Modifiable();
            owner.Stats.SetBase(Health, maximum);

            return owner;
        }

        [Test]
        public void Max_ReadsTheOwnersValueOfTheStat() {
            var pool = new ResourcePool(OwnerWithHealth(100f), Health, 0f);

            Assert.AreEqual(Health, pool.Stat);
            Assert.AreEqual(0f, pool.Min);
            Assert.AreEqual(100f, pool.Max);
        }

        [Test]
        public void SetCurrent_AboveMax_ClampsToMax() {
            var pool = new ResourcePool(OwnerWithHealth(100f), Health, 0f);

            pool.SetCurrent(250f);

            Assert.AreEqual(100f, pool.Current);
        }

        [Test]
        public void SetCurrent_BelowMin_ClampsToMin() {
            var pool = new ResourcePool(OwnerWithHealth(100f), Health, 10f);

            pool.SetCurrent(-40f);

            Assert.AreEqual(10f, pool.Current);
            Assert.IsTrue(pool.IsDepleted);
        }

        [Test]
        public void Adjust_ClampsBothWaysAndAnswersTheAppliedChange() {
            var pool = new ResourcePool(OwnerWithHealth(100f), Health, 0f);
            pool.ResetToMax();

            Assert.AreEqual(-100f, pool.Adjust(-500f));
            Assert.AreEqual(0f, pool.Current);
            Assert.AreEqual(100f, pool.Adjust(500f));
            Assert.AreEqual(100f, pool.Current);
        }

        [Test]
        public void ResetToMax_FillsThePool() {
            var pool = new ResourcePool(OwnerWithHealth(80f), Health, 0f);

            pool.ResetToMax();

            Assert.AreEqual(80f, pool.Current);
            Assert.IsFalse(pool.IsDepleted);
        }

        [Test]
        public void Depleted_FiresOncePerCrossing() {
            var pool = new ResourcePool(OwnerWithHealth(100f), Health, 0f);
            pool.ResetToMax();
            var depletions = 0;
            pool.Depleted += _ => depletions++;

            pool.Adjust(-100f);
            pool.Adjust(-10f);
            pool.SetCurrent(0f);

            Assert.AreEqual(1, depletions);

            pool.Adjust(5f);
            pool.Adjust(-5f);

            Assert.AreEqual(2, depletions);
        }

        [Test]
        public void Depleted_PassesThePool() {
            var pool = new ResourcePool(OwnerWithHealth(100f), Health, 0f);
            ResourcePool reported = null;
            pool.Depleted += depleted => reported = depleted;

            pool.SetCurrent(0f);

            Assert.AreSame(pool, reported);
        }

        [Test]
        public void Changed_PassesThePoolOnEveryEffectiveWrite() {
            var pool = new ResourcePool(OwnerWithHealth(100f), Health, 0f);
            pool.ResetToMax();
            var reported = new List<ResourcePool>();
            pool.Changed += reported.Add;

            pool.Adjust(-30f);
            pool.Adjust(0f);
            pool.SetCurrent(70f);

            Assert.AreEqual(1, reported.Count);
            Assert.AreSame(pool, reported[0]);
        }

        [Test]
        public void MaxChange_ReclampsTheCurrentValue() {
            var owner = OwnerWithHealth(100f);
            var pool = new ResourcePool(owner, Health, 0f);
            pool.ResetToMax();
            var changes = 0;
            pool.Changed += _ => changes++;

            owner.Stats.SetBase(Health, 40f);

            Assert.AreEqual(40f, pool.Current);
            Assert.AreEqual(1, changes);

            owner.Stats.SetBase(Health, 100f);

            Assert.AreEqual(40f, pool.Current);
            Assert.AreEqual(100f, pool.Max);
        }

        [Test]
        public void MaxChange_DownToMin_DepletesThePool() {
            var owner = OwnerWithHealth(100f);
            var pool = new ResourcePool(owner, Health, 0f);
            pool.ResetToMax();
            var depletions = 0;
            pool.Depleted += _ => depletions++;

            owner.Stats.SetBase(Health, 0f);

            Assert.IsTrue(pool.IsDepleted);
            Assert.AreEqual(1, depletions);
        }

        [Test]
        public void AnotherStatsChange_LeavesThePoolAlone() {
            var owner = OwnerWithHealth(100f);
            var pool = new ResourcePool(owner, Health, 0f);
            pool.ResetToMax();
            var changes = 0;
            pool.Changed += _ => changes++;

            owner.Stats.SetBase(Armor, 25f);

            Assert.AreEqual(0, changes);
            Assert.AreEqual(100f, pool.Current);
        }

        [Test]
        public void Dispose_StopsTheSubscription() {
            var owner = OwnerWithHealth(100f);
            var pool = new ResourcePool(owner, Health, 0f);
            pool.ResetToMax();
            var changes = 0;
            pool.Changed += _ => changes++;

            pool.Dispose();
            owner.Stats.SetBase(Health, 40f);
            owner.Stats.SetBase(Health, 100f);

            Assert.AreEqual(0, changes);
            Assert.AreEqual(100f, pool.Current);
        }

        [Test]
        public void TryConsume_RefusesWhatThePoolCannotPay() {
            var pool = new ResourcePool(OwnerWithHealth(100f), Health, 0f);
            pool.ResetToMax();

            Assert.IsTrue(pool.TryConsume(60f));
            Assert.IsFalse(pool.TryConsume(60f));
            Assert.AreEqual(40f, pool.Current);
            Assert.AreEqual(40f, pool.Drain(70f));
            Assert.AreEqual(0f, pool.Current);
        }

        [Test]
        public void MaxBelowMin_CurrentReadsMin() {
            var owner = OwnerWithHealth(100f);
            var pool = new ResourcePool(owner, Health, 10f);
            pool.ResetToMax();

            owner.Stats.SetBase(Health, 4f);

            Assert.AreEqual(10f, pool.Current);
            Assert.IsTrue(pool.IsDepleted);
        }

        [Test]
        public void NewPool_ReadsMax() {
            var pool = new ResourcePool(OwnerWithHealth(80f), Health, 0f);

            Assert.AreEqual(80f, pool.Current);
            Assert.IsFalse(pool.IsDepleted);
        }

        [Test]
        public void ResetToMax_WithMaxBelowMin_ReadsMinAndDepletesOnce() {
            var pool = new ResourcePool(OwnerWithHealth(5f), Health, 10f);
            var depletions = 0;
            pool.Depleted += _ => depletions++;

            pool.ResetToMax();

            Assert.AreEqual(10f, pool.Current);
            Assert.AreEqual(1, depletions);
        }

        [Test]
        public void ResetToMax_WithMaxEqualToMin_ReadsMinAndDepletesOnce() {
            var pool = new ResourcePool(OwnerWithHealth(10f), Health, 10f);
            var depletions = 0;
            pool.Depleted += _ => depletions++;

            pool.ResetToMax();

            Assert.AreEqual(10f, pool.Current);
            Assert.AreEqual(1, depletions);
        }
    }
}
