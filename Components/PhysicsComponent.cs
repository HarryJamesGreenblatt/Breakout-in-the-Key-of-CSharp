using Godot;
using Breakout.Game;
using System.Collections.Generic;

namespace Breakout.Components
{
    /// <summary>
    /// PhysicsComponent encapsulates ball physics simulation.
    /// Follows Nystrom's Component pattern: owns all physics state and behavior.
    /// 
    /// Responsibilities:
    /// - Velocity state management
    /// - Position updates based on velocity
    /// - Collision detection and bounce calculations
    /// - Collision state tracking (active contacts)
    /// - Speed modifiers for game rules
    /// 
    /// This component is owned by Ball entity; Ball delegates physics to it.
    /// Component signals important events (collisions, out of bounds).
    /// </summary>
    public class PhysicsComponent
    {
        #region Events
        /// <summary>
        /// C# event emitted when ball bounces. Allows Ball to respond (e.g., emit BallHitPaddle signal).
        /// Parameters: bounceType ("paddle", "brick", etc)
        /// </summary>
        public event System.Action<string> BounceOccurred;

        /// <summary>
        /// C# event emitted when ball goes out of bounds. Allows Ball to reset and emit BallOutOfBounds signal.
        /// </summary>
        public event System.Action OutOfBounds;

        /// <summary>
        /// C# event emitted when ball hits ceiling (upper wall).
        /// Allows game rules to respond (e.g., paddle shrink after red row contact).
        /// </summary>
        public event System.Action CeilingHit;
        #endregion

        #region State
        /// <summary>
        /// Current velocity (direction and magnitude per frame).
        /// </summary>
        private Vector2 velocity;

        /// <summary>
        /// Initial velocity (used for reset).
        /// </summary>
        private Vector2 initialVelocity;

        /// <summary>
        /// Cumulative speed multiplier (persists across ball resets).
        /// Starts at 1.0, multiplied by speed increase factors.
        /// When ball resets, velocity = initialVelocity * currentSpeedMultiplier.
        /// This ensures speed increases are persistent and don't reset.
        /// </summary>
        private float currentSpeedMultiplier = 1.0f;

        /// <summary>
        /// Ball size (needed for radius calculations in bounce logic).
        /// </summary>
        private Vector2 ballSize;

        /// <summary>
        /// Current position of the ball.
        /// </summary>
        private Vector2 position;

        /// <summary>
        /// Initial position for reset.
        /// </summary>
        private Vector2 initialPosition;

        /// <summary>
        /// Track active collisions to prevent bouncing multiple times in same contact.
        /// Uses signal-based state: collision tracked when area_entered, removed when area_exited.
        /// </summary>
        private HashSet<Node> activeCollisions = new();
        #endregion

        #region Constructor
        public PhysicsComponent(Vector2 initialPosition, Vector2 ballSize, Vector2 initialVelocity)
        {
            this.position = initialPosition;
            this.initialPosition = initialPosition;
            this.ballSize = ballSize;
            this.velocity = initialVelocity;
            this.initialVelocity = initialVelocity;
        }
        #endregion

        #region Public API - Position & Velocity
        /// <summary>
        /// Gets the current position.
        /// </summary>
        public Vector2 GetPosition() => position;

        /// <summary>
        /// Gets the current velocity.
        /// </summary>
        public Vector2 GetVelocity() => velocity;

        /// <summary>
        /// Sets velocity (used by game rules, e.g., speed multipliers).
        /// </summary>
        public void SetVelocity(Vector2 newVelocity) => velocity = newVelocity;
        #endregion

        #region Public API - Update & Physics
        /// <summary>
        /// Updates position based on current velocity.
        /// Call this from Ball._Process(delta).
        /// </summary>
        /// <param name="delta">Delta time</param>
        /// <returns>New position after velocity is applied</returns>
        public Vector2 Update(float delta)
        {
            // Update position
            position += velocity * delta;

            // Handle wall collisions
            HandleWallBounceX();
            HandleWallBounceY();

            // Check out of bounds
            if (position.Y > Config.Ball.OutOfBoundsY)
            {
                OutOfBounds?.Invoke();
                // NOTE: ResetPhysics() is now called by Ball after OutOfBounds event
                // This allows Controller to check game state before resetting
            }

            return position;
        }

        /// <summary>
        /// Resets position and velocity to initial values.
        /// Preserves cumulative speed multiplier so speed increases persist across ball resets.
        /// velocity = initialVelocity * currentSpeedMultiplier
        /// </summary>
        public void ResetPhysics()
        {
            position = initialPosition;
            velocity = initialVelocity * currentSpeedMultiplier;
            activeCollisions.Clear();
            GD.Print($"Ball reset. Speed multiplier: {currentSpeedMultiplier}x");
        }
        #endregion

