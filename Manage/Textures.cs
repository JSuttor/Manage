using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage
{
    public sealed class Textures
    {
        private static Textures instance = null;

        public static Textures Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Textures();
                }
                return instance;
            }
        }

        //biome chunk textures
        public Texture2D water;
        public Texture2D sand;
        public Texture2D forest;
        public Texture2D grass;
        public Texture2D mountains;
        public Texture2D snow;
        public Texture2D city;
        //biome feature textures
        public Texture2D island;
        public Texture2D boat;
        public Texture2D cacti;
        public Texture2D cactus;
        public Texture2D desert;
        public Texture2D desertRock;
        public Texture2D desertHill;
        public Texture2D dune;
        public Texture2D forest1;
        public Texture2D forest2;
        public Texture2D forest3;
        public Texture2D forest4;
        public Texture2D grass1;
        public Texture2D hill;
        public Texture2D hill2;
        public Texture2D hill3;
        public Texture2D house1;
        public Texture2D iceHill;
        public Texture2D iceHill2;
        public Texture2D monster;
        public Texture2D mountain1;
        public Texture2D mountain2;
        public Texture2D mountain3;
        public Texture2D mountain4;
        public Texture2D waves;
        public Texture2D waves2;
        public Texture2D waves3;
        //building textures
        public Texture2D factory;


        public void loadAssets(ContentManager Content)
        {
            //load biome image assets
            water = Content.Load<Texture2D>("map/water");
            sand = Content.Load<Texture2D>("map/sand");
            forest = Content.Load<Texture2D>("map/forest");
            grass = Content.Load<Texture2D>("map/grass");
            mountains = Content.Load<Texture2D>("map/mountain");
            snow = Content.Load<Texture2D>("map/snow");
            city = Content.Load<Texture2D>("map/city");


            cacti = Content.Load<Texture2D>("map/cacti");
            cactus = Content.Load<Texture2D>("map/cactus");
            desert = Content.Load<Texture2D>("map/desert");
            desertRock = Content.Load<Texture2D>("map/desertRock");
            desertHill = Content.Load<Texture2D>("map/desertHill");
            dune = Content.Load<Texture2D>("map/dune");

            forest1 = Content.Load<Texture2D>("map/forest1");
            forest2 = Content.Load<Texture2D>("map/forest2");
            forest3 = Content.Load<Texture2D>("map/forest3");
            forest4 = Content.Load<Texture2D>("map/forest4");

            mountain1 = Content.Load<Texture2D>("map/mountain1");
            mountain2 = Content.Load<Texture2D>("map/mountain2");
            mountain3 = Content.Load<Texture2D>("map/mountain3");
            mountain4 = Content.Load<Texture2D>("map/mountain4");

            grass1 = Content.Load<Texture2D>("map/grass1");
            hill = Content.Load<Texture2D>("map/hill");
            hill2 = Content.Load<Texture2D>("map/hill2");
            hill3 = Content.Load<Texture2D>("map/hill3");
            house1 = Content.Load<Texture2D>("map/house1");

            iceHill = Content.Load<Texture2D>("map/iceHill");
            iceHill2 = Content.Load<Texture2D>("map/iceHill2");

            monster = Content.Load<Texture2D>("map/monster");
            waves = Content.Load<Texture2D>("map/waves");
            waves2 = Content.Load<Texture2D>("map/waves2");
            waves3 = Content.Load<Texture2D>("map/waves3");
            boat = Content.Load<Texture2D>("map/boat");
            island = Content.Load<Texture2D>("map/island");

            factory = Content.Load<Texture2D>("buildings/factory");
        }

    }
}
