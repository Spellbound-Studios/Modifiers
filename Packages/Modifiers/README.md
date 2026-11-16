# Spellbound.Modifiers

A Unity library for games where everything can be modified. An entity exposes stats; anything in the game (an item, a talent, a buff, the level it spawned in) contributes to those stats under a removable source id, and a single query resolves the truth with Path-of-Exile-grade algebra (flat, increased, more, override). Modifiers are authored once as rollable definitions, rolled into packable facts that survive saving and networking, and applied anywhere: a "Thick Hide" can be an item affix, a racial perk, a five-second buff, or a level-wide mutation without changing a line of code. Entities chain (gear modifies the player, skills read through the caster), timed modifiers expire against a clock you own, and event circuits let content restructure behavior, like damage mitigation, at runtime. All math runs on deterministic fixed-point integers behind a float-facing API, steady-state allocation-free.

## Installation

Requires Unity 6000.3 or newer and Spellbound.Core 1.1.9 or newer and below 3.0.0. `StatData`, the stat block an instance carries, lives in the `Spellbound.Modifiers.WorldObjects` assembly, which compiles only when Spellbound.WorldObjects is installed beside it. UPM cannot resolve git-to-git dependencies, so Core, and WorldObjects when you want `StatData`, must be added to the project alongside this package. Add them to `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.spellboundstudios.core": "https://github.com/Spellbound-Studios/Core.git?path=/Packages/Core#dev",
    "com.spellboundstudios.worldobjects": "https://github.com/Spellbound-Studios/WorldObjects.git?path=/Packages/WorldObjects#beta",
    "com.spellboundstudios.modifiers": "https://github.com/Spellbound-Studios/Modifiers.git?path=/Packages/Modifiers#main"
  }
}
```

Or via **Window → Package Manager → + → Add package from git URL** using the same URLs. A playable combat demo is the Combat Demo sample, imported from the package's Samples tab in the Package Manager, and `CHANGELOG.md` tracks the API.