        #region Public API - Collision Handling
        /// <summary>
        /// Handles collision enter event. Call from Ball._OnAreaEntered().
        /// </summary>
        public void HandleCollisionEnter(Area2D area)
        {
            // Only process if this is a NEW contact (not already being tracked)
            if (activeCollisions.Contains(area)) return;
            activeCollisions.Add(area);

            if (area is Entities.Paddle paddle)
            {
                HandlePaddleCollision(paddle);
                BounceOccurred?.Invoke("paddle");
            }
            else if (area is Entities.Brick brick)
            {
                HandleBrickCollision(brick);
                BounceOccurred?.Invoke("brick");
            }
        }

        /// <summary>
        /// Handles collision exit event. Call from Ball._OnAreaExited().
        /// </summary>
        public void HandleCollisionExit(Area2D area)
        {
            activeCollisions.Remove(area);
        }
        #endregion

        #region Private - Wall Collisions
        private void HandleWallBounceX()
        {
            float ballRadius = ballSize.X / 2;

            // Bounce off left wall (inner edge at x=0)
            if (position.X + ballRadius < 0)
            {
                velocity.X = -velocity.X;
            }
            // Bounce off right wall (inner edge at x=ViewportWidth)
            else if (position.X + ballRadius > Config.ViewportWidth)
            {
                velocity.X = -velocity.X;
            }
        }

        private void HandleWallBounceY()
        {
            float ballRadius = ballSize.Y / 2;

            // Bounce off ceiling (inner edge at y=0)
            if (position.Y + ballRadius < 0)
            {
                velocity.Y = -velocity.Y;
                CeilingHit?.Invoke();  // Emit event for game rules
            }
        }
        #endregion

        #region Private - Paddle Collision
        private void HandlePaddleCollision(Entities.Paddle paddle)
        {
            // Calculate contact point on paddle
            Vector2 ballCenter = position + ballSize / 2;
            Vector2 paddleCenter = paddle.Position + paddle.GetSize() / 2;
            Vector2 paddleSize = paddle.GetSize();

            // Delta between ball and paddle centers (normalized to -1...+1)
            Vector2 delta = ballCenter - paddleCenter;
            float normalizedX = delta.X / (paddleSize.X / 2);
            normalizedX = Mathf.Clamp(normalizedX, -1f, 1f);

            // Direct velocity manipulation for stronger steering effect
            float speedMagnitude = velocity.Length();

            // Ensure upward bounce: always make Y negative (upward in Godot)
            velocity.Y = -Mathf.Abs(velocity.Y);

            // Impart horizontal steering based on paddle contact point
            const float maxSteerFactor = 0.6f;
            velocity.X = speedMagnitude * maxSteerFactor * normalizedX;

            // NOTE: Do NOT apply speed boost here. Speed increases follow canonical ruleset:
            // - After 4 brick hits (4 hit milestone)
            // - After 12 brick hits (12 hit milestone)
            // - After ball contacts orange or red rows
            // These are managed by Orchestrator, not by physics engine.

            GD.Print($"Paddle bounce: contact={normalizedX:F2}, vel={velocity}");
        }
        #endregion

        #region Private - Brick Collision
        private void HandleBrickCollision(Entities.Brick brick)
        {
            float ballRadius = ballSize.X / 2;

            // Calculate penetration on each axis
            Vector2 ballCenter = position + ballSize / 2;
            Vector2 brickCenter = brick.Position + brick.GetBrickSize() / 2;
            Vector2 brickSize = brick.GetBrickSize();

            Vector2 delta = ballCenter - brickCenter;
            float overlapLeft = (brickSize.X / 2) + ballRadius + delta.X;
            float overlapRight = (brickSize.X / 2) + ballRadius - delta.X;
            float overlapTop = (brickSize.Y / 2) + ballRadius + delta.Y;
            float overlapBottom = (brickSize.Y / 2) + ballRadius - delta.Y;

            // Find the smallest overlap to determine which edge was hit
            float minOverlap = Mathf.Min(
                Mathf.Min(overlapLeft, overlapRight),
                Mathf.Min(overlapTop, overlapBottom)
            );

            if (minOverlap == overlapTop || minOverlap == overlapBottom)
            {
                // Hit top or bottom edge
                velocity.Y = -velocity.Y;
                GD.Print("Bounce off brick (vertical)");
            }
            else
            {
                // Hit left or right edge
                velocity.X = -velocity.X;
                GD.Print("Bounce off brick (horizontal)");
            }

            // Destroy the brick (emits signal for game rules)
            brick.Destroy();
        }
        #endregion

        #region Public API - Speed Modifiers
        /// <summary>
        /// Applies a speed multiplier to the current velocity.
        /// Used by game rules (e.g., speed increases after N hits).
        /// </summary>
        public void ApplySpeedMultiplier(float factor)
        {
            currentSpeedMultiplier *= factor;
            velocity = initialVelocity * currentSpeedMultiplier;
            GD.Print($"Speed multiplier applied: {factor}x, cumulative: {currentSpeedMultiplier}x, new velocity={velocity}");
        }
        #endregion
    }
}

