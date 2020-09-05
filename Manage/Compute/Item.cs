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
        string partName;                        //name of part
        string partDesc;                        //description
        double weight;                          //weight of object.  used for storage capacity and transportation
        bool finalproduct;
        string requiredFacility;                //does this get made at a factory? refined at a refinery? smelted at a smeltery?
        struct itemHolder                   
        {
            Item item;                          //item needed to produce
            int needed;                         //number of this item needed to produce
        }
        itemHolder[] itemList;                  //list of items necessary to produce item
    }
}
