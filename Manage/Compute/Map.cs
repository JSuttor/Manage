using Microsoft.Xna.Framework.Graphics;
using Manage.Compute;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Devices.PointOfService;
using Windows.UI.Xaml.Controls.Maps;
using Microsoft.Xna.Framework.Content;
using Windows.Security.Cryptography.Certificates;
using System.ComponentModel;

namespace Manage.Compute
{
    class Map
    {
        //TOOLS
        Random random = new Random();   //used to generate random numbers

        //PARAMETERS
        int biomeNum = 5;               //number of types of biomes
        int biomeRepeat;                //number of each biome on the map
        int biomeSizeMult;              //smaller value is larger biome

        //ACCESSIBLE
        int cityNum;                    //populated with number of cities
        int[,] mapArr;                  //populated with integer representation of biome map
        int[,] cityArr;                 //populated with cities (for now, same as start points)
        int mapSizeX = 128;             //populated with number of horizontal chunks
        int mapSizeY = 128;             //populated with number of vertical chunks
        int totalBiomes;                //populated with number of biomes, biomeRepeat * biomeNum
        int[,] startPts;                //populated with beginning points for biome gen

        //biome chunk textures
        private Texture2D water;
        private Texture2D sand;
        private Texture2D forest;
        private Texture2D grass;
        private Texture2D mountains;
        private Texture2D snow;
        private Texture2D city;

        //constructor
        public Map(ContentManager content)
        {
            //load biome images on new object
            loadAssets(content);
        }

