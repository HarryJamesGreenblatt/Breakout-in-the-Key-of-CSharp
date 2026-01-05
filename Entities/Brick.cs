using Breakout.Game;
using Godot;

namespace Breakout.Entities
{
    /// <summary>
    /// Brick entity that can be destroyed by ball collision.
    /// Uses signals to notify the orchestrator when hit.
    /// </summary>
    public partial class Brick : Area2D
    {
        #region Signals
        /// <summary>
        /// Triggered when the brick is destroyed.
        /// Passes the brick's unique ID for grid management.
        /// </summary>
        [Signal]
        public delegate void BrickDestroyedEventHandler(int brickId);
        #endregion

        #region Properties
        private int brickId;
        private int health = 1;  // Number of hits to destroy
        private Vector2 size;    // Store size for collision detection

        public int BrickId => brickId;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a brick at the given position with the given size and color.
        /// </summary>
        /// <param name="id">Unique identifier for this brick in the grid</param>
        /// <param name="position">Top-left corner position</param>
        /// <param name="size">Width and height of the brick</param>
        /// <param name="color">Visual color of the brick</param>
        public Brick(int id, Vector2 position, Vector2 size, Color color)
        {
            brickId = id;
            this.size = size;
            Name = $"Brick_{id}";
            Position = position;

            // Collision shape offset to center of the visual rect
            var collisionShape = new CollisionShape2D();
            collisionShape.Position = size / 2;
            collisionShape.Shape = new RectangleShape2D { Size = size };
            AddChild(collisionShape);

            // Visual representation
            var visual = new ColorRect
            {
                Size = size,
                Color = color
            };
            AddChild(visual);

            // Collision setup from config
            CollisionLayer = Config.Brick.CollisionLayer;
            CollisionMask = Config.Brick.CollisionMask;
        }
        #endregion

        #region Game Behavior
        /// <summary>
        /// Sets up the brick by connecting collision signals.
        /// </summary>
        public override void _Ready()
        {
            AreaEntered += _OnAreaEntered;
        }

        /// <summary>
        /// Returns the brick's size for collision calculations.
        /// </summary>
        public Vector2 GetBrickSize()
        {
            return size;
        }

        /// <summary>
        /// Handles collision with the ball.
        /// </summary>
        private void _OnAreaEntered(Area2D area)
        {
            if (area is Ball)
            {
                health--;
                if (health <= 0)
                {
                    EmitSignal(SignalName.BrickDestroyed, brickId);
                    QueueFree();  // Schedule for deletion at end of frame
                }
            }
        }
        #endregion
    }
}
