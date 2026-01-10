using Godot;

namespace Breakout.Game
{
    /// <summary>
    /// Centralized game configuration. All magic numbers, constants, and tunable values here.
    /// 
    /// IMPORTANT: ViewportWidth and ViewportHeight are read dynamically from project.godot
    /// at initialization time. All other values (Paddle, Ball, Brick, BrickGrid) are computed
    /// based on these dimensions. This ensures that changing window/size settings in project.godot
    /// automatically cascades through the entire game configuration—no manual sync needed.
    /// 
    /// Initialization flow:
    /// 1. Static constructor reads actual window dimensions from ProjectSettings
    /// 2. Computes all derived values (positions, sizes, grid layout)
    /// 3. Game starts with correct proportions for the window size
    /// </summary>
    public static class Config
    {
        #region Viewport Dimensions (Synced from project.godot)
        /// <summary>
        /// Viewport width read from project.godot [display]/window/size/viewport_width.
        /// Dynamic: changes in project.godot are automatically reflected.
        /// </summary>
        public static float ViewportWidth { get; private set; }

        /// <summary>
        /// Viewport height read from project.godot [display]/window/size/viewport_height.
        /// Dynamic: changes in project.godot are automatically reflected.
        /// </summary>
        public static float ViewportHeight { get; private set; }

        /// <summary>
        /// Static constructor: reads actual window dimensions from ProjectSettings.
        /// Called once at app startup; all other Config values computed from these.
        /// </summary>
        static Config()
        {
            // Read from project.godot [display] section
            // ProjectSettings returns Variant; convert to float
            var widthSetting = ProjectSettings.GetSetting("display/window/size/viewport_width");
            var heightSetting = ProjectSettings.GetSetting("display/window/size/viewport_height");
            
            ViewportWidth = widthSetting.AsInt32();
            ViewportHeight = heightSetting.AsInt32();

            GD.Print($"Config initialized: Viewport {ViewportWidth}x{ViewportHeight}");
        }
        #endregion

        #region Infrastructure
        public const float WallThickness = 10f;
        public const float CeilingThickness = 20f;  // Ceiling is twice wall thickness

        public static class Walls
        {
            public static readonly Color Color = new Color(1, 0, 0, 1);  // Red (matches top brick row)
            public const int CollisionLayer = 1;
            public const int CollisionMask = 1;
        }
        #endregion

        #region Game Entities
        public static class Paddle
        {
            // Cache computed values to avoid re-computing on every access
            private static Vector2 cachedPosition;
            private static Vector2 cachedSize;
            private static float cachedSpeed;
            private static bool initialized = false;

            private static void EnsureInitialized()
            {
                if (initialized) return;
                cachedPosition = new Vector2(ViewportWidth / 2, ViewportHeight - 28);
                cachedSize = new Vector2(ViewportWidth * 0.117f, ViewportHeight * 0.027f);
                cachedSpeed = ViewportWidth * 0.586f;
                initialized = true;
                GD.Print($"Paddle config: pos={cachedPosition}, size={cachedSize}, speed={cachedSpeed}");
            }

            public static Vector2 Position { get { EnsureInitialized(); return cachedPosition; } }
            public static Vector2 Size { get { EnsureInitialized(); return cachedSize; } }
            public static readonly Color Color = new Color(0.4f, 0.8f, 1f, 1);  // Sky blue / robin's egg
            public static float Speed { get { EnsureInitialized(); return cachedSpeed; } }

            public static float MinX => 0;
            public static float MaxX { get { EnsureInitialized(); return ViewportWidth - cachedSize.X; } }

            public const int CollisionLayer = 1;
            public const int CollisionMask = 1;
        }

        public static class Ball
        {
            // Cache computed values
            private static Vector2 cachedPosition;
            private static Vector2 cachedSize;
            private static Vector2 cachedVelocity;
            private static float cachedBounceMarginX;
            private static float cachedBounceMarginTop;
            private static bool initialized = false;