        //generate biome map
        public void generateMap(int mapSize)
        {
            //note, algorithm slightly favors stretching biomes towards bottom right due to looping. Think about later. Maybe double buffer the array? sounds like a maybe
            //note, adding too many biomes and repeats will basically eliminate all of the water. Is this important? probably not. But really makes ya think.

            //TEMPORARY USE VARIABLES FOR MAP GEN
            int randX;                      //stores current random number
            int randY;                      //stores current random number
            double distance;                //stores current distance between start points
            int biomeType;                  //stores currently placed biome start point
            int current;                    //stores biome type being expanded for biome gen
            int direction;                  //tracks direction of expansion for biome gen

            //set biome parameters for different sizes. small = 1, medium = 2, large = 3. see comments documenting variable uses in vatriable declaration
            if (mapSize == 1)                   //small map
            {
                biomeSizeMult = 6;              //small biomes
                mapSizeX = 64;
                mapSizeY = 64;
                biomeRepeat = 1;                //1 of each
            }
            else if (mapSize == 2)              //medium map
            {
                biomeSizeMult = 4;              //medium biomes
                mapSizeX = 128;
                mapSizeY = 128;
                biomeRepeat = 2;                //2 of each
            }
            else                                //large map
            {
                biomeSizeMult = 2;              //large biomes
                mapSizeX = 256;
                mapSizeY = 256;
                biomeRepeat = 3;                //3 of each
            }
            
            mapArr = new int[mapSizeX, mapSizeY];   //declare map based on size
            totalBiomes = biomeNum * biomeRepeat;   //populate total biomes
            startPts = new int[totalBiomes, 4];     //declare number of start points based on biome number

            //calculate the minimum distance biome start points can be placed from each other
            int minDistance = minDistance = (((mapSizeX + mapSizeY) / 2) / (totalBiomes)); ;
            if (mapSize != 1)
            {
                //for the small map size, biomes can be placed closer together.  
                //Without this, there is the potential for small map gen to infinitely loop
                minDistance *= 2;
            }
            
            //customize the size of non-water biomes. 1 is big, 2 is medium, 3 is small, 4 is tiny
            int size1 = 1;
            int size2 = 2;
            int size3 = 2;
            int size4 = 3;
            int size5 = 4;
            int sizeDef = 2;
            
            //loop to place biomes
            for (int i = 1; i <= totalBiomes; i++)
            {
                //initial randomized
                randX = random.Next(Convert.ToInt32(minDistance / 2), Convert.ToInt32((mapSizeX - minDistance / 2)));
                randY = random.Next(Convert.ToInt32(minDistance / 2), Convert.ToInt32((mapSizeY - minDistance / 2)));
                //for each placed entry
                for (int j = 0; j < i; j++)
                {
                    //calculate distance from existing start point, formula    d = sqrt((x2-x1)^2 + (y2-y1)^2)
                    distance = Math.Sqrt(Math.Pow(Convert.ToDouble(randX - startPts[j, 0]), 2.0) + Math.Pow(Convert.ToDouble(randY - startPts[j, 1]), 2.0));
                    //reposition if its too close to a point, and restart testing from the top
                    if (distance < minDistance)
                    {
                        //new randomized
                        randX = random.Next(Convert.ToInt32(minDistance / 2), Convert.ToInt32((mapSizeX - minDistance / 2)));
                        randY = random.Next(Convert.ToInt32(minDistance / 2), Convert.ToInt32((mapSizeY- minDistance / 2)));
                        //start over
                        j = -1;
                    }
                }
                biomeType = i;
                //resolves biomes to possible numbers. ie for 5 biomes, numbers 6-10 will become 1-5 when there are 2 of each biome. This allows for a single iteration
                for(int j = 1; j <= biomeRepeat; j++)
                {
                    //test which biome repeat we are on
                    if ((j - 1) * biomeNum < i && j * biomeNum >= i)
                    {
                        //subtract to get number from 1 to number of biomes
                        biomeType -= (j - 1) * biomeNum;
                    }
                }
                //add start point to tracking array
                startPts[i-1, 0] = randX;
                startPts[i-1, 1] = randY;
                //add biome start point to map
                mapArr[randX, randY] = (biomeType);
            }

            //expand biomes from start point
            //100 expansion opportunities for each chunk
            for (int count = 0; count < 100; count++)
            {
                //for each chunk on the map
                for (int i = 0; i < mapSizeX; i++)
                {
                    for (int j = 0; j < mapSizeY; j++)
                    {
                        //current chunk's biome
                        current = mapArr[i, j];
                        //if the chunk is not water
                        if(current != 0)
                        {
                            //this randomization includes both the probbility of expansion and randomizing the direction of expansion
                            //biomeSizeMult determines the probability that an expansion will occur
                            //if generated number is from 1-3, expansion will occur
                            if(current == 1)
                                direction = random.Next(0, (size1 * biomeSizeMult) + 3);
                            else if(current == 2)
                                direction = random.Next(0, (size2 * biomeSizeMult) + 3);
                            else if(current == 3)
                                direction = random.Next(0, (size3 * biomeSizeMult) + 3);
                            else if(current == 4)
                                direction = random.Next(0, (size4 * biomeSizeMult) + 3);
                            else if(current == 5)
                                direction = random.Next(0, (size5 * biomeSizeMult) + 3);
                            else
                                direction = random.Next(0, (sizeDef * biomeSizeMult) + 3);
                            //if random int is within expansion range, figure out direction and place another chunk of the biome in that direction
                            //if statement also makes sure chunk will not attempt to place out of the bounds of the map array
                            if (direction == 0 && ((i + 1) < mapSizeX))
                            {
                                if (mapArr[i + 1, j] == 0)
                                    mapArr[i + 1, j] = current;     //place chunk to right
                            }
                            else if (direction == 1 && ((i - 1) >= 0))
                            {
                                if (mapArr[i - 1, j] == 0)
                                    mapArr[i - 1, j] = current;     //place chunk to left
                            }
                            else if (direction == 2 && ((j + 1) < mapSizeY))
                            {
                                if (mapArr[i, j + 1] == 0)  
                                    mapArr[i, j + 1] = current;     //place chunk down
                            }
                            else if (direction == 3 && ((j - 1) >= 0))
                            {
                                if (mapArr[i, j - 1] == 0)
                                    mapArr[i, j - 1] = current;     //place chunk up
                            }
                        }
                    }
                }
            }
            //cities are at the starting point of biome gen. At least for now
            cityArr = startPts;
            cityNum = totalBiomes;
        }

        //purely for testing purposes, put an integer representation of the biome map in the console
        public void printMapArr(int xSize, int ySize)
        {
            for (int i = 0; i < xSize; i++)
            {
                for (int j = 0; j < ySize; j++)
                {
                    System.Diagnostics.Debug.Write(string.Format("{0}", mapArr[i, j]));
                }
                System.Diagnostics.Debug.Write(Environment.NewLine);
            }
        }

