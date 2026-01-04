using Godot;

namespace Breakout
{
    /// <summary>
    /// Centralized game configuration. All entity parameters defined here.
    /// Game controller uses this to instantiate entities without hardcoding values.
    /// </summary>
    public static class GameConfig
    {
        public static class Paddle
        {
            public static readonly Vector2 Position = new Vector2(400, 550);
            public static readonly Vector2 Size = new Vector2(100, 20);
            public static readonly Color Color = new Color(0, 1, 0, 1);
        }

        public static class Ball
        {
            public static readonly Vector2 Position = new Vector2(400, 300);
            public static readonly Vector2 Size = new Vector2(20, 20);
            public static readonly Vector2 Velocity = new Vector2(200, -200);
            public static readonly Color Color = new Color(1, 1, 0, 1);
        }
    }
}
