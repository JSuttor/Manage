using Microsoft.VisualBasic.CompilerServices;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.AllJoyn;
using Windows.UI.Xaml.Documents;

namespace Manage.Compute
{
    class Factory
    {
        struct containedItemHolder
        {
            public Item item;
            public int count;
        }
        struct neededItemHolder
        {
            public Item item;
            public int count;
        }
        struct producedItemHolder
        {
            public Item item;
            public bool canUseCityPower;
            public int targetPerSpan;                   //how many to make per day if possible
            public int productionPriority;              //if limeted materials and power, what get's produced first
        }

        //int storageCapacity;                            //inherent storage capacity of the factory. can be expanded with adjacent warehouse
        //int heldObjects;                                //storage capacity filled

        int factLevel;                                  //factories can be upgraded for more production and concurrent assembly lines
        int powerPriority;                              //determnines what gets power in cases of shortage
        int maxConcurrent;
        int concurrent;                                 //number of concurrent production lines
        containedItemHolder[] containedItemList;        //items stored in the factory
        producedItemHolder[] producedItemList;          //items that are being produced here.  will be of length maxConcurrent

        public Factory(int factLevel)
        {
            this.factLevel = factLevel;
            if(factLevel == 1)
            {
                maxConcurrent = 3;
                //storageCapacity = 1000;
            }
            containedItemList = new containedItemHolder[20];
            producedItemList = new producedItemHolder[20];
            concurrent = 0;
        }

        public void addContainedItem(Item item, int count)
        {
            bool added = false;
            for(int i = 0; i < containedItemList.Length; i++)
            {
                if (!added) {
                    if (containedItemList[i].item == item)
                    {
                        containedItemList[i].count += count;
                        added = true;
                    }
                    else if (containedItemList[i].item == null)
                    {
                        containedItemList[i].item = item;
                        containedItemList[i].count = count;
                        added = true;
                    }
                }
            }
        }
        public bool addProduction(Item item, int targetPerSpan)
        {
            bool added = false;
            for(int i = 0; i < maxConcurrent; i++)
            {
                if (!added)
                {
                    if (producedItemList[i].item == item)
                    {
                        producedItemList[i].targetPerSpan += targetPerSpan;
                        added = true;
                    }
                    else if(producedItemList[i].item == null)
                    {
                        producedItemList[i].item = item;
                        producedItemList[i].targetPerSpan = targetPerSpan;
                        producedItemList[i].productionPriority = 1;
                        producedItemList[i].canUseCityPower = true;
                        added = true;
                    }
                }
                
            }
            return added;
        }


