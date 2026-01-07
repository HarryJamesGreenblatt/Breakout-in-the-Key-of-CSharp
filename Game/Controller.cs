using Breakout.Components;
using Breakout.Utilities;
using Godot;

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
    /// 
    /// Objective 2.1 Update:
    /// - GameUIComponent wired to ScoreChanged/LivesChanged events
    /// - GameStateComponent now includes state machine (Playing/GameOver/LevelComplete)
    /// - Game-over detection triggers state transition within GameStateComponent
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

            // Instantiate UI component (manages HUD display of score and lives)
            var uiComponent = new UIComponent();
            AddChild(uiComponent);

            // Wire all signals directly to component behavior owners (zero indirection)
            gameState.SpeedIncreaseRequired += ballPhysics.ApplySpeedMultiplier;  // Wire to physics, not ball
            gameState.PaddleShrinkRequired += paddle.Shrink;
            
            // Wire brick destruction to BOTH game rules (speed/shrink) AND scoring
            brickGrid.BrickDestroyedWithColor += gameState.OnBrickDestroyed;    // Game rules (speed increases, paddle shrink)
            brickGrid.BrickDestroyedWithColor += gameState.AddScore;             // Scoring

            // Wire UI events
            gameState.ScoreChanged += uiComponent.OnScoreChanged;
            gameState.LivesChanged += uiComponent.OnLivesChanged;

            // Wire state machine (game-over detection)
            gameState.LivesChanged += (lives) => {
                if (lives <= 0)
                {
                    gameState.SetState(GameStateComponent.GameState.GameOver);
                }
            };

            // Wire GameOver event to halt gameplay and display message
            gameState.GameOver += () => {
                OnGameOver(ball, ballPhysics, paddle, uiComponent, gameState);
            };

            // Connect ball signals to game state
            ball.BallHitPaddle += () => OnBallHitPaddle();
            ball.BallOutOfBounds += () => {
                OnBallOutOfBounds();
                gameState.LoseLive();
            };
            ball.BallHitCeiling += () => gameState.OnBallHitCeiling();

            // Wire all-bricks-destroyed to level complete
            brickGrid.AllBricksDestroyed += () => {
                gameState.SetState(GameStateComponent.GameState.LevelComplete);
            };

            GD.Print("Controller initialized with Objective 2.1: Scoring & UI");
        }

        public override void _Process(double delta)
        {
            // Game loop (state transitions, rule checks, etc.)
            // All event-driven now; no polling
        }
        #endregion

        #region Signal Handlers
        /// <summary>
        /// Called when ball hits the paddle.
        /// </summary>
        private void OnBallHitPaddle()
        {
            GD.Print("Ball hit paddle!");
        }

        /// <summary>
        ///  Called when ball goes out of bounds (missed by paddle).
        /// </summary>
        private void OnBallOutOfBounds()
        {
            GD.Print("Ball out of bounds!");
        }

        /// <summary>
        /// Called when game transitions to GameOver state.
        /// Stops gameplay and displays game-over message.
        /// </summary>
        private void OnGameOver(Entities.Ball ball, PhysicsComponent ballPhysics, Entities.Paddle paddle, UIComponent uiComponent, GameStateComponent gameState)
        {
            GD.Print("GAME OVER!");

            // Stop ball movement
            ballPhysics.SetVelocity(Vector2.Zero);

            // Disable paddle input
            paddle.SetInputEnabled(false);

            // Display game over message in UI
            uiComponent.ShowGameOverMessage();
        }
        #endregion
    }
}
