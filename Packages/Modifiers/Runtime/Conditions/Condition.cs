// Copyright 2026 Spellbound Studio Inc.

namespace Spellbound.Modifiers {
    public abstract class Condition {
        public abstract bool Met(CircuitContext ctx);
    }
}