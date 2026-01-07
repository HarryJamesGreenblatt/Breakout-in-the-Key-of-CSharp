using Godot;
using Breakout.Game;

namespace Breakout.Components
{
    /// <summary>
    /// RenderingComponent — manages all rendering concerns for the game.
    /// 
    /// Following Nystrom's Component pattern:
    /// - Component owns the rendering responsibility (visual presentation layer)
    /// - Listens to GameStateComponent events and updates display
    /// - Currently: Renders score/lives UI labels
    /// - Future: Will handle entity rendering, menus, particle effects, animations
    /// 
    /// This abstracts rendering as a cohesive concern, separate from game logic.
    /// Not just UI — it's the rendering system for all game presentation.
    /// </summary>
    public partial class RenderingComponent : CanvasLayer
    {
        #region UI Elements
        private Label scoreLabel;
        private Label livesLabel;
        #endregion

        #region Lifecycle
        public override void _Ready()
        {
            // Create score label (top-left)
            scoreLabel = new Label();
            scoreLabel.Position = new Vector2(10, 10);
            scoreLabel.Text = "Score: 0";
            scoreLabel.AddThemeFontSizeOverride("font_size", 32);
            AddChild(scoreLabel);

            // Create lives label (top-right)
            livesLabel = new Label();
            livesLabel.Position = new Vector2(Config.ViewportWidth - 200, 10);
            livesLabel.Text = "Lives: 3";
            livesLabel.AddThemeFontSizeOverride("font_size", 32);
            AddChild(livesLabel);

            GD.Print("GameUIComponent initialized");
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Called when score changes.
        /// Updates score label display.
        /// </summary>
        public void OnScoreChanged(int newScore)
        {
            scoreLabel.Text = $"Score: {newScore}";
        }

        /// <summary>
        /// Called when lives change.
        /// Updates lives label display.
        /// </summary>
        public void OnLivesChanged(int newLives)
        {
            livesLabel.Text = $"Lives: {newLives}";
        }
        #endregion
    }
}
