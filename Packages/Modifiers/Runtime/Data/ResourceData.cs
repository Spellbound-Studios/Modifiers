// Copyright 2026 Spellbound Studio Inc.

using System;
using Spellbound.Core.Packing;

namespace Spellbound.Modifiers {
    public struct ResourceData : IPacker {
        public uint statHash;
        public float max;
        public float min;
        public float current;

        public ResourceData(uint statHash, float max, float min, float current) {
            this.statHash = statHash;
            this.max = max;
            this.min = min;
            this.current = current;
        }

        public ResourceData(uint statHash, float max) {
            this.statHash = statHash;
            this.max = max;
            min = 0;
            current = max;
        }

        public void Pack(ref Span<byte> buffer) {
            Packer.WriteUInt(ref buffer, statHash);
            Packer.WriteFloat(ref buffer, max);
            Packer.WriteFloat(ref buffer, min);
            Packer.WriteFloat(ref buffer, current);
        }

        public void Unpack(ref ReadOnlySpan<byte> buffer) {
            statHash = Packer.ReadUInt(ref buffer);
            max = Packer.ReadFloat(ref buffer);
            min = Packer.ReadFloat(ref buffer);
            current = Packer.ReadFloat(ref buffer);
        }

        public override string ToString() =>
                StatRegistry.TryGetName(
                    statHash, out var n)
                        ? $"{n} max: {max}=min: {min} current: {current}"
                        : $"#{statHash} max: {max}=min: {min} current: {current}";
    }
}