using Godot;
using Breakout.Game;

namespace Breakout.Components
{
    /// <summary>
    /// UIComponent — manages HUD display (score, lives labels).
    /// 
    /// Following Nystrom's Component pattern:
    /// - Component owns the UI rendering responsibility (HUD overlay)
    /// - Listens to GameStateComponent events and updates display
    /// - Responsibility: Display score and lives; react to state changes via events
    /// 
    /// This is a focused UI component for HUD labels only.
    /// </summary>
    public partial class UIComponent : CanvasLayer
    {
        #region UI Elements
        private Label scoreLabel;
        private Label livesLabel;
        private Label gameOverLabel;
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

        /// <summary>
        /// Called when game transitions to GameOver state.
        /// Displays game-over message in center of screen.
        /// </summary>
        public void ShowGameOverMessage()
        {
            gameOverLabel = new Label();
            gameOverLabel.Text = "GAME OVER\nPress ESC to exit";
            gameOverLabel.AddThemeFontSizeOverride("font_size", 64);
            gameOverLabel.HorizontalAlignment = HorizontalAlignment.Center;
            gameOverLabel.VerticalAlignment = VerticalAlignment.Center;
            gameOverLabel.Position = new Vector2(Config.ViewportWidth / 2 - 200, Config.ViewportHeight / 2 - 100);
            AddChild(gameOverLabel);

            GD.Print("Game Over message displayed");
        }
        #endregion
    }
}
