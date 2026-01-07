using Godot;
using Breakout.Components;
using Breakout.Utilities;
using Breakout.Infrastructure;

namespace Breakout.Game
{
    /// <summary>
    /// Game controller: pure signal wiring and coordination.
    /// Instantiation delegated to EntityFactory (separation of concerns).
    /// All business logic delegated to components (GameStateComponent, etc.).
    /// 
    /// Responsibility: ONLY wire signals between components and entities.
    /// Nothing else. Zero persistent state.
    /// 
    /// Following Nystrom's Component pattern:
    /// - EntityFactory creates and manages entities
    /// - Controller only wires signals
    /// - Components own all state and logic
    /// </summary>
    public partial class Controller : Node2D
    {
        #region Game Loop
        public override void _Ready()
        {
            // Instantiate EntityFactoryUtility (responsible for creating entity-component pairs)
            var entityFactory = new EntityFactoryUtility();

            // Instantiate components and entities
            var gameState = entityFactory.CreateGameState();
            var brickGrid = entityFactory.CreateBrickGrid(this);
            var paddle = entityFactory.CreatePaddle(this);
            var (ball, ballPhysics) = entityFactory.CreateBallWithPhysics(this);
            var walls = entityFactory.CreateWalls(this);  // For completeness; walls are stateless

            // Wire all signals directly to component behavior owners (zero indirection)
            gameState.SpeedIncreaseRequired += ballPhysics.ApplySpeedMultiplier;  // Wire to physics, not ball
            gameState.PaddleShrinkRequired += paddle.Shrink;
            brickGrid.BrickDestroyedWithColor += gameState.OnBrickDestroyed;

            // Connect ball signals to game state
            ball.BallHitPaddle += () => OnBallHitPaddle();
            ball.BallOutOfBounds += () => {
                OnBallOutOfBounds();
                gameState.LoseLive();
            };
            ball.BallHitCeiling += () => gameState.OnBallHitCeiling();
        }

        public override void _Process(double delta)
        {
            // Game loop (state transitions, rule checks, etc.)
            // All event-driven now; no polling
        }
        #endregion

        #region Signals
        private void OnBallHitPaddle()
        {
            GD.Print("Ball hit paddle!");
        }

        private void OnBallOutOfBounds()
        {
            GD.Print("Ball out of bounds!");
        }
        #endregion
    }
}
