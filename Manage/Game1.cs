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
        SpriteBatch mapSprite;
        SpriteBatch spriteBatch;
        bool mapExists = false;
        int mapSize = 2;

        private Color _backgroundColor = Color.CornflowerBlue;
        private List<Component> gameComponents;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        Map gameMap;

        protected override void Initialize()
        {
            mapSprite = new SpriteBatch(GraphicsDevice);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            gameMap = new Map(Content);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            var smallMapButton = new Button(Content.Load<Texture2D>("Controls/refresh"), Content.Load<SpriteFont>("Fonts/Font"))
            {
                Position = new Vector2(45, 0),
                Width = 20,
                Height = 20,
                Text = "S",
            };
            var mediumMapButton = new Button(Content.Load<Texture2D>("Controls/refresh"), Content.Load<SpriteFont>("Fonts/Font"))
            {
                Position = new Vector2(70, 0),
                Width = 20,
                Height = 20,
                Text = "M",
            };
            var largeMapButtonButton = new Button(Content.Load<Texture2D>("Controls/refresh"), Content.Load<SpriteFont>("Fonts/Font"))
            {
                Position = new Vector2(95, 0),
                Width = 20,
                Height = 20,
                Text = "L",
            };

            smallMapButton.Click += SmallMapButton_Click;
            mediumMapButton.Click += MediumMapButton_Click;
            largeMapButtonButton.Click += LargeMapButtonButton_Click;

            gameComponents = new List<Component>()
            {
                smallMapButton,
                mediumMapButton,
                largeMapButtonButton,
            };
        }

        private void LargeMapButtonButton_Click(object sender, System.EventArgs e)
        {
            mapSize = 3;
            mapExists = true;
            gameMap.generateMap(mapSize);
        }

        private void MediumMapButton_Click(object sender, System.EventArgs e)
        {
            mapSize = 2;
            mapExists = true;
            gameMap.generateMap(mapSize);
        }

        private void SmallMapButton_Click(object sender, System.EventArgs e)
        {
            mapSize = 1;
            mapExists = true;
            gameMap.generateMap(mapSize);
        }

        protected override void Update(GameTime gameTime)
        {
            foreach (var component in gameComponents)
                component.Update(gameTime);
            base.Update(gameTime);
        }
        
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            foreach (var component in gameComponents)
                component.Draw(gameTime, spriteBatch);
            spriteBatch.End();

            if(mapExists)
                gameMap.displayMap(20, 20, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, mapSprite);
            base.Draw(gameTime);
        }
    }
}
