// Copyright 2026 Spellbound Studio Inc.

namespace Spellbound.Modifiers {
    public abstract class Decorator : Node {
        protected readonly Node Child;

        protected Decorator(Node child) {
            Child = child;
        }
    }
}