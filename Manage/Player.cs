using Manage.Compute;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage
{
    public sealed class Player
    {
        private static Player instance = null;

        public static Player Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Player();
                }
                return instance;
            }
        }

        public GameState currentState;
        public enum GameState
        {
            menu,
            settings,
            newGame,
            mainGame
        }
        public string placing = "factory";

        public bool mapExists = false;
        public int mapSize;
        public Vector2 mapCenter = new Vector2(0, 0);
        public int zoomPct = 100;
        public SpriteBatch mapSprite;
        public SpriteBatch spriteBatch;
    }
}
