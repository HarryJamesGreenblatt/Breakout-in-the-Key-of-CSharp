using Breakout.Game;
using Breakout.Utilities;
using Godot;

namespace Breakout.Infrastructure
{
    /// <summary>
    /// Container for immobile boundary walls (left, right, top).
    /// Creates walls programmatically with collision and visual components.
    /// </summary>
    public partial class Walls : Node
    {
        #region Wall Definition
        /// <summary>
        /// A single wall with collision and visual representation rendered as a ColorRect.
        /// </summary>
        private partial class Wall : StaticBody2D
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Wall"/> class.
            /// </summary>
            /// <param name="name"></param>
            /// <param name="position"></param>
            /// <param name="size"></param>
            /// <param name="collisionOffset"></param>
            /// <param name="color"></param>
            public Wall(string name, Vector2 position, Vector2 size, Vector2 collisionOffset, Color color)
            {
                Name = name;
                Position = position;

                // Collision shape with explicit offset parameter
                var collisionShape = new CollisionShape2D();
                collisionShape.Position = collisionOffset;
                collisionShape.Shape = new RectangleShape2D { Size = size };
                AddChild(collisionShape);

                // Visual representation
                var visual = new ColorRect
                {
                    Position = Vector2.Zero,
                    Size = size,
                    Color = color
                };
                AddChild(visual);

                // Collision setup from config
                CollisionLayer = Config.Walls.CollisionLayer;
                CollisionMask = Config.Walls.CollisionMask;
            }
        }
        #endregion

        #region Game Behavior
        public override void _Ready()
        {
            // Create boundary walls - base color is whitesmoke
            var topWallSize = new Vector2(Config.ViewportWidth, Config.WallThickness);
            var verticalWallSize = new Vector2(Config.WallThickness, Config.ViewportHeight);
            var whitesmoke = new Color(0.96f, 0.96f, 0.96f, 1);  // whitesmoke

            // Top wall: whitesmoke (no brick row above it)
            var topWall = new Wall("TopWall", new Vector2(0, 0), topWallSize, topWallSize / 2, whitesmoke);
            AddChild(topWall);
            
            // Left wall: full height, whitesmoke base
            var leftWall = new Wall("LeftWall", new Vector2(0, 0), verticalWallSize, verticalWallSize / 2, whitesmoke);
            AddChild(leftWall);
            
            // Right wall: full height, whitesmoke base
            var rightWall = new Wall("RightWall", new Vector2(Config.ViewportWidth - Config.WallThickness, 0), verticalWallSize, verticalWallSize / 2, whitesmoke);
            AddChild(rightWall);

            // Create colored wall overlays for brick-aligned segments and paddle area
            CreateColoredWallOverlays();
        }

        private void CreateColoredWallOverlays()
        {
            float segmentHeight = Config.Brick.Size.Y + Config.BrickGrid.VerticalGap;
            float startY = Config.BrickGrid.GridStartPosition.Y;
            float wallThickness = Config.WallThickness;

            // Create overlays for brick-aligned segments
            for (int row = 0; row < Config.BrickGrid.GridRows; row++)
            {
                // Get the color for this row
                var brickColor = BrickColorUtility.GetColorForRow(row);
                var config = BrickColorUtility.GetConfig(brickColor);
                var segmentColor = config.VisualColor;

                // Calculate Y position for this segment
                float segmentY = startY + (row * segmentHeight);

                // Left wall overlay (matches brick row color)
                var leftOverlay = new ColorRect
                {
                    Position = new Vector2(0, segmentY),
                    Size = new Vector2(wallThickness, segmentHeight),
                    Color = segmentColor,
                    MouseFilter = Control.MouseFilterEnum.Ignore
                };
                AddChild(leftOverlay);

                // Right wall overlay (matches brick row color)
                var rightOverlay = new ColorRect
                {
                    Position = new Vector2(Config.ViewportWidth - wallThickness, segmentY),
                    Size = new Vector2(wallThickness, segmentHeight),
                    Color = segmentColor,
                    MouseFilter = Control.MouseFilterEnum.Ignore
                };
                AddChild(rightOverlay);
            }

            // Create paddle-aligned wall overlay at bottom
            float paddleY = Config.Paddle.Position.Y;
            float wallBottomY = Config.ViewportHeight;
            float paddleSegmentHeight = wallBottomY - paddleY;

            if (paddleSegmentHeight > 0)
            {
                var paddleColor = Config.Paddle.Color;  // Blue

                // Left wall paddle segment
                var leftPaddleOverlay = new ColorRect
                {
                    Position = new Vector2(0, paddleY),
                    Size = new Vector2(wallThickness, paddleSegmentHeight),
                    Color = paddleColor,
                    MouseFilter = Control.MouseFilterEnum.Ignore
                };
                AddChild(leftPaddleOverlay);

                // Right wall paddle segment
                var rightPaddleOverlay = new ColorRect
                {
                    Position = new Vector2(Config.ViewportWidth - wallThickness, paddleY),
                    Size = new Vector2(wallThickness, paddleSegmentHeight),
                    Color = paddleColor,
                    MouseFilter = Control.MouseFilterEnum.Ignore
                };
                AddChild(rightPaddleOverlay);
            }
        }
        #endregion
    }
}