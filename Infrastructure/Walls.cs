using Breakout.Game;
using Breakout.Entities;
using Breakout.Utilities;
using Godot;

namespace Breakout.Infrastructure
{
    /// <summary>
    /// Container for immobile boundary walls (left, right, top).
    /// Creates wall entities programmatically and manages colored visual overlays.
    /// </summary>
    public partial class Walls : Node
    {
        #region WallColorMarker Definition
        /// <summary>
        /// A visual-only color marker for wall segments that align with bricks or paddle.
        /// No collision geometry - purely visual overlay.
        /// </summary>
        private partial class WallColorMarker : Node2D
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="WallColorMarker"/> class.
            /// </summary>
            public WallColorMarker(Vector2 position, Vector2 size, Color color)
            {
                Position = position;

                // Visual representation only (no collision)
                var rect = new ColorRect
                {
                    Position = Vector2.Zero,
                    Size = size,
                    Color = color,
                    MouseFilter = Control.MouseFilterEnum.Ignore
                };
                AddChild(rect);
            }
        }
        #endregion

        #region Game Behavior
        public override void _Ready()
        {
            // Create boundary walls (Area2D for physics collisions only)
            var topWallSize = new Vector2(Config.ViewportWidth, Config.CeilingThickness);
            var verticalWallSize = new Vector2(Config.WallThickness, Config.ViewportHeight);
            var whitesmoke = new Color(0.96f, 0.96f, 0.96f, 1);  // whitesmoke

            // Top wall: whitesmoke (no brick row above it), double thickness
            var topWall = new Wall("TopWall", new Vector2(0, 0), topWallSize, topWallSize / 2, whitesmoke);
            AddChild(topWall);
            
            // Left wall: full height, whitesmoke base
            var leftWall = new Wall("LeftWall", new Vector2(0, 0), verticalWallSize, verticalWallSize / 2, whitesmoke);
            AddChild(leftWall);
            
            // Right wall: full height, whitesmoke base
            var rightWall = new Wall("RightWall", new Vector2(Config.ViewportWidth - Config.WallThickness, 0), verticalWallSize, verticalWallSize / 2, whitesmoke);
            AddChild(rightWall);

            // Create separate visual marker nodes for colored overlays
            CreateColoredWallMarkers();
        }

        private void CreateColoredWallMarkers()
        {
            float segmentHeight = Config.Brick.Size.Y + Config.BrickGrid.VerticalGap;
            float startY = Config.BrickGrid.GridStartPosition.Y;
            float wallThickness = Config.WallThickness;

            // Create visual markers for brick-aligned segments
            for (int row = 0; row < Config.BrickGrid.GridRows; row++)
            {
                // Get the color for this row
                var brickColor = BrickColorUtility.GetColorForRow(row);
                var config = BrickColorUtility.GetConfig(brickColor);
                var segmentColor = config.VisualColor;

                // Calculate Y position for this segment
                float segmentY = startY + (row * segmentHeight);

                // Left and right wall color markers
                var leftMarker = new WallColorMarker(
                    new Vector2(0, segmentY),
                    new Vector2(wallThickness, segmentHeight),
                    segmentColor
                );
                AddChild(leftMarker);

                var rightMarker = new WallColorMarker(
                    new Vector2(Config.ViewportWidth - wallThickness, segmentY),
                    new Vector2(wallThickness, segmentHeight),
                    segmentColor
                );
                AddChild(rightMarker);
            }

            // Create visual markers for paddle-aligned area
            float paddleY = Config.Paddle.Position.Y;
            float wallBottomY = Config.ViewportHeight;
            float paddleSegmentHeight = wallBottomY - paddleY;

            if (paddleSegmentHeight > 0)
            {
                var paddleColor = Config.Paddle.Color;  // Sky blue

                var leftPaddleMarker = new WallColorMarker(
                    new Vector2(0, paddleY),
                    new Vector2(wallThickness, paddleSegmentHeight),
                    paddleColor
                );
                AddChild(leftPaddleMarker);

                var rightPaddleMarker = new WallColorMarker(
                    new Vector2(Config.ViewportWidth - wallThickness, paddleY),
                    new Vector2(wallThickness, paddleSegmentHeight),
                    paddleColor
                );
                AddChild(rightPaddleMarker);
            }
        }
        #endregion
    }
}