using Godot;

namespace Breakout.Infrastructure
{
    /// <summary>
    /// Container for immobile boundary walls (left, right, top).
    /// Creates walls programmatically with collision and visual components.
    /// </summary>
    public partial class Walls : Node
    {
        private partial class Wall : StaticBody2D
        {
            public Wall(string name, Vector2 position, Vector2 size, Color color)
            {
                Name = name;
                Position = position;

                // Collision shape
                var collisionShape = new CollisionShape2D();
                collisionShape.Shape = new RectangleShape2D { Size = size };
                AddChild(collisionShape);

                // Visual representation
                var visual = new ColorRect
                {
                    Size = size,
                    Color = color
                };
                AddChild(visual);
            }
        }

        public override void _Ready()
        {
            // Create boundary walls
            var color = new Color(0.5f, 0.5f, 0.5f, 1); // Gray

            var leftWall = new Wall("LeftWall", new Vector2(0, 300), new Vector2(20, 600), color);
            var rightWall = new Wall("RightWall", new Vector2(800, 300), new Vector2(20, 600), color);
            var topWall = new Wall("TopWall", new Vector2(400, 0), new Vector2(800, 20), color);

            AddChild(leftWall);
            AddChild(rightWall);
            AddChild(topWall);
        }
    }
}