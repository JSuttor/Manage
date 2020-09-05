using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Manage.Compute
{
    class Factory
    {
        struct containedItemHolder
        {
            Item item;
            int count;
        }
        struct producedItemHolder
        {
            Item item;
            int target;
        }

        int storageCapacity;                //inherent storage capacity of the factory. can be expanded with adjacent warehouse
        int heldObjects;                    //storage capacity filled

        int factLevel;                      //factories can be upgraded for more production and concurrent assembly lines
        int powerNeed;                      //units of power needed to run at full capacity
        int productionCap;                  //integer representing this factory's current ability to produce
        int concurrent;                     //possible number of concurrent production lines
        containedItemHolder[] containedItemList;        //items stored in the factory
        producedItemHolder[] producedItemList;          //items that are being produced here.  will be of length concurrent
    }
}
