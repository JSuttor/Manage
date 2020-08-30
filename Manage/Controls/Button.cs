using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage.Controls
{
    public class Button : Component
    {
        private MouseState currentMouse;
        private SpriteFont font;
        private bool isHovering;
        private MouseState previousMouse;
        private Texture2D buttonTexture;

        public event EventHandler Click;

        public bool Clicked { get; private set; }
        public Color FontColor { get; set; }
        public Vector2 Position { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Rectangle Rectangle
        {
            get
            {
                if(Width == 0)
                {
                    Width = buttonTexture.Width;
                }
                if (Height == 0)
                {
                    Height = buttonTexture.Height;
                }
                return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            }
        }

        public string Text { get; set; }

        public Button(Texture2D texture, SpriteFont font)
        {
            buttonTexture = texture;
            this.font = font;
            FontColor = Color.White;
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var color = Color.White;
            if (isHovering)
                color = Color.LightGray;

            spriteBatch.Draw(buttonTexture, Rectangle, color);

            if (!string.IsNullOrEmpty(Text))
            {
                
                var x = (Rectangle.X + ((Rectangle.Width / 2)) - (font.MeasureString(Text).X / 2));
                var y = (Rectangle.Y + ((Rectangle.Height / 2)) - (font.MeasureString(Text).Y / 2));
                spriteBatch.DrawString(font, Text, new Vector2(x, y), FontColor);
            }
        }

        public override void Update(GameTime gameTime)
        {
            previousMouse = currentMouse;
            currentMouse = Mouse.GetState();

            var mouseRectangle = new Rectangle(currentMouse.X, currentMouse.Y, 1, 1);
            isHovering = false;
            if(mouseRectangle.Intersects(Rectangle))
            {
                isHovering = true;

                if(currentMouse.LeftButton == ButtonState.Released && previousMouse.LeftButton == ButtonState.Pressed)
                {
                    Click?.Invoke(this, new EventArgs());;
                }
            }
        }
    }
}
