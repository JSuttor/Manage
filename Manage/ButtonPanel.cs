using Manage.Compute;
using Manage.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.ContentRestrictions;

namespace Manage
{
    class ButtonPanel : Component
    {
        ContentManager Content;
        private List<Component> menuScreenComponents;
        private List<Component> settingsScreenComponents;
        private List<Component> newGameScreenComponents;
        private List<Component> mainGameScreenComponents;
        Player p1 = Player.Instance;
        Player.GameState currentState;
        Map gameMap = Map.Instance;
        //Factory f;
        GraphicsDevice GraphicsDevice;
        public ButtonPanel(ContentManager content, GraphicsDevice graphicsDevice)
        {
            this.Content = content;
            this.GraphicsDevice = graphicsDevice;
            init();
        }

        void init()
        {
            currentState = Player.GameState.menu;
            //f = new Factory(1);

            var testButton = new Button(Content.Load<Texture2D>("Controls/basicButton"), Content.Load<SpriteFont>("Fonts/Font"))
            {
                Position = new Vector2(100, 0),
                Width = 128,
                Height = 64,
                Text = "TEST",
            };

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
            var zoomInButton = new Button(Content.Load<Texture2D>("Controls/basicButton"), Content.Load<SpriteFont>("Fonts/Font"))
            {
                Position = new Vector2(1268, 0),
                Width = 32,
                Height = 32,
                Text = "+",
            };
            var zoomOutButton = new Button(Content.Load<Texture2D>("Controls/basicButton"), Content.Load<SpriteFont>("Fonts/Font"))
            {
                Position = new Vector2(1332, 0),
                Width = 32,
                Height = 32,
                Text = "-",
            };
            var upButton = new Button(Content.Load<Texture2D>("Controls/basicButton"), Content.Load<SpriteFont>("Fonts/Font"))
            {
                Position = new Vector2(1300, 0),
                Width = 32,
                Height = 32,
                Text = "^",
            };
            var leftButton = new Button(Content.Load<Texture2D>("Controls/basicButton"), Content.Load<SpriteFont>("Fonts/Font"))
            {
                Position = new Vector2(1268, 32),
                Width = 32,
                Height = 32,
                Text = "<",
            };
            var downButton = new Button(Content.Load<Texture2D>("Controls/basicButton"), Content.Load<SpriteFont>("Fonts/Font"))
            {
                Position = new Vector2(1300, 32),
                Width = 32,
                Height = 32,
                Text = "v",
            };
            var rightButton = new Button(Content.Load<Texture2D>("Controls/basicButton"), Content.Load<SpriteFont>("Fonts/Font"))
            {
                Position = new Vector2(1332, 32),
                Width = 32,
                Height = 32,
                Text = ">",
            };
            var acceptButton = new Button(Content.Load<Texture2D>("Controls/basicButton"), Content.Load<SpriteFont>("Fonts/Font"))
            {
                Position = new Vector2(1400, 0),
                Width = 128,
                Height = 32,
                Text = "ACCEPT",
            };
            var clearButton = new Button(Content.Load<Texture2D>("Controls/basicButton"), Content.Load<SpriteFont>("Fonts/Font"))
            {
                Position = new Vector2(300, 0),
                Width = 200,
                Height = 64,
                Text = "CLEAR",
            };
            var FactoryButton = new Button(Content.Load<Texture2D>("Controls/basicButton"), Content.Load<SpriteFont>("Fonts/Font"))
            {
                Position = new Vector2(300, 0),
                Width = 200,
                Height = 64,
                Text = "FACTORY",
            };

            smallMapButton.Click += SmallMapButton_Click;
            mediumMapButton.Click += MediumMapButton_Click;
            largeMapButton.Click += LargeMapButton_Click;
            newGameButton.Click += NewGameButton_Click;
            menuButton.Click += MenuButton_Click;
            zoomInButton.Click += zoomInButton_Click;
            zoomOutButton.Click += ZoomOutButton_Click;
            upButton.Click += UpButton_Click;
            leftButton.Click += LeftButton_Click;
            downButton.Click += DownButton_Click;
            rightButton.Click += RightButton_Click;
            acceptButton.Click += AcceptButton_Click;
            testButton.Click += TestButton_Click;
            clearButton.Click += ClearButton_Click;
            FactoryButton.Click += FactoryButton_Click;


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
                zoomInButton,
                zoomOutButton,
                upButton,
                leftButton,
                rightButton,
                downButton,
                acceptButton,
            };
            mainGameScreenComponents = new List<Component>()
            {
                testButton,
                menuButton,
                zoomInButton,
                zoomOutButton,
                upButton,
                leftButton,
                rightButton,
                downButton,
            };
        }

