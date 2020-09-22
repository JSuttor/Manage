using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Xna.Framework;
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
    class Factory : Building
    {
        //int storageCapacity;                              //inherent storage capacity of the factory. can be expanded with adjacent warehouse
        //int heldObjects;                                  //storage capacity filled

        int concurrent;                                     //number of concurrent production lines


        public Factory(int factLevel, Vector2 position)
        {
            this.buildingLevel = factLevel;
            if(factLevel == 1)
            {
                maxConcurrent = 3;
                workCapacity = 1000;
            }
            containedItemList = new containedItemHolder[20];
            producedItemList = new producedItemHolder[20];
            concurrent = 0;
            this.position = position;

        }


        public override void addContainedItem(Item item, int count)
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
        public override int subtractContainedItem(Item item, int count)
        {
            int subtracted = 0;
            for (int i = 0; i < containedItemList.Length; i++)
            {
                if (containedItemList[i].item == item)
                {
                    if (containedItemList[i].count >= count)
                    {
                        containedItemList[i].count -= count;
                        subtracted = count;
                    }
                    else
                    {
                        subtracted = containedItemList[i].count;
                        containedItemList[i].count = 0;
                    }
                }
            }
            return subtracted;
        }
        public override bool addProduction(Item item, int targetPerSpan, int priority)
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
                        producedItemList[i].productionPriority = priority;
                        producedItemList[i].canUseCityPower = false;
                        added = true;
                        concurrent++;
                    }
                }
                
            }
            return added;
        }
        public override void subtractProduction(Item item, int toSubtract)
        {
            for (int i = 0; i < maxConcurrent; i++)
            {
                if (producedItemList[i].item == item)
                {
                    if(producedItemList[i].targetPerSpan > toSubtract)
                    {
                        producedItemList[i].targetPerSpan -= toSubtract;
                    }
                    else
                    {
                        producedItemList[i].targetPerSpan = 0;
                    }
                    
                }
            }
        }

        public override void removeProduction(Item item)
        {
            for(int i = 0; i < maxConcurrent; i++)
            {
                if(producedItemList[i].item == item)
                {
                    item = null;
                    concurrent--;
                }
            }
            for(int i = 0; i < maxConcurrent; i++)
            {
                if(producedItemList[i].item == null && i < (maxConcurrent - 1))
                {
                    producedItemList[i] = producedItemList[i++];
                    producedItemList[i++].item = null;
                }
            }
        }

        public override bool produce(int powerAccess)
        {
            producedItemHolder[] currentPriorityItems;
            producedItemHolder[] produceableItems;
            neededItemHolder[] currentNeededItems;
            containedItemHolder[] leftAfterProd;
            int countAtPriority;
            double powerPct;
            double workPct;
            int powerFromCity = 0;
            int neededPower = 0;
            bool producedAdded;
            int workAccess = workCapacity;
            for (int i = 1; i <= 10; i++)
            {
                currentPriorityItems = new producedItemHolder[concurrent];
                countAtPriority = 0;
                currentNeededItems = new neededItemHolder[50];
                for (int j = 0; j < concurrent; j++)
                {
                    if (producedItemList[j].productionPriority == i)
                    {
                        currentPriorityItems[countAtPriority] = producedItemList[j];
                        countAtPriority++;
                    }
                }
                bool addedItem;
                if (currentPriorityItems[0].item != null)
                {
                    //for each item in priority list
                    for (int j = 0; j < countAtPriority; j++)
                    {
                        //for each item needed for the current item
                        for (int k = 0; k < currentPriorityItems[j].item.itemList.Length; k++)
                        {
                            addedItem = false;
                            //for each existing needed item entry
                            for (int l = 0; l < currentNeededItems.Length; l++)
                            {
                                if (currentNeededItems[l].item == currentPriorityItems[j].item.itemList[k].item && !addedItem)
                                {
                                    currentNeededItems[l].count += currentPriorityItems[j].item.itemList[k].needed * currentPriorityItems[j].targetPerSpan;
                                    addedItem = true;
                                }

                                if (currentNeededItems[l].item == null && !addedItem)
                                {
                                    currentNeededItems[l].item = currentPriorityItems[j].item.itemList[k].item;
                                    currentNeededItems[l].count = currentPriorityItems[j].item.itemList[k].needed * currentPriorityItems[j].targetPerSpan;
                                    addedItem = true;
                                }

                            }
                        }
                    }
                    leftAfterProd = (containedItemHolder[])containedItemList.Clone();
                    bool canProduceAll = true;
                    //for each item in factory
                    for (int j = 0; j < containedItemList.Length; j++)
                    {
                        //for each item we need
                        for (int k = 0; k < currentNeededItems.Length; k++)
                        {
                            //subtract items from owned
                            if (containedItemList[j].item == currentNeededItems[k].item)
                            {
                                if (leftAfterProd[j].count - currentNeededItems[k].count < 0)
                                {
                                    canProduceAll = false;
                                }
                                else
                                {
                                    leftAfterProd[j].count -= currentNeededItems[k].count;
                                }
                            }
                        }
                    }

                    produceableItems = (producedItemHolder[])currentPriorityItems.Clone();
                    bool stillProducing = true;
                    bool isProduceable;
                    if (!canProduceAll)
                    {
                        //algorithm to determine how much we can produce with materials
                        //change produceableItems to reflect the list of items we have the materials to produce

                        for (int j = 0; j < produceableItems.Length; j++)
                        {
                            produceableItems[j].targetPerSpan = 0;
                        }

                        while (stillProducing)
                        {
                            stillProducing = false;
                            for (int j = 0; j < currentPriorityItems.Length; j++)
                            {
                                isProduceable = true;
                                if (produceableItems[j].targetPerSpan == currentPriorityItems[j].targetPerSpan)
                                {
                                    isProduceable = false;
                                }
                                else
                                {
                                    //for each current item type
                                    for (int k = 0; k < currentPriorityItems[j].item.itemList.Length; k++)
                                    {
                                        //for each owned material type
                                        for (int l = 0; l < containedItemList.Length; l++)
                                        {
                                            //for each item needed for current item type
                                            if (containedItemList[l].item == currentPriorityItems[j].item.itemList[k].item)
                                            {
                                                int currentCount = 0;
                                                for(int m = 0; m < produceableItems.Length; m++) {
                                                    for(int n = 0; n < produceableItems[m].item.itemList.Length; n++)
                                                    {
                                                        if(containedItemList[l].item == produceableItems[m].item.itemList[n].item)
                                                        {
                                                            currentCount += produceableItems[m].targetPerSpan * produceableItems[m].item.itemList[n].needed;
                                                        }
                                                    }
                                                }
                                                if (containedItemList[l].count < currentPriorityItems[j].item.itemList[k].needed + currentCount)
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
                    int neededWork = 0;

                    //figure out work needs
                    for (int j = 0; j < produceableItems.Length; j++)
                    {
                        if(produceableItems[j].item != null)
                            neededWork += produceableItems[j].item.workNeeded * produceableItems[j].targetPerSpan;
                    }
                    if (neededWork <= workAccess)
                    {
                        workAccess -= neededWork;
                    }
                    else
                    {
                        workPct = Convert.ToDouble(neededWork) / Convert.ToDouble(workAccess);
                        workAccess = 0;
                        for (int j = 0; j < produceableItems.Length; j++)
                        {
                            produceableItems[j].targetPerSpan = Convert.ToInt32(Convert.ToDouble(produceableItems[j].targetPerSpan) * workPct);
                        }
                    }

                    //figure out power needs
                    for (int j = 0; j < produceableItems.Length; j++)
                    {
                        if (produceableItems[j].item != null)
                            neededPower += produceableItems[j].item.powerNeeded * produceableItems[j].targetPerSpan;
                    }
                    if (neededPower <= powerAccess)
                    {
                        powerAccess -= neededPower;
                    }
                    else
                    {
                        powerPct = Convert.ToDouble(powerAccess) / Convert.ToDouble(neededPower);
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
                                powerAccess = 0;
                            }
                        }
                    }

                    //produce possible items
                    for (int j = 0; j < produceableItems.Length; j++)
                    {
                        producedAdded = false;
                        for (int k = 0; k < containedItemList.Length; k++)
                        {
                            if (containedItemList[k].item == produceableItems[j].item || containedItemList[k].item == null && producedAdded == false)
                            {
                                containedItemList[k].count += produceableItems[j].targetPerSpan;
                                containedItemList[k].item = produceableItems[j].item;
                                producedAdded = true;
                            }
                            if (produceableItems[j].item != null)
                            {
                                for (int l = 0; l < produceableItems[j].item.itemList.Length; l++)
                                {
                                    if (produceableItems[j].item.itemList[l].item == containedItemList[k].item)
                                    {
                                        containedItemList[k].count -= produceableItems[j].item.itemList[l].needed * produceableItems[j].targetPerSpan;
                                    }
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
