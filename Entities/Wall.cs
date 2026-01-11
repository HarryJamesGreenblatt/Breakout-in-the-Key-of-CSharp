using Breakout.Game;
using Godot;

namespace Breakout.Entities
{
    /// <summary>
    /// Wall entity — immobile boundary wall segment with collision and visual representation.
    /// 
    /// Individual wall segment that comprises part of the game boundary.
    /// Supports both collision geometry and visual rendering.
    /// </summary>
    public partial class Wall : Area2D
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Wall"/> class.
        /// </summary>
        /// <param name="name">Identifier for this wall segment (e.g., "TopWall", "LeftWall").</param>
        /// <param name="position">Position in world space.</param>
        /// <param name="size">Dimensions (width × height) of the wall.</param>
        /// <param name="collisionOffset">Offset for the collision shape relative to position.</param>
        /// <param name="color">Visual color of the wall.</param>
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
}