        private void TestButton_Click(object sender, System.EventArgs e)
        {
            /*
            if (init)
            {
                Item i1 = new Item("OmniPin", "A pin for holding a bumbo to three blonks", 1, false, "factory", 10, 10, 2);
                f.addContainedItem(i1, 100);
                Item i2 = new Item("bumbo", "Connected to an OmniPin", 1, false, "factory", 10, 10, 1);
                f.addContainedItem(i2, 100);
                Item i3 = new Item("blonk", "Connected to an OmniPin", 1, false, "factory", 10, 10, 1);
                f.addContainedItem(i3, 100);
                Item i = new Item("cheem", "comprised of an OmniPin plus accessories", 1, true, "factory", 30, 30, 3);
                i.addNeededItem(i1, 1);
                i.addNeededItem(i2, 1);
                i.addNeededItem(i3, 3);
                f.addProduction(i, 10, 1);
                Item i4 = new Item("Blop", "comprised of an OmniPin plus accessories", 2, true, "factory", 20, 20, 3);
                i4.addNeededItem(i1, 1);
                i4.addNeededItem(i2, 2);
                i4.addNeededItem(i3, 2);
                f.addProduction(i4, 15, 1);
                init = false;
            }
            f.produce(1000);
            */
        }

        private void FactoryButton_Click(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void ClearButton_Click(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void AcceptButton_Click(object sender, System.EventArgs e)
        {
            if (p1.mapExists)
                currentState = Player.GameState.mainGame;
        }

        private void RightButton_Click(object sender, System.EventArgs e)
        {
            p1.mapCenter.X--;
        }

        private void DownButton_Click(object sender, System.EventArgs e)
        {
            p1.mapCenter.Y--;
        }

        private void LeftButton_Click(object sender, System.EventArgs e)
        {
            p1.mapCenter.X++;
        }

        private void UpButton_Click(object sender, System.EventArgs e)
        {
            p1.mapCenter.Y++;
        }

        private void LargeMapButton_Click(object sender, System.EventArgs e)
        {
            p1.mapSize = 3;
            p1.mapExists = true;
            p1.zoomPct = 100;
            gameMap.generateMap(p1.mapSize);
        }
        private void MediumMapButton_Click(object sender, System.EventArgs e)
        {
            p1.mapSize = 2;
            p1.mapExists = true;
            p1.zoomPct = 100;
            gameMap.generateMap(p1.mapSize);
        }
        private void SmallMapButton_Click(object sender, System.EventArgs e)
        {
            p1.mapSize = 1;
            p1.mapExists = true;
            p1.zoomPct = 100;
            gameMap.generateMap(p1.mapSize);
        }
        private void NewGameButton_Click(object sender, System.EventArgs e)
        {
            currentState = Player.GameState.newGame;
        }
        private void MenuButton_Click(object sender, System.EventArgs e)
        {
            currentState = Player.GameState.menu;
        }
        private void zoomInButton_Click(object sender, System.EventArgs e)
        {
            if (p1.zoomPct >= 80)
                p1.zoomPct -= 5;
        }
        private void ZoomOutButton_Click(object sender, System.EventArgs e)
        {
            if (p1.zoomPct <= 95)
                p1.zoomPct += 5;
        }
        
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            GraphicsDevice.Clear(Color.White);
            switch (currentState)
            {
                case Player.GameState.menu:
                    DrawMenu(gameTime);
                    break;
                case Player.GameState.settings:
                    DrawSettings(gameTime);
                    break;
                case Player.GameState.newGame:
                    DrawNewGame(gameTime);
                    break;
                case Player.GameState.mainGame:
                    DrawMainGame(gameTime);
                    break;
            }
            
        }

        protected void DrawMenu(GameTime gameTime)
        {
            p1.spriteBatch.Begin();
            foreach (var component in menuScreenComponents)
                component.Draw(gameTime, p1.spriteBatch);
            p1.spriteBatch.End();
        }
        protected void DrawSettings(GameTime gameTime)
        {
            p1.spriteBatch.Begin();
            foreach (var component in settingsScreenComponents)
                component.Draw(gameTime, p1.spriteBatch);
            p1.spriteBatch.End();
        }
        bool dispCities;
        protected void DrawNewGame(GameTime gameTime)
        {
            p1.spriteBatch.Begin();
            foreach (var component in newGameScreenComponents)
                component.Draw(gameTime, p1.spriteBatch);
            p1.spriteBatch.End();

            if (p1.mapExists)
            {
                dispCities = true;
                gameMap.displayMap(100, 100, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, p1.zoomPct, ref p1.mapCenter, dispCities, p1.mapSprite);
                //gameMap.highlightBlock(spriteBatch);
            }
        }
        protected void DrawMainGame(GameTime gameTime)
        {
            p1.spriteBatch.Begin();
            foreach (var component in mainGameScreenComponents)
                component.Draw(gameTime, p1.spriteBatch);
            p1.spriteBatch.End();

            dispCities = false;
            gameMap.displayMap(100, 100, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, p1.zoomPct, ref p1.mapCenter, dispCities, p1.mapSprite);
        }

        public override void Update(GameTime gameTime)
        {
            switch (currentState)
            {
                case Player.GameState.menu:
                    foreach (var component in menuScreenComponents)
                        component.Update(gameTime);
                    break;
                case Player.GameState.settings:
                    foreach (var component in settingsScreenComponents)
                        component.Update(gameTime);
                    break;
                case Player.GameState.newGame:
                    foreach (var component in newGameScreenComponents)
                        component.Update(gameTime);
                    break;
                case Player.GameState.mainGame:
                    foreach (var component in mainGameScreenComponents)
                        component.Update(gameTime);
                    break;
            }
        }
    }
}
