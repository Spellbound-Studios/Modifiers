// Copyright 2026 Spellbound Studio Inc.

namespace Spellbound.Modifiers {
    public static class StatSettings {
        private static bool _locked;

        public static int Precision { get; private set; } = 10000;

        public static void SetDecimalPrecision(int decimalPlaces) {
            var precision = (int)System.Math.Pow(10, decimalPlaces);

            if (precision == Precision)
                return;

            if (_locked) {
                throw new System.InvalidOperationException(
                    $"Stat precision is locked at {Precision} because stat conversions have already run. " +
                    "Set it once at startup before any stats are created.");
            }

            Precision = precision;
        }

        public static int ToInternal(float value) {
            _locked = true;

            return (int)System.Math.Round(value * (double)Precision, System.MidpointRounding.AwayFromZero);
        }

        public static float ToExternal(int value) {
            _locked = true;

            return (float)value / Precision;
        }
    }
}