// Copyright 2026 Spellbound Studio Inc.

using System;
using System.Collections.Generic;
using Spellbound.Core.Registries;
using UnityEngine;

namespace Spellbound.Modifiers {
    public static class StatRegistry {
        private const string ResourceFolder = "Stats";

        private static readonly HashRegistry<StatDefinition> Registry = new();
        private static readonly Dictionary<string, StatDefinition> NameIndex = new();
        private static bool _isLoaded;

        public static IReadOnlyList<StatDefinition> All {
            get {
                EnsureLoaded();

                return Registry.All;
            }
        }

        #region Lifecycle

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetForPlaySession() {
            Registry.Clear();
            NameIndex.Clear();
            _isLoaded = false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void WarmUp() => EnsureLoaded();

        #endregion

        #region API

        public static uint GetHash(string statName) {
            EnsureLoaded();

            if (!NameIndex.TryGetValue(statName, out var definition)) {
                throw new KeyNotFoundException(
                    $"Stat '{statName}' is not registered. Author a StatDefinition for it under " +
                    $"Resources/{ResourceFolder}.");
            }

            return definition.Hash;
        }

        public static bool TryGetHash(string statName, out uint hash) {
            EnsureLoaded();

            if (NameIndex.TryGetValue(statName, out var definition)) {
                hash = definition.Hash;

                return true;
            }

            hash = 0u;

            return false;
        }

        public static bool IsRegistered(string statName) {
            EnsureLoaded();

            return NameIndex.ContainsKey(statName);
        }

        public static StatDefinition GetDefinition(uint statHash) {
            EnsureLoaded();

            return Registry.TryGet(statHash, out var def) ? def : null;
        }

        public static StatDefinition GetDefinition(string statName) {
            EnsureLoaded();

            return NameIndex.TryGetValue(statName, out var definition) ? definition : null;
        }

        public static string GetName(uint statHash) => GetDefinition(statHash)?.StatName;

        public static bool TryGetName(uint statHash, out string statName) {
            statName = GetName(statHash);

            return statName != null;
        }

        #endregion

        #region Internal

        private static void EnsureLoaded() {
            if (_isLoaded)
                return;

            try {
                foreach (var definition in Resources.LoadAll<StatDefinition>(ResourceFolder)) {
                    if (Registry.Contains(definition.Hash)) {
                        throw new InvalidOperationException(
                            $"Stat hash collision: '{definition.StatName}' (asset '{definition.name}') collides " +
                            $"with an already-registered stat at hash {definition.Hash}. Regenerate one asset's " +
                            "GUID to resolve.");
                    }

                    if (!NameIndex.TryAdd(definition.StatName, definition)) {
                        throw new InvalidOperationException(
                            $"Duplicate stat name: '{definition.StatName}' (asset '{definition.name}') is already " +
                            "registered by another StatDefinition. Stat names must be unique. Rename one.");
                    }

                    Registry.Add(definition);
                }
            }
            catch {
                Registry.Clear();
                NameIndex.Clear();

                throw;
            }

            _isLoaded = true;
        }

        #endregion
    }
}