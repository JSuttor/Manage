using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage.Compute
{
    public abstract class Building
    {
        public struct containedItemHolder
        {
            public Item item;
            public int count;
        }
        public struct neededItemHolder
        {
            public Item item;
            public int count;
        }
        public struct producedItemHolder
        {
            public Item item;
            public bool canUseCityPower;
            public int targetPerSpan;                                       //how many to make per day if possible
            public int productionPriority;                                  //if limeted materials and power, what get's produced first
        }
        public containedItemHolder[] containedItemList { get; set; }        //items stored in the factory
        public producedItemHolder[] producedItemList { get; set; }          //items that are being produced here.  will be of length maxConcurrent
        public int buildingLevel;                                           //buildings can be upgraded for more production, storage and concurrent assembly lines
        public int powerPriority;                                           //determnines what gets power in cases of shortage
        public int maxConcurrent;
        public int workCapacity;
        public Texture2D texture;

        Rectangle buildingRect;
        bool isHovering = false;
        private MouseState previousMouse;
        private MouseState currentMouse;
        public event EventHandler Click;
        public Vector2 position;

        public void IncreaseLevel()
        {
            buildingLevel++;
            maxConcurrent++;
        }

        public abstract void addContainedItem(Item item, int count);
        public abstract int subtractContainedItem(Item item, int count);
        public abstract bool addProduction(Item item, int targetPerSpan, int priority);
        public abstract void subtractProduction(Item item, int toSubtract);
        public abstract bool produce(int powerAccess);
        public abstract void removeProduction(Item item);

        public void DrawBuilding(int printSize, SpriteBatch spriteBatch)
        {
            previousMouse = currentMouse;
            currentMouse = Mouse.GetState();

            var mouseRectangle = new Rectangle(currentMouse.X, currentMouse.Y, 1, 1);
            isHovering = false;

            if (mouseRectangle.Intersects(buildingRect))
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

            spriteBatch.Draw(texture, new Rectangle(Convert.ToInt32(position.X) - printSize / 2, Convert.ToInt32(position.Y) - printSize / 2, printSize, printSize), color);
        }

    }
}