            private static void EnsureInitialized()
            {
                if (initialized) return;
                cachedPosition = new Vector2(ViewportWidth / 2, ViewportHeight / 2);
                cachedSize = new Vector2(ViewportWidth * 0.0078f, ViewportWidth * 0.0078f);
                cachedVelocity = new Vector2(ViewportWidth * 0.234f, ViewportHeight * 0.268f);
                cachedBounceMarginX = cachedSize.X / 2;
                cachedBounceMarginTop = cachedSize.Y / 2;
                initialized = true;
                GD.Print($"Ball config: size={cachedSize}, velocity={cachedVelocity}");
            }

            public static Vector2 Position { get { EnsureInitialized(); return cachedPosition; } }
            public static Vector2 Size { get { EnsureInitialized(); return cachedSize; } }
            public static Vector2 Velocity { get { EnsureInitialized(); return cachedVelocity; } }
            public static readonly Color Color = new Color(1, 1, 1, 1);  // White

            public static float BounceMarginX { get { EnsureInitialized(); return cachedBounceMarginX; } }
            public static float BounceMarginTop { get { EnsureInitialized(); return cachedBounceMarginTop; } }
            public static float OutOfBoundsY => ViewportHeight;

            public const int CollisionLayer = 1;
            public const int CollisionMask = 1;
        }

        public static class Brick
        {
            // Cache computed size
            private static Vector2 cachedSize;
            private static bool initialized = false;

            private static void EnsureInitialized()
            {
                if (initialized) return;
                cachedSize = ComputeBrickSize();
                initialized = true;
                GD.Print($"Brick config: size={cachedSize}");
            }

            // Brick entity configuration: computed to fill viewport width
            public static Vector2 Size { get { EnsureInitialized(); return cachedSize; } }
            public const int CollisionLayer = 1;
            public const int CollisionMask = 1;

            /// <summary>
            /// Computes brick size to fill viewport width with small gaps between bricks.
            /// Formula: BrickWidth = (ViewportWidth - 2*margin - (GridColumns-1)*gap) / GridColumns
            /// Auto-scales with viewport width.
            /// </summary>
            private static Vector2 ComputeBrickSize()
            {
                float totalHorizontalGaps = (BrickGrid.GridColumns - 1) * BrickGrid.HorizontalGap;
                float availableWidth = ViewportWidth - totalHorizontalGaps;  // No margins, use full width
                float brickWidth = availableWidth / BrickGrid.GridColumns;
                float brickHeight = ViewportHeight * 0.018f;  // 1.8% of height
                return new Vector2(brickWidth, brickHeight);
            }
        }

        public static class BrickGrid
        {
            // Cache computed values
            private static Vector2 cachedGridStartPosition;
            private static float cachedGridSpacingX;
            private static float cachedGridSpacingY;
            private static bool initialized = false;

            private static void EnsureInitialized()
            {
                if (initialized) return;
                cachedGridStartPosition = new Vector2(0, ViewportHeight * 0.089f);  // Flush to left edge (no margin)
                cachedGridSpacingX = Brick.Size.X + HorizontalGap;
                cachedGridSpacingY = Brick.Size.Y + VerticalGap;  // Small vertical gap between rows
                initialized = true;
                GD.Print($"BrickGrid config: startPos={cachedGridStartPosition}, spacingX={cachedGridSpacingX}, spacingY={cachedGridSpacingY}");
            }

            // Grid infrastructure configuration
            public const int GridRows = 8;
            public const int GridColumns = 8;
            public const float HorizontalGap = 2f;  // Pixel gap between bricks horizontally
            public const float VerticalGap = 1f;    // Pixel gap between bricks vertically
            
            // Grid start position: left margin, below score display area
            public static Vector2 GridStartPosition { get { EnsureInitialized(); return cachedGridStartPosition; } }
            
            // Grid spacing: brick size + gap
            public static float GridSpacingX { get { EnsureInitialized(); return cachedGridSpacingX; } }
            public static float GridSpacingY { get { EnsureInitialized(); return cachedGridSpacingY; } }
        }
        #endregion
    }
}
