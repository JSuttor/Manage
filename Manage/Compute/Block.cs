using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage.Compute
{
    class Block
    {
        Rectangle blockRect;
        double sizeMult;
        int biomeType = 0;
        Texture2D biomeTexture;
        Texture2D feature;
        bool isHovering = false;
        private MouseState previousMouse;
        private MouseState currentMouse;
        public event EventHandler Click;
        bool startPoint = false;
        Vector2 position;

        public Block(Texture2D water)
        {
            biomeTexture = water;
        }
        public void SetRect(int xPos, int yPos, int xSize, int ySize)
        {
            blockRect = new Rectangle(xPos, yPos, xSize, ySize);
            position = new Vector2(xPos, yPos);
        }
        public void SetStart()
        {
            startPoint = true;
        }
        public void SetBiome(int biomeType)
        {
            this.biomeType = biomeType;
        }
        public void SetFeature(Texture2D feature, double sizeMult)
        {
            this.feature = feature;
            this.sizeMult = sizeMult;
        }
        public void SetTexture(Texture2D biomeTexture)
        {
            this.biomeTexture = biomeTexture;
        }
        public bool getStart()
        {
            return startPoint;
        }
        public int GetBiomeType()
        {
            return biomeType;
        }
        public Texture2D GetFeature()
        {
            return feature;
        }
        public void DrawBlock(SpriteBatch spriteBatch)
        {
            previousMouse = currentMouse;
            currentMouse = Mouse.GetState();

            var mouseRectangle = new Rectangle(currentMouse.X, currentMouse.Y, 1, 1);
            isHovering = false;
            if (mouseRectangle.Intersects(blockRect))
            {
                isHovering = true;

                if (currentMouse.LeftButton == ButtonState.Released && previousMouse.LeftButton == ButtonState.Pressed)
                {
                    Click?.Invoke(this, new EventArgs()); ;
                }
            }

            var color = Color.White;
            if (isHovering)
                color = Color.LightGray;

            spriteBatch.Draw(biomeTexture, blockRect, color);
        }
        public void DrawFeature(int size, SpriteBatch spriteBatch)
        {
            if (feature != null)
            {
                int printSize = Convert.ToInt32(Convert.ToDouble(size) * sizeMult);
                spriteBatch.Draw(feature, new Rectangle(Convert.ToInt32(position.X) - printSize/2, Convert.ToInt32(position.Y) - printSize / 2, printSize, printSize), Color.White);
            }
        }

    }
}
