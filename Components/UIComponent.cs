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
            // Create score label (top-left, arcade-style: small, minimal)
            scoreLabel = new Label();
            scoreLabel.Position = new Vector2(5, 5);
            scoreLabel.Text = "SCORE: 0";
            scoreLabel.AddThemeFontSizeOverride("font_size", 14);
            scoreLabel.AddThemeColorOverride("font_color", new Color(1, 1, 1, 1));  // White text
            AddChild(scoreLabel);

            // Create lives label (top-right, arcade-style: small, minimal)
            livesLabel = new Label();
            livesLabel.Position = new Vector2(Config.ViewportWidth - 80, 5);
            livesLabel.Text = "LIVES: 3";
            livesLabel.AddThemeFontSizeOverride("font_size", 14);
            livesLabel.AddThemeColorOverride("font_color", new Color(1, 1, 1, 1));  // White text
            AddChild(livesLabel);

            GD.Print("UIComponent initialized (arcade-style)");
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Called when score changes.
        /// Updates score label display.
        /// </summary>
        public void OnScoreChanged(int newScore)
        {
            scoreLabel.Text = $"SCORE: {newScore}";
        }

        /// <summary>
        /// Called when lives change.
        /// Updates lives label display.
        /// </summary>
        public void OnLivesChanged(int newLives)
        {
            livesLabel.Text = $"LIVES: {newLives}";
        }

        /// <summary>
        /// Called when game transitions to GameOver state.
        /// Displays game-over message in center of screen.
        /// </summary>
        public void ShowGameOverMessage()
        {
            gameOverLabel = new Label();
            gameOverLabel.Text = "GAME OVER";
            gameOverLabel.AddThemeFontSizeOverride("font_size", 32);
            gameOverLabel.AddThemeColorOverride("font_color", new Color(1, 1, 1, 1));  // White
            gameOverLabel.HorizontalAlignment = HorizontalAlignment.Center;
            gameOverLabel.VerticalAlignment = VerticalAlignment.Center;
            
            // Set size before positioning so we can calculate center correctly
            gameOverLabel.CustomMinimumSize = new Vector2(300, 150);
            
            // Center the label by positioning it so its center is at viewport center
            // Position is top-left corner, so we subtract half the size to center it
            float centerX = Config.ViewportWidth / 2 - gameOverLabel.CustomMinimumSize.X / 2;
            float centerY = Config.ViewportHeight / 2 - gameOverLabel.CustomMinimumSize.Y / 2;
            gameOverLabel.Position = new Vector2(centerX, centerY);
            
            AddChild(gameOverLabel);

            GD.Print("Game Over message displayed");
        }
        #endregion
    }
}
