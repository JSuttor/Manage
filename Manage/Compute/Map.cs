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
        bool generatingPos = false;

        //ACCESSIBLE
        int cityNum;                    //populated with number of cities
        int[,] mapArr;                  //populated with integer representation of biome map
        int[,] cityArr;                 //populated with cities (for now, same as start points)
        int mapSize;
        int mapSizeX = 128;             //populated with number of horizontal chunks
        int mapSizeY = 128;             //populated with number of vertical chunks
        int totalBiomes;                //populated with number of biomes, biomeRepeat * biomeNum
        int[,] startPts;                //populated with beginning points for biome gen
        Texture2D[,] features;          //map visual features
        Vector2[,] positions;           //actual position of every map block
        Rectangle[,] blockRects;



        //constructor
        public Map(ContentManager content)
        {
            //load biome images on new object
            loadAssets(content);
        }

        //generate biome map
        public void generateMap(int passMapSize)
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
            mapSize = passMapSize;
            initCenter = true;

            positions = null;
            generatingPos = false;

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
                        randY = random.Next(Convert.ToInt32(minDistance / 2), Convert.ToInt32((mapSizeY - minDistance / 2)));
                        //start over
                        j = -1;
                    }
                }
                biomeType = i;
                //resolves biomes to possible numbers. ie for 5 biomes, numbers 6-10 will become 1-5 when there are 2 of each biome. This allows for a single iteration
                for (int j = 1; j <= biomeRepeat; j++)
                {
                    //test which biome repeat we are on
                    if ((j - 1) * biomeNum < i && j * biomeNum >= i)
                    {
                        //subtract to get number from 1 to number of biomes
                        biomeType -= (j - 1) * biomeNum;
                    }
                }
                //add start point to tracking array
                startPts[i - 1, 0] = randX;
                startPts[i - 1, 1] = randY;
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

            //place map visual features

            //this is a little weird so bare with. A number will be genned 0-X. X will usually be something like 10x the number of feature types for a biome. 
            //the chance integer determines how many numbers of that x could result in that feature being placed if genned by random
            //the number of random numbers genned is determined by the size of the biome, and that code is next

            //chances out of 120
            int boatChance = 1;
            int wavesChance = 1;
            int waves2Chance = 1;
            int waves3Chance = 1;
            int monsterChance = 1;
            int islandChance = 1;

            //chances out of 50
            int cactiChance = 2;
            int cactusChance = 2;
            int desertChance = 6;
            int desertRockChance = 2;
            int desertHillChance = 8;
            int duneChance = 2;

            //chances out of 40
            int forest1Chance = 10;
            int forest2Chance = 10;
            int forest3Chance = 10;
            int forest4Chance = 10;

            //chances out of 60
            int grass1Chance = 10;
            int hillChance = 2;
            int hill2Chance = 2;
            int hill3Chance = 2;
            int house1Chance = 1;

            //chances out of 40
            int iceHillChance = 1;
            int iceHill2Chance = 2;

            //chances out of 20
            int mountain1Chance = 10;
            int mountain2Chance = 10;
            int mountain3Chance = 10;
            int mountain4Chance = 2;

            //number of random numbers generated that could result in a map feature
            int passes = 1000;
            if(mapSize == 1)
            {
                passes = 1500;
                
            }
            else if (mapSize == 2)
            {
                passes = 2000;
            }
            else if (mapSize == 3)
            {
                passes = 2500;
            }

            //determine how far apart features must be
            minDistance = 2;

            //determine size of feature array based on size of map
            features = new Texture2D[mapSizeX, mapSizeY];

            //generate features
            for (int i = 0; i < passes; i++)
            {
                //generate random position
                randX = random.Next(Convert.ToInt32(minDistance / 2), Convert.ToInt32((mapSizeX - minDistance / 2)));
                randY = random.Next(Convert.ToInt32(minDistance / 2), Convert.ToInt32((mapSizeY - minDistance / 2)));
                //for each possible position on map
                for (int j = 0; j < mapSizeX; j++)
                {
                    for (int k = 0; k < mapSizeY; k++)
                    {
                        //if a feature exists, test it
                        if (features[j, k] != null)
                        {
                            //calculate distance from existing start point, formula    d = sqrt((x2-x1)^2 + (y2-y1)^2)
                            distance = Math.Sqrt(Math.Pow(Convert.ToDouble(randX - j), 2.0) + Math.Pow(Convert.ToDouble(randY - k), 2.0));
                            //reposition if its too close to a point, and restart testing from the top
                            if (distance < minDistance)
                            {
                                //new randomized
                                randX = random.Next(Convert.ToInt32(minDistance / 2), Convert.ToInt32((mapSizeX - minDistance / 2)));
                                randY = random.Next(Convert.ToInt32(minDistance / 2), Convert.ToInt32((mapSizeY - minDistance / 2)));
                                //start over
                                j = 0;
                                k = 0;
                            }
                        }
                    }
                }
                //biome of current random position
                biomeType = mapArr[randX, randY];
                //random number holder
                int feat;
                //by biome type
                switch (biomeType)
                {
                    case 0:
                        feat = random.Next(0, 120);     //generate a random number 
                        if (feat >= 0 && feat < 0 + boatChance)     //determine if and what feature to place
                        {
                            features[randX, randY] = boat;
                        }
                        else if (feat >= 10 && feat < 10 + wavesChance)
                        {
                            features[randX, randY] = waves;
                        }
                        else if (feat >= 20 && feat < 20 + waves2Chance)
                        {
                            features[randX, randY] = waves2;
                        }
                        else if (feat >= 30 && feat < 30 + waves3Chance)
                        {
                            features[randX, randY] = waves3;
                        }
                        else if (feat >= 40 && feat < 40 + monsterChance)
                        {
                            features[randX, randY] = monster;
                        }
                        else if (feat >= 50 && feat < 50 + islandChance)
                        {
                            features[randX, randY] = island;
                        }
                        break;
                    case 1:
                        feat = random.Next(0, 50);      //generate a random number 
                        if (feat >= 0 && feat < 0 + grass1Chance)   //determine if and what feature to place
                        {
                            features[randX, randY] = grass1;
                        }
                        else if (feat >= 10 && feat < 10 + hillChance)
                        {
                            features[randX, randY] = hill;
                        }
                        else if (feat >= 20 && feat < 20 + hill2Chance)
                        {
                            features[randX, randY] = hill2;
                        }
                        else if (feat >= 30 && feat < 30 + hill3Chance)
                        {
                            features[randX, randY] = hill3;
                        }
                        else if (feat >= 40 && feat < 40 + house1Chance)
                        {
                            features[randX, randY] = house1;
                        }
                        break;
                    case 2:
                        feat = random.Next(0, 40);      //generate a random number 
                        if (feat >= 0 && feat < 0 + forest1Chance)  //determine if and what feature to place
                        {
                            features[randX, randY] = forest1;
                        }
                        else if (feat >= 10 && feat < 10 + forest2Chance)
                        {
                            features[randX, randY] = forest2;
                        }
                        else if (feat >= 20 && feat < 20 + forest3Chance)
                        {
                            features[randX, randY] = forest3;
                        }
                        else if (feat >= 30 && feat < 30 + forest4Chance)
                        {
                            features[randX, randY] = forest4;
                        }
                        break;
                    case 3:
                        feat = random.Next(0, 60);      //generate a random number 
                        if (feat >= 0 && feat < 0 + cactiChance)    //determine if and what feature to place
                        {
                            features[randX, randY] = cacti;
                        }
                        else if (feat >= 10 && feat < 10 + cactusChance)
                        {
                            features[randX, randY] = cactus;
                        }
                        else if (feat >= 20 && feat < 20 + desertChance)
                        {
                            features[randX, randY] = desert;
                        }
                        else if (feat >= 30 && feat < 30 + desertRockChance)
                        {
                            features[randX, randY] = desertRock;
                        }
                        else if (feat >= 40 && feat < 40 + duneChance)
                        {
                            features[randX, randY] = dune;
                        }
                        else if (feat >= 30 && feat < 30 + desertHillChance)
                        {
                            features[randX, randY] = desertHill;
                        }
                        break;
                    case 4:
                        feat = random.Next(0, 40);      //generate a random number 
                        if (feat >= 0 && feat < 0 + mountain1Chance)    //determine if and what feature to place
                        {
                            features[randX, randY] = mountain1;
                        }
                        else if (feat >= 10 && feat < 10 + mountain2Chance)
                        {
                            features[randX, randY] = mountain2;
                        }
                        else if (feat >= 20 && feat < 20 + mountain3Chance)
                        {
                            features[randX, randY] = mountain3;
                        }
                        else if (feat >= 30 && feat < 30 + mountain4Chance)
                        {
                            features[randX, randY] = mountain4;
                        }
                        break;

                    case 5:
                        feat = random.Next(0, 20);      //generate a random number 
                        if (feat >= 0 && feat < 0 + iceHillChance)      //determine if and what feature to place
                        {
                            features[randX, randY] = iceHill;
                        }
                        else if (feat >= 10 && feat < 10 + iceHill2Chance)
                        {
                            features[randX, randY] = iceHill2;
                        }
                        break;
                }
                //add biome start point to map
                mapArr[randX, randY] = (biomeType);
            }
            //add cities to features. Will be 
            for(int i = 0; i < cityNum; i++)
            {
                features[cityArr[i, 0], cityArr[i, 1]] = city;
            }
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

        Vector2 mapCenter;
        bool initCenter = true;
        int zoomStartX;
        int zoomStartY;
        int zoomSubX;
        int zoomSubY;

        //display map to screen in position and size passed
        public void displayMap(int startX, int startY, int dispSizeX, int dispSizeY, int zoomPct, ref Vector2 moveMap, bool dispCities, SpriteBatch mapSprite)
        {
            //define the map ratio. This will later allow different ratios in different areas if passed
            double xRatio = 15;             //x per yRatio
            double yRatio = 9;              //y per xRatio

            int mapBlocksX;
            int mapBlocksY;

            //if the y size is too large for the x size to maintain the ratio
            if (dispSizeX * yRatio / xRatio < dispSizeY)
            {
                //define the y size based on the ratio of x to y
                dispSizeY = Convert.ToInt32(Convert.ToDouble(dispSizeX) * yRatio / xRatio);
            }
            //if the x size is too large for the y size to maintain the ratio
            else
            {
                //define the x size based on the ratio of y to x
                dispSizeX = Convert.ToInt32(Convert.ToDouble(dispSizeY) * xRatio / yRatio);
            }

            //determine blocks to be shown given zoom
            zoomStartX = mapSizeX - Convert.ToInt32(Convert.ToDouble(mapSizeX) * (Convert.ToDouble(zoomPct) / 100.0)) + Convert.ToInt32(mapCenter.X);
            zoomStartY = mapSizeX - Convert.ToInt32(Convert.ToDouble(mapSizeY) * (Convert.ToDouble(zoomPct) / 100.0)) + Convert.ToInt32(mapCenter.Y);
            zoomSubX = Convert.ToInt32(Convert.ToDouble(mapSizeX) * (Convert.ToDouble(zoomPct) / 100.0)) + Convert.ToInt32(mapCenter.X);
            zoomSubY = Convert.ToInt32(Convert.ToDouble(mapSizeY) * (Convert.ToDouble(zoomPct) / 100.0)) + Convert.ToInt32(mapCenter.Y);

            //on new map generation, set map display center to 0
            if (initCenter)
            {
                mapCenter.X = 0;
                mapCenter.Y = 0;
                initCenter = false;

            }

            //determine how to pan map when zoomed
            if(moveMap.X == -1 && zoomSubX < mapSizeX)
            {
                mapCenter.X++;
            }
            else if (moveMap.X == 1 && zoomStartX > 0)
            {
                mapCenter.X--;
            }
            moveMap.X = 0;
            if (moveMap.Y == -1 && zoomSubY < mapSizeY)
            {
                mapCenter.Y++;
            }
            else if (moveMap.Y == 1 && zoomStartY > 0)
            {
                mapCenter.Y--;
            }
            moveMap.Y = 0;

            //number of blocks given zoom
            mapBlocksX = zoomSubX - zoomStartX;
            mapBlocksY = zoomSubY - zoomStartY;

            //pan map if able
            if(zoomSubX > mapSizeX)
            {
                zoomStartX -= (zoomSubX - mapSizeX);
                mapCenter.X -= (zoomSubX - mapSizeX);
                zoomSubX = mapSizeX;
            }
            if (zoomSubY > mapSizeY)
            {
                zoomStartY -= (zoomSubY - mapSizeY);
                mapCenter.Y -= (zoomSubY - mapSizeY);
                zoomSubY = mapSizeY;
            }
            if (zoomStartX < 0)
            {
                mapCenter.X -= zoomStartX;
                zoomSubX -= zoomStartX;
                zoomStartX = 0;
            }
            if (zoomStartY < 0)
            {
                mapCenter.Y -= zoomStartY;
                zoomSubY -= zoomStartY;
                zoomStartY = 0;
            }

            //TEMPORARY USE VARIABLES FOR MAP DISPLAY
            double xPos = startX;           //actual position from upper left of map that current chunk is drawn from
            double yPos = startY;           //  ''
            //chunk's size as a double. Each chunk will be slightly smaller or larger depending on position to allow for more dynamic size variation
            double xSize = (Convert.ToDouble(dispSizeX) - (Convert.ToDouble(startX) * 2)) / Convert.ToDouble(mapBlocksX);     
            double ySize = (Convert.ToDouble(dispSizeY) - (Convert.ToDouble(startY) * 2)) / Convert.ToDouble(mapBlocksY);
            //These 4 are calculated using xSize and ySize doubles for each chunk on the fly so that the map size does not have to
            //be smaller or larger than intended due to a set number of chunks of a set size
            int xPosInt = Convert.ToInt32(xPos);
            int yPosInt = Convert.ToInt32(yPos);
            int xSizeInt = Convert.ToInt32(xSize);
            int ySizeInt = Convert.ToInt32(ySize);



            //a table to look up the pixel position of the upper left corner of map blocks
            if (positions == null) {
                positions = new Vector2[mapSizeX, mapSizeY];
                generatingPos = true;
            }

            mapSprite.Begin();
            blockRects = new Rectangle[mapSizeX, mapSizeY];
            //for each chunk on map
            for (int i = zoomStartY; i < zoomSubY; i++) 
            {
                for (int j = zoomStartX; j < zoomSubX; j++)
                {
                    //resolve size and position to integer (always shrinks to avoid gaps between chunks, 
                    //this is a product of double to int conversion but works to my advantage)
                    xPosInt = Convert.ToInt32(xPos);
                    yPosInt = Convert.ToInt32(yPos);
                    if (generatingPos)
                    {
                        positions[i, j].X = xPosInt;
                        positions[i, j].Y = yPosInt;
                    }
                    xSizeInt = Convert.ToInt32(xSize) + 1;
                    ySizeInt = Convert.ToInt32(ySize) + 1;

                    blockRects[i, j] = new Rectangle(xPosInt, yPosInt, xSizeInt, ySizeInt);
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

            //determine the size of map features
            int size = dispSizeX / 30;
            //no change for small
            if (mapSize == 2)
            {
                //shrink the size of features by a little
                size = Convert.ToInt32(Convert.ToDouble(size) * .8);
            }
            else if (mapSize == 3)
            {
                //shrink the size of features by a lot
                size = Convert.ToInt32(Convert.ToDouble(size) * .6);
            }
            size = Convert.ToInt32(Convert.ToDouble(size) / (Convert.ToDouble(zoomPct) / 100.0));
            int printSize;
            //draw map features
            for (int i = zoomStartY; i < zoomSubY; i++) //for each x
            {
                for (int j = zoomStartX; j < zoomSubX; j++) //for each y
                {
                    if(features[i,j] != null) //if there is a feature on this block
                    {
                        printSize = size;
                        if (features[i, j] == city && dispCities)
                        {
                            printSize = Convert.ToInt32(Convert.ToDouble(size) * 1.2);
                            mapSprite.Draw(features[i, j], new Rectangle(Convert.ToInt32(positions[i, j].X) - printSize / 2, Convert.ToInt32(positions[i, j].Y) - printSize / 2, printSize, printSize), Color.White);
                        }
                        else
                        {
                            //draw a feasture here. Get pixel position from positions, centering using size.
                            mapSprite.Draw(features[i, j], new Rectangle(Convert.ToInt32(positions[i, j].X) - printSize / 2, Convert.ToInt32(positions[i, j].Y) - printSize / 2, printSize, printSize), Color.White);
                        }
                    }
                }
            }
            
            mapSprite.End();
        }

        //biome chunk textures
        private Texture2D water;
        private Texture2D sand;
        private Texture2D forest;
        private Texture2D grass;
        private Texture2D mountains;
        private Texture2D snow;
        private Texture2D city;
        //biome feature textures
        private Texture2D island;
        private Texture2D boat;
        private Texture2D cacti;
        private Texture2D cactus;
        private Texture2D desert;
        private Texture2D desertRock;
        private Texture2D desertHill;
        private Texture2D dune;
        private Texture2D forest1;
        private Texture2D forest2;
        private Texture2D forest3;
        private Texture2D forest4;
        private Texture2D grass1;
        private Texture2D hill;
        private Texture2D hill2;
        private Texture2D hill3;
        private Texture2D house1;
        private Texture2D iceHill;
        private Texture2D iceHill2;
        private Texture2D monster;
        private Texture2D mountain1;
        private Texture2D mountain2;
        private Texture2D mountain3;
        private Texture2D mountain4;
        private Texture2D waves;
        private Texture2D waves2;
        private Texture2D waves3;

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
        }

        private MouseState currentMouse;
        private bool isHovering;
        private MouseState previousMouse;
        public void highlightBlock(SpriteBatch spriteBatch)
        {
            previousMouse = currentMouse;
            currentMouse = Mouse.GetState();
            var mouseRectangle = new Rectangle(currentMouse.X, currentMouse.Y, 1, 1);
            isHovering = false;
            bool alreadyDetected = false;
            spriteBatch.Begin();

            for (int i = 0; i < mapSizeY; i++)
            {
                for (int j = 0; j < mapSizeX; j++)
                {
                    if (mouseRectangle.Intersects(blockRects[i,j]) && alreadyDetected == false)
                    {
                        isHovering = true;
                        if (mapArr[i, j] == 0)
                        {
                            spriteBatch.Draw(water, blockRects[i, j], Color.LightGray);
                            alreadyDetected = true;
                        }
                        //if 1, chunk has grass texture
                        else if (mapArr[i, j] == 1)
                        {
                            spriteBatch.Draw(grass, blockRects[i, j], Color.LightGray);
                            alreadyDetected = true;
                        }
                        //if 2, chunk has forest texture
                        else if (mapArr[i, j] == 2)
                        {
                            spriteBatch.Draw(forest, blockRects[i, j], Color.LightGray);
                            alreadyDetected = true;
                        }
                        //if 3, chunk has sand texture
                        else if (mapArr[i, j] == 3)
                        {
                            spriteBatch.Draw(sand, blockRects[i, j], Color.LightGray);
                            alreadyDetected = true;
                        }
                        //if 4, chunk has mountains texture
                        else if (mapArr[i, j] == 4)
                        {
                            spriteBatch.Draw(mountains, blockRects[i, j], Color.LightGray);
                            alreadyDetected = true;
                        }
                        //if 5, chunk has snow texture
                        else if (mapArr[i, j] == 5)
                        {
                            spriteBatch.Draw(snow, blockRects[i, j], Color.LightGray);
                            alreadyDetected = true;
                        }


                        /*
                        if (currentMouse.LeftButton == ButtonState.Released && previousMouse.LeftButton == ButtonState.Pressed)
                        {
                            Click?.Invoke(this, new EventArgs()); ;
                        }
                        */
                    }
                }
            }
            spriteBatch.End();

        }
    }
}

