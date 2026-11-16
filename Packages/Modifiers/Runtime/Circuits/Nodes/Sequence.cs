// Copyright 2026 Spellbound Studio Inc.

namespace Spellbound.Modifiers {
    public sealed class Sequence : Composite {
        public Sequence(params Node[] children) : base(children) { }

        public override void Process(CircuitContext ctx) {
            for (var i = 0; i < Children.Length; i++)
                Children[i].Process(ctx);
        }
    }
}