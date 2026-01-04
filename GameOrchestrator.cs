using Godot;
using Breakout.Entities;
using Breakout.Infrastructure;

namespace Breakout
{
    /// <summary>
    /// Game orchestrator: manages entities, signals, and game loop.
    /// Responsible for instantiation, signal binding, and overall game state.
    /// </summary>
    public partial class GameOrchestrator : Node2D
    {

        public override void _Ready()
        {
            // Instantiate entities with parameters
            var paddle = new Paddle(
                new Vector2(400, 550),
                new Vector2(100, 20),
                new Color(0, 1, 0, 1) // Green
            );
            AddChild(paddle);

            var ball = new Ball(
                new Vector2(400, 300),
                new Vector2(20, 20),
                new Vector2(200, -200),
                new Color(1, 1, 0, 1) // Yellow
            );
            AddChild(ball);

            // Instantiate infrastructure
            var walls = new Walls();
            AddChild(walls);

            // Connect signal listeners
            ball.BallHitPaddle += OnBallHitPaddle;
            ball.BallOutOfBounds += OnBallOutOfBounds;
        }

        public override void _Process(double delta)
        {
            // Main game loop (future: game state, scoring, etc.)
        }

        private void OnBallHitPaddle()
        {
            GD.Print("Ball hit paddle!");
        }

        private void OnBallOutOfBounds()
        {
            GD.Print("Ball out of bounds!");
        }
    }
}