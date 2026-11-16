// Copyright 2026 Spellbound Studio Inc.

namespace Spellbound.Modifiers {
    public abstract class Composite : Node {
        protected readonly Node[] Children;

        protected Composite(params Node[] children) {
            Children = children;
        }
    }
}