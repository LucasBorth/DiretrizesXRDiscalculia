using System;
namespace MatematicaHXR
{
    // A single source of truth shared by both presentation conditions.
    [Serializable]
    public sealed class SumModel
    {
        public const int Total = 15;
        public int Step { get; private set; }
        public int Left => Total - Step;
        public int Right => Step;
        public bool Move(int delta)
        {
            int next = Math.Max(0, Math.Min(Total, Step + delta));
            if (next == Step) return false;
            Step = next;
            return true;
        }
        public void Reset() => Step = 0;
        public string Equation => $"{Left} + {Right} = {Total}";
    }
}
