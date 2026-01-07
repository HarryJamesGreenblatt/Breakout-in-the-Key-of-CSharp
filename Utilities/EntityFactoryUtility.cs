using Godot;
using Breakout.Entities;
using Breakout.Infrastructure;
using Breakout.Components;
using Breakout.Game;

namespace Breakout.Utilities
{
    /// <summary>
    /// EntityFactoryUtility — factory utility for instantiating and composing entity-component pairs.
    /// 
    /// Factory utility responsible for:
    /// - Creating entity instances with consistent configuration
    /// - Attaching infrastructure components (BrickGrid)
    /// - Composing entities with their corresponding behavior components
    /// - Managing scene tree instantiation
    /// 
    /// Encapsulates creation logic for:
    /// - Ball entity with PhysicsComponent
    /// - Paddle entity
    /// - Brick grid infrastructure with BrickGrid
    /// - Walls infrastructure
    /// - Game state component
    /// 
    /// This ensures all entity-component pairs are created consistently and the Controller
    /// remains pure signal wiring.
    /// </summary>
    public class EntityFactoryUtility
    {
        #region Factory Methods

        /// <summary>
        /// Create the paddle entity and add to scene tree.
        /// </summary>
        /// <param name="parent">Parent node to attach paddle to.</param>
        /// <returns>Instantiated Paddle entity.</returns>
        public Paddle CreatePaddle(Node parent)
        {
            var paddle = new Paddle(
                Config.Paddle.Position,
                Config.Paddle.Size,
                Config.Paddle.Color
            );
            parent.AddChild(paddle);
            return paddle;
        }

        /// <summary>
        /// Create the walls infrastructure and add to scene tree.
        /// </summary>
        /// <param name="parent">Parent node to attach walls to.</param>
        /// <returns>Instantiated Walls.</returns>
        public Walls CreateWalls(Node parent)
        {
            var walls = new Walls();
            parent.AddChild(walls);
            return walls;
        }

        /// <summary>
        /// Create the brick grid infrastructure component and instantiate all bricks in the scene.
        /// </summary>
        /// <param name="parent">Parent node to attach bricks to.</param>
        /// <returns>Instantiated BrickGrid.</returns>
        public BrickGrid CreateBrickGrid(Node parent)
        {
            var brickGrid = new BrickGrid();
            brickGrid.InstantiateGrid(parent);
            return brickGrid;
        }

        /// <summary>
        /// Create the ball entity and get its physics component.
        /// Returns both the entity (for signals) and component (for behavior wiring).
        /// </summary>
        /// <param name="parent">Parent node to attach ball to.</param>
        /// <returns>Tuple of (Ball entity, PhysicsComponent).</returns>
        public (Ball entity, PhysicsComponent physics) CreateBallWithPhysics(Node parent)
        {
            var ball = new Ball(
                Config.Ball.Position,
                Config.Ball.Size,
                Config.Ball.Velocity,
                Config.Ball.Color
            );
            parent.AddChild(ball);
            return (ball, ball.GetPhysicsComponent());
        }

        /// <summary>
        /// Create the game state component (owns all game state and rules).
        /// </summary>
        /// <returns>Instantiated GameStateComponent.</returns>
        public GameStateComponent CreateGameState()
        {
            return new GameStateComponent();
        }

        #endregion
    }
}