        //display map to screen in position and size passed
        public void displayMap(int startX, int startY, int dispSizeX, int dispSizeY, SpriteBatch mapSprite)
        {
            //TEMPORARY USE VARIABLES FOR MAP DISPLAY
            double xPos = startX;           //actual position from upper left of map that current chunk is drawn from
            double yPos = startY;           //  ''
            //chunk's size as a double. Each chunk will be slightly smaller or larger depending on position to allow for more dynamic size variation
            double xSize = (Convert.ToDouble(dispSizeX) - (Convert.ToDouble(startX) * 2)) / Convert.ToDouble(mapSizeX);     
            double ySize = (Convert.ToDouble(dispSizeY) - (Convert.ToDouble(startY) * 2)) / Convert.ToDouble(mapSizeY);
            //These 4 are calculated using xSize and ySize doubles for each chunk on the fly so that the map size does not have to
            //be smaller or larger than intended due to a set number of chunks of a set size
            int xPosInt = Convert.ToInt32(xPos);
            int yPosInt = Convert.ToInt32(yPos);
            int xSizeInt = Convert.ToInt32(xSize);
            int ySizeInt = Convert.ToInt32(ySize);

            mapSprite.Begin();

            //for each chunk on map
            for (int i = 0; i < mapSizeY; i++) 
            {
                for (int j = 0; j < mapSizeX; j++)
                {
                    //resolve size and position to integer (always shrinks to avoid gaps between chunks, 
                    //this is a product of double to int conversion but works to my advantage)
                    xPosInt = Convert.ToInt32(xPos);
                    yPosInt = Convert.ToInt32(yPos);
                    xSizeInt = Convert.ToInt32(xSize) + 1;
                    ySizeInt = Convert.ToInt32(ySize) + 1;
                    //if we are calculating the position of a chunk that happens to be a biome start point, 
                    //add the position to the start points array in slots 2 and 3 for x and y
                    //iterate through startpoints
                    for (int k = 0; k < totalBiomes; k++)
                    {
                        //test for match
                        if (startPts[k,0] == i && startPts[k,1] == j) 
                        {
                            //add physical location
                            startPts[k, 2] = xPosInt;
                            startPts[k, 3] = yPosInt;
                        }
                    }
                    //if 0, chunk has water texture
                    if (mapArr[i,j] == 0)
                    {
                        mapSprite.Draw(water, new Rectangle(xPosInt, yPosInt, xSizeInt, ySizeInt), Color.White);
                    }
                    //if 1, chunk has grass texture
                    else if (mapArr[i, j] == 1)
                    {
                        mapSprite.Draw(grass, new Rectangle(xPosInt, yPosInt, xSizeInt, ySizeInt), Color.White);
                    }
                    //if 2, chunk has forest texture
                    else if (mapArr[i, j] == 2)
                    {
                        mapSprite.Draw(forest, new Rectangle(xPosInt, yPosInt, xSizeInt, ySizeInt), Color.White);
                    }
                    //if 3, chunk has sand texture
                    else if (mapArr[i, j] == 3)
                    {
                        mapSprite.Draw(sand, new Rectangle(xPosInt, yPosInt, xSizeInt, ySizeInt), Color.White);
                    }
                    //if 4, chunk has mountains texture
                    else if (mapArr[i, j] == 4)
                    {
                        mapSprite.Draw(mountains, new Rectangle(xPosInt, yPosInt, xSizeInt, ySizeInt), Color.White);
                    }
                    //if 5, chunk has snow texture
                    else if (mapArr[i, j] == 5)
                    {
                        mapSprite.Draw(snow, new Rectangle(xPosInt, yPosInt, xSizeInt, ySizeInt), Color.White);
                    }
                    //if anything else, no chunk is drawn (error) may add default missing texture later

                    xPos += xSize;      //calculate x of next chunk
                }
                yPos += ySize;          //calculate y of next chunk
                xPos = startX;          //reset next line of chunks to left
            }

            //for each city
            for(int i = 0; i < (cityNum); i++)
            {
                //draw a little city. I'll need to figure out how to make these into buttons or place an 
                //invisible button over them later. also dynamic sizing will be added
                mapSprite.Draw(city, new Rectangle(cityArr[i, 2], cityArr[i, 3], 30, 30), Color.White);
            }
            mapSprite.End();
        }
        
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
        }
    }
}

