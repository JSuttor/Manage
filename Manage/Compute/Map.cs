using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Devices.PointOfService;

namespace Manage.Compute
{
    class Map
    {
        int[,] mapArr;
        public Map()
        {
            generateMap(128, 128, 6, 2);
            printMap(128, 128);
        }
        public void generateMap(int xSize, int ySize, int biomeNum, int biomeRepeat)
        {
            //note, algorithm slightly favors stretching biomes towards bottom right due to looping. Think about later. Maybe double buffer the array? sounds like a maybe

            //note, adding too many biomes and repeats will basically eliminate all of the water. Is this important? probably not. But really makes ya think.
            Random random = new Random();
            mapArr = new int[xSize, ySize];
            //place biome start points
            int[,] startPts = new int[biomeNum * biomeRepeat, 2];
            int minDistance = (((xSize + ySize) / 2) / (biomeNum * biomeRepeat)) * 2;
            int randX;
            int randY;
            double distance;
            //each biome gets placed X times. The last line of this loop achieves this with a modulo.
            for (int i = 1; i < biomeNum * biomeRepeat; i++)
            {
                randX = random.Next(Convert.ToInt32(minDistance / 2), Convert.ToInt32((xSize - minDistance / 2)));
                randY = random.Next(Convert.ToInt32(minDistance / 2), Convert.ToInt32((ySize - minDistance / 2)));
                //for each placed entry
                for (int placed = 0; placed < i-1; placed++)
                {
                    distance = Math.Sqrt(Math.Pow(Convert.ToDouble(randX - startPts[placed, 0]), 2.0) + Math.Pow(Convert.ToDouble(randY - startPts[placed, 1]), 2.0));
                    //reposition if its too close to a point, and restart testing from the top
                    if (distance < minDistance)
                    {
                        randX = random.Next(Convert.ToInt32(minDistance / 2), Convert.ToInt32((xSize - minDistance / 2)));
                        randY = random.Next(Convert.ToInt32(minDistance / 2), Convert.ToInt32((ySize - minDistance / 2)));
                        placed = -1;
                    }
                }
                startPts[i-1, 0] = randX;
                startPts[i-1, 1] = randY;
                mapArr[randX, randY] = (i % biomeNum);
            }
            int current;
            int direction;
            for (int count = 0; count < 100; count++)
            {
                for (int i = 0; i < xSize; i++)
                {
                    for (int j = 0; j < ySize; j++)
                    {
                        current = mapArr[i, j];
                        if(current != 0)
                        {
                            direction = random.Next(0, 10);
                            if (direction == 0 && ((i + 1) < xSize))
                            {
                                if (mapArr[i + 1, j] == 0)
                                    mapArr[i + 1, j] = current;
                            }
                            else if (direction == 1 && ((i - 1) >= 0))
                            {
                                if (mapArr[i - 1, j] == 0)
                                    mapArr[i - 1, j] = current;
                            }
                            else if (direction == 2 && ((j + 1) < ySize))
                            {
                                if (mapArr[i, j + 1] == 0)
                                    mapArr[i, j + 1] = current;
                            }
                            else if (direction == 3 && ((j - 1) >= 0))
                            {
                                if (mapArr[i, j - 1] == 0)
                                    mapArr[i, j - 1] = current;
                            }
                        }
                    }
                }
            }                
        }
        public void printMap(int xSize, int ySize)
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
    }
}

