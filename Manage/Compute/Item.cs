using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage.Compute
{
    //part can be anything from a raw material to components of a finished product
    class Item
    {
        public string partName;                        //name of part
        public string partDesc;                        //description
        public double weight;                          //weight of object.  used for storage capacity and transportation
        public bool finalproduct;
        public string requiredFacility;                //does this get made at a factory? refined at a refinery? smelted at a smeltery?
        public int powerNeeded;
        public int workNeeded;
        int neededItemTypes;
        public struct neededItemHolder                   
        {
            public Item item;                          //item needed to produce
            public int needed;                         //number of this item needed to produce
        }
        public neededItemHolder[] itemList;                  //list of items necessary to produce item

        public Item(string partName, string partDesc, double weight, bool finalProduct, string requiredFacility, int powerNeeded, int workNeeded, int neededItemTypes)
        {
            this.partName = partName;
            this.partDesc = partDesc;
            this.weight = weight;
            this.finalproduct = finalProduct;
            this.requiredFacility = requiredFacility;
            this.powerNeeded = powerNeeded;
            this.workNeeded = workNeeded;
            this.neededItemTypes = neededItemTypes;
            itemList = new neededItemHolder[neededItemTypes];
        }
        bool placed;
        public void addNeededItem(Item item, int count)
        {
            neededItemHolder needed;
            needed.item = item;
            needed.needed = count;
            placed = false;
            for(int i = 0; i < neededItemTypes; i++)
            {
                if(itemList[i].item == null && !placed)
                {
                    itemList[i].item = item;
                    itemList[i].needed = count;
                }
            }
        }
    }
}
