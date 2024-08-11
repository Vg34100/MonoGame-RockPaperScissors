using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RockPaperScissors.Managers;

public class SettingsScreen
{
    private GameStateManager _gameStateManager;
    private Rectangle _volumeSliderRectangle;
    private Texture2D _sliderTexture;
    private Texture2D _knobTexture;
    private Rectangle _knobRectangle;
    private bool _isDraggingKnob;

    public SettingsScreen(GameStateManager gameStateManager)
    {
        _gameStateManager = gameStateManager;
        }

    public void LoadContent()
    {
        _sliderTexture = _gameStateManager.SliderTexture;
        _knobTexture = _gameStateManager.KnobTexture;
        _volumeSliderRectangle = new Rectangle(100, 100, 300, 20); // Example position and size
        _knobRectangle = new Rectangle(_volumeSliderRectangle.X + (int)(_volumeSliderRectangle.Width * _gameStateManager.SoundEffectVolume) - (_knobTexture.Width / 2), _volumeSliderRectangle.Y - (_knobTexture.Height / 2) + (_volumeSliderRectangle.Height / 2), _knobTexture.Width, _knobTexture.Height);

    }

    public void Update(MouseState currentMouseState, MouseState previousMouseState)
    {
        if (_isDraggingKnob)
        {
            if (currentMouseState.LeftButton == ButtonState.Released)
            {
                _isDraggingKnob = false;
            }
            else
            {
                int newKnobX = MathHelper.Clamp(currentMouseState.X, _volumeSliderRectangle.X, _volumeSliderRectangle.Right);
                _knobRectangle.X = newKnobX - (_knobRectangle.Width / 2);
                _gameStateManager.SetVolume((newKnobX - _volumeSliderRectangle.X) / (float)_volumeSliderRectangle.Width);
            }
        }
        else if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
        {
            if (_knobRectangle.Contains(currentMouseState.Position))
            {
                _isDraggingKnob = true;
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_sliderTexture, _volumeSliderRectangle, Color.White);
        spriteBatch.Draw(_knobTexture, _knobRectangle, Color.White);
    }
}
