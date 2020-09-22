using Manage.Compute;
using Manage.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Windows.Graphics.Imaging;
using Windows.UI.Input.Inking;

namespace Manage
{
    public class Game1 : Game
    {

        private GraphicsDeviceManager graphics;
        Player p1 = Player.Instance;
        private Color _backgroundColor = Color.CornflowerBlue;
        ButtonPanel bp;
        Textures textures = Textures.Instance;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        Map gameMap;

        protected override void Initialize()
        {
            textures.loadAssets(Content);
            p1.mapSprite = new SpriteBatch(GraphicsDevice);
            p1.spriteBatch = new SpriteBatch(GraphicsDevice);
            gameMap = Map.Instance;
            gameMap.initMap();
            base.Initialize();
            bp = new ButtonPanel(Content, GraphicsDevice);
            
        }

        protected override void LoadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            bp.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            bp.Draw(gameTime, p1.spriteBatch);
            base.Draw(gameTime);
        }
    }
}
