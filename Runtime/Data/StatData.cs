// Copyright 2026 Spellbound Studio Inc.

using System;
using System.Collections.Generic;
using System.Text;
using Spellbound.Core.Packing;
using Spellbound.WorldObjects;

namespace Spellbound.Modifiers {
    [PackerId("stat_data")]
    public struct StatData : IInstanceData {
        public List<ResourceData> resourceData;
        public List<RolledModifier> modifiers;
        public List<TimedModifier> buffs;
        public List<TimedModifier> debuffs;

        #region Hydration

        public readonly void ApplyTo(
            Modifiable target, TimedModifierSet buffSet = null, TimedModifierSet debuffSet = null) {
            if (modifiers != null) {
                for (var i = 0; i < modifiers.Count; i++)
                    modifiers[i].TryApplyTo(target);
            }

            if (buffSet != null && buffs != null) {
                for (var i = 0; i < buffs.Count; i++)
                    buffSet.Restore(buffs[i]);
            }

            if (debuffSet != null && debuffs != null) {
                for (var i = 0; i < debuffs.Count; i++)
                    debuffSet.Restore(debuffs[i]);
            }
        }

        public static StatData Capture(
            List<ResourceData> resources, List<RolledModifier> applied,
            TimedModifierSet buffSet = null, TimedModifierSet debuffSet = null) =>
                new() {
                    resourceData = resources,
                    modifiers = applied != null ? new List<RolledModifier>(applied) : null,
                    buffs = Snapshot(buffSet),
                    debuffs = Snapshot(debuffSet)
                };

        private static List<TimedModifier> Snapshot(TimedModifierSet set) {
            if (set == null || set.Active.Count == 0)
                return null;

            return new List<TimedModifier>(set.Active);
        }

        #endregion Hydration

        #region IInstanceData

        public IInstanceData Empty() => new StatData();

        public void RaiseChanged(in InstanceView view, IInstancePresenter presenter) =>
                this.RaiseChangedTo(in view, presenter);

        public void RaiseResolve(InstanceWriter writer, in InstanceView view) =>
                this.RaiseResolveTo(writer, in view);

        #endregion IInstanceData

        #region IPacker

        public void Pack(ref Span<byte> buffer) {
            Packer.PackList(ref buffer, resourceData);
            Packer.PackList(ref buffer, modifiers);
            Packer.PackList(ref buffer, buffs);
            Packer.PackList(ref buffer, debuffs);
        }

        public void Unpack(ref ReadOnlySpan<byte> buffer) {
            resourceData = Packer.UnpackList<ResourceData>(ref buffer);
            modifiers = Packer.UnpackList<RolledModifier>(ref buffer);
            buffs = Packer.UnpackList<TimedModifier>(ref buffer);
            debuffs = Packer.UnpackList<TimedModifier>(ref buffer);
        }

        #endregion IPacker

        #region IRegistryEntry

        public uint Hash => SmartPackerRegistry.GetHash(GetType());

        #endregion IRegistryEntry

        #region ISmartPacker

        public ISmartPacker CreateNewInstance() => new StatData();

        #endregion ISmartPacker

        #region ToString

        public override string ToString() {
            var sb = new StringBuilder(64);

            sb.Append("StatData [resources: ").Append(ResourceCount)
                    .Append(", modifiers: ").Append(ModifierCount)
                    .Append(", buffs: ").Append(BuffCount)
                    .Append(", debuffs: ").Append(DebuffCount)
                    .Append(", packed: ").Append(PackedSize).Append(" B]");

            for (var i = 0; i < ResourceCount; i++)
                sb.Append("\n    res[").Append(i).Append("] ").Append(resourceData[i]);

            for (var i = 0; i < ModifierCount; i++)
                sb.Append("\n    mod[").Append(i).Append("] ").Append(modifiers[i]);

            for (var i = 0; i < BuffCount; i++)
                sb.Append("\n    buff[").Append(i).Append("] ").Append(buffs[i]);

            for (var i = 0; i < DebuffCount; i++)
                sb.Append("\n    debuff[").Append(i).Append("] ").Append(debuffs[i]);

            return sb.ToString();
        }

        #endregion ToString

        #region Queries

        private const int CountPrefixBytes = sizeof(int);
        private const int ResourceBytes = sizeof(uint) + 3 * sizeof(float);

        public int ResourceCount => resourceData?.Count ?? 0;

        public int ModifierCount => modifiers?.Count ?? 0;

        public int BuffCount => buffs?.Count ?? 0;

        public int DebuffCount => debuffs?.Count ?? 0;

        public int Count => ResourceCount + ModifierCount + BuffCount + DebuffCount;

        public bool IsEmpty => Count == 0;

        public int PackedSize {
            get {
                var size = 4 * CountPrefixBytes + ResourceCount * ResourceBytes;

                for (var i = 0; i < ModifierCount; i++)
                    size += modifiers[i].PackedSize;

                for (var i = 0; i < BuffCount; i++)
                    size += buffs[i].PackedSize;

                for (var i = 0; i < DebuffCount; i++)
                    size += debuffs[i].PackedSize;

                return size;
            }
        }

        #endregion Queries
    }
}