        public bool produce(ref int powerAccess)
        {
            producedItemHolder[] currentPriorityItems;
            producedItemHolder[] produceableItems;
            neededItemHolder[] currentNeededItems;
            containedItemHolder[] leftAfterProd;
            int countAtPriority;
            double powerPct;
            int powerFromCity = 0;
            int neededPower = 0;
            bool producedAdded;
            for (int i = 10; i > 0; i--)
            {
                currentPriorityItems = new producedItemHolder[concurrent];
                countAtPriority = 0;
                currentNeededItems = new neededItemHolder[50];
                for (int j = 0; j < concurrent; j++)
                {
                    if(producedItemList[j].productionPriority == i)
                    {
                        currentPriorityItems[countAtPriority] = producedItemList[j];
                        countAtPriority++;
                    }
                }

                //for each item in priority list
                for(int j = 0; j < countAtPriority; j++)
                {
                    //for each item needed for the current item
                    for (int k = 0; k < currentPriorityItems[j].item.itemList.Length; k++)
                    {
                        //for each item in needed item list for production item
                        for (int m = 0; m < currentPriorityItems[j].item.itemList.Length; m++)
                        {
                            //for each existing needed item entry
                            for (int l = 0; l < currentNeededItems.Length; l++)
                            {
                                if (currentNeededItems[l].item == currentPriorityItems[j].item.itemList[m].item)
                                {
                                    currentNeededItems[l].count += currentPriorityItems[j].item.itemList[m].needed * currentPriorityItems[j].targetPerSpan;
                                }
                                if (currentNeededItems[l].item == null)
                                {
                                    currentNeededItems[l].item = currentPriorityItems[j].item.itemList[m].item;
                                    currentNeededItems[l].count = currentPriorityItems[j].item.itemList[m].needed * currentPriorityItems[j].targetPerSpan;
                                }
                            }
                        }
                    }
                }
                leftAfterProd = containedItemList;
                bool canProduceAll = true;
                //for each item in factory
                for (int j = 0; j < containedItemList.Length; j++)
                {
                    //for each item we need
                    for(int k = 0; k < currentNeededItems.Length; k++)
                    {
                        //subtract items from owned
                        if (containedItemList[j].item == currentNeededItems[k].item) {
                            leftAfterProd[j].count -= currentNeededItems[k].count;
                            if(currentNeededItems[k].count < 0)
                            {
                                canProduceAll = false;
                            }
                        }
                    }
                }

                produceableItems = currentPriorityItems;
                bool stillProducing = true;
                bool isProduceable = true;
                if (!canProduceAll)
                {
                    //algorithm to determine how much we can produce with materials
                    //change produceableItems to reflect the list of items we have the materials to produce

                    for(int j = 0; j < produceableItems.Length; j++)
                    {
                        produceableItems[j].targetPerSpan = 0;
                    }

                    while (stillProducing)
                    {
                        stillProducing = false;
                        for(int j = 0; j < currentPriorityItems.Length; j++)
                        {
                            isProduceable = true;
                            if (produceableItems[j].targetPerSpan == currentPriorityItems[j].targetPerSpan)
                            {
                                isProduceable = false;
                            }
                            else
                            {
                                for (int k = 0; k < currentPriorityItems[j].item.itemList.Length; k++)
                                {
                                    for (int l = 0; l < containedItemList.Length; l++)
                                    {
                                        if (containedItemList[l].item == currentPriorityItems[j].item.itemList[k].item)
                                        {
                                            if (containedItemList[l].count < currentPriorityItems[j].item.itemList[k].needed + produceableItems[j].targetPerSpan * currentPriorityItems[j].item.itemList[k].needed)
                                            {
                                                isProduceable = false;
                                            }
                                        }
                                    }
                                }
                            }
                            if (isProduceable)
                            {
                                produceableItems[j].targetPerSpan++;
                                stillProducing = true;
                            }
                        }
                    }
                }

                //figure out power needs
                for (int j = 0; j < produceableItems.Length; j++)
                {
                    neededPower += produceableItems[j].item.powerNeeded * produceableItems[j].targetPerSpan;
                }
                if (neededPower <= powerAccess)
                {
                    powerAccess -= neededPower;
                    containedItemList = leftAfterProd;
                    for (int j = 0; j < produceableItems.Length; j++)
                    {
                        producedAdded = false;
                        for (int k = 0; k < containedItemList.Length; k++)
                        {
                            if (containedItemList[k].item == produceableItems[j].item || containedItemList[k].item == null && producedAdded == false)
                            {
                                containedItemList[k].count += produceableItems[j].targetPerSpan;
                                producedAdded = true;
                            }
                        }
                    }
                }
                else
                {
                    powerPct = Convert.ToDouble(neededPower) / Convert.ToDouble(powerAccess);
                    powerAccess = 0;
                    for (int j = 0; j < produceableItems.Length; j++)
                    {
                        if (!produceableItems[j].canUseCityPower)
                        {
                            produceableItems[j].targetPerSpan = Convert.ToInt32(Convert.ToDouble(produceableItems[j].targetPerSpan) * powerPct);
                        }
                        else
                        {
                            powerFromCity += (produceableItems[j].targetPerSpan * produceableItems[j].item.powerNeeded) - Convert.ToInt32(Convert.ToDouble(produceableItems[j].targetPerSpan) * Convert.ToDouble(produceableItems[j].item.powerNeeded) * powerPct);
                        }
                    }
                    for (int j = 0; j < produceableItems.Length; j++)
                    {
                        producedAdded = false;
                        for (int k = 0; k < containedItemList.Length; k++)
                        {
                            if (containedItemList[k].item == produceableItems[j].item || containedItemList[k].item == null && producedAdded == false)
                            {
                                containedItemList[k].count += produceableItems[j].targetPerSpan;
                                producedAdded = true;
                            }
                            for(int l = 0; l < produceableItems[j].item.itemList.Length; l++)
                            {
                                if(produceableItems[j].item.itemList[l].item == containedItemList[k].item)
                                {
                                    containedItemList[k].count -= produceableItems[j].item.itemList[l].needed;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

    }
}
