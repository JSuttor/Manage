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
        //VARIABLES
        private GraphicsDeviceManager graphics;
        SpriteBatch mapSprite;
        SpriteBatch spriteBatch;
        bool mapExists = false;
        int mapSize;
        GameState currentState;
        private Color _backgroundColor = Color.CornflowerBlue;
        private List<Component> menuScreenComponents;
        private List<Component> settingsScreenComponents;
        private List<Component> newGameScreenComponents;
        private List<Component> mainGameScreenComponents;

        enum GameState
        {
            menu,
            settings,
            newGame,
            mainGame
        }

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
            currentState = GameState.menu;
        }

        protected override void LoadContent()
        {
            var smallMapButton = new Button(Content.Load<Texture2D>("Controls/basicButton"), Content.Load<SpriteFont>("Fonts/Font"))
            {
                Position = new Vector2(100, 0),
                Width = 150,
                Height = 64,
                Text = "SMALL",
            };
            var mediumMapButton = new Button(Content.Load<Texture2D>("Controls/basicButton"), Content.Load<SpriteFont>("Fonts/Font"))
            {
                Position = new Vector2(300, 0),
                Width = 200,
                Height = 64,
                Text = "MEDIUM",
            };
            var largeMapButton = new Button(Content.Load<Texture2D>("Controls/basicButton"), Content.Load<SpriteFont>("Fonts/Font"))
            {
                Position = new Vector2(550, 0),
                Width = 150,
                Height = 64,
                Text = "LARGE",
            };
            var menuButton = new Button(Content.Load<Texture2D>("Controls/basicButton"), Content.Load<SpriteFont>("Fonts/Font"))
            {
                Position = new Vector2(750, 0),
                Width = 128,
                Height = 64,
                Text = "MENU",
            };
            var newGameButton = new Button(Content.Load<Texture2D>("Controls/basicButton"), Content.Load<SpriteFont>("Fonts/Font"))
            {
                Position = new Vector2(100, 0),
                Width = 128,
                Height = 64,
                Text = "NEW",
            };

            smallMapButton.Click += SmallMapButton_Click;
            mediumMapButton.Click += MediumMapButton_Click;
            largeMapButton.Click += LargeMapButton_Click;
            newGameButton.Click += NewGameButton_Click; ;
            menuButton.Click += MenuButton_Click;

            menuScreenComponents = new List<Component>()
            {
                newGameButton,
            };
            settingsScreenComponents = new List<Component>()
            {
            };
            newGameScreenComponents = new List<Component>()
            {
                smallMapButton,
                mediumMapButton,
                largeMapButton,
                menuButton,
            };
            mainGameScreenComponents = new List<Component>()
            {
            };
        }

        private void LargeMapButton_Click(object sender, System.EventArgs e)
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
        private void NewGameButton_Click(object sender, System.EventArgs e)
        {
            currentState = GameState.newGame;
        }
        private void MenuButton_Click(object sender, System.EventArgs e)
        {
            currentState = GameState.menu;
        }
        protected override void Update(GameTime gameTime)
        {
            switch (currentState)
            {
                case GameState.menu:
                    foreach (var component in menuScreenComponents)
                        component.Update(gameTime);
                    break;
                case GameState.settings:
                    foreach (var component in settingsScreenComponents)
                        component.Draw(gameTime, spriteBatch);
                    break;
                case GameState.newGame:
                    foreach (var component in newGameScreenComponents)
                        component.Update(gameTime);
                    break;
                case GameState.mainGame:
                    foreach (var component in mainGameScreenComponents)
                        component.Draw(gameTime, spriteBatch);
                    break;
            }

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            switch (currentState)
            {
                case GameState.menu:
                    DrawMenu(gameTime);
                    break;
                case GameState.settings:
                    DrawSettings(gameTime);
                    break;
                case GameState.newGame:
                    DrawNewGame(gameTime);
                    break;
                case GameState.mainGame:
                    DrawMainGame(gameTime);
                    break;
            }
            base.Draw(gameTime);
        }

        protected void DrawMenu(GameTime gameTime)
        {
            spriteBatch.Begin();
            foreach (var component in menuScreenComponents)
                component.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }
        protected void DrawSettings(GameTime gameTime)
        {
            spriteBatch.Begin();
            foreach (var component in settingsScreenComponents)
                component.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }
        protected void DrawNewGame(GameTime gameTime)
        {
            spriteBatch.Begin();
            foreach (var component in newGameScreenComponents)
                component.Draw(gameTime, spriteBatch);
            spriteBatch.End();

            if (mapExists)
                gameMap.displayMap(100, 100, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, mapSprite);
        }
        protected void DrawMainGame(GameTime gameTime)
        {
            spriteBatch.Begin();
            foreach (var component in mainGameScreenComponents)
                component.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }
    }
}
