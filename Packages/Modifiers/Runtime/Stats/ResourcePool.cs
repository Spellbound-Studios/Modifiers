// Copyright 2026 Spellbound Studio Inc.

using System;
using UnityEngine;

namespace Spellbound.Modifiers {
    public sealed class ResourcePool : IDisposable {
        private readonly Modifiable _owner;
        private float _current;
        private bool _depletionArmed = true;
        private bool _subscribed;
        private Vector2 _lastFired = new(float.NaN, float.NaN);

        public event Action<ResourcePool> Changed;
        public event Action<ResourcePool> Depleted;

        public ResourcePool(Modifiable owner, StatId stat, float min) {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Stat = stat;
            Min = min;
            _current = ClampToRange(Max);
            _owner.Changed += HandleOwnerStatChanged;
            _subscribed = true;
        }

        public StatId Stat { get; }

        public float Min { get; }

        public float Current => ClampToRange(_current);

        public float Max => _owner.GetValue(Stat);

        public bool IsDepleted => Current <= Min;

        public void ResetToMax() {
            _current = ClampToRange(Max);
            _depletionArmed = true;
            NotifyChange();
            UpdateDepletion(Current);
        }

        public float SetCurrent(float value) {
            var before = Current;
            _current = ClampToRange(value);

            return FinishWrite(before);
        }

        public float Adjust(float delta) {
            var before = Current;
            _current = ClampToRange(Current + delta);

            return FinishWrite(before);
        }

        public float Drain(float amount) => -Adjust(-amount);

        public bool TryConsume(float amount) {
            if (Current - amount < Min)
                return false;

            Adjust(-amount);

            return true;
        }

        public void Broadcast() {
            _lastFired = new Vector2(float.NaN, float.NaN);
            NotifyChange();
        }

        public void Dispose() {
            if (!_subscribed)
                return;

            _owner.Changed -= HandleOwnerStatChanged;
            _subscribed = false;
        }

        private void HandleOwnerStatChanged(StatId changedStat) {
            if (changedStat.Hash != Stat.Hash)
                return;

            _current = ClampToRange(_current);
            NotifyChange();
            UpdateDepletion(Current);
        }

        private float ClampToRange(float value) => Mathf.Max(Min, Mathf.Min(value, Max));

        private void NotifyChange() {
            var snapshot = new Vector2(Current, Max);

            if (snapshot == _lastFired)
                return;

            _lastFired = snapshot;
            Changed?.Invoke(this);
        }

        private float FinishWrite(float before) {
            var after = Current;

            if (!Mathf.Approximately(after, before))
                NotifyChange();

            UpdateDepletion(after);

            return after - before;
        }

        private void UpdateDepletion(float value) {
            if (value > Min) {
                _depletionArmed = true;

                return;
            }

            if (!_depletionArmed)
                return;

            _depletionArmed = false;
            Depleted?.Invoke(this);
        }
    }
}
