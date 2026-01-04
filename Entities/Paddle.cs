using Godot;


namespace Breakout.Entities
{
    /// <summary>
    /// Paddle entity controlled by player input (left/right arrow keys).
    /// Parametrized for position, size, and color.
    /// </summary>
    public partial class Paddle : Area2D
    {

        #region Physics Properties
        private Vector2 speed = new Vector2(600, 0);
        private float minX = 50;
        private float maxX = 750;
        #endregion

        /// <summary>
        ///  Default constructor for Paddle entity.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        #region Constructor
        public Paddle(Vector2 position, Vector2 size, Color color)
        {
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
        #endregion

        public override void _Ready()
        {
            // Lifecycle setup (if needed in future)
        }

        /// <summary>
        /// Update paddle position based on player input each frame.
        /// </summary>
        /// <param name="delta"></param>
        public override void _Process(double delta)
        {
            // Handle input
            var input = Input.GetAxis("ui_left", "ui_right");
            
            // Update position
            Position += speed * input * (float)delta;
            
            // Constrain to bounds
            Position = new Vector2(
                Mathf.Clamp(Position.X, minX, maxX),
                Position.Y
            );
        }
    }
}
