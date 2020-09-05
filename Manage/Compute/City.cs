using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manage.Compute
{
    class City
    {
        int population = 2000000;
        int wealth = 80;                    //wealth factor out of 100
        //percent male and female
        double pctFemale = .55;
        double pctMale = .45;
        //percent age groups
        double pct0to18 = .2;
        double pct19to40 = .25;
        double pct41to65 = .30;
        double pct65plus = .25;
        //percent wealth class
        double pctUpperClass = .15;
        double pctMiddleClass = .65;
        double pctLowerClass = .20;
        //resources
        double powerCost = 5;               //cost per unit of power for buildings near this city
        //stats
        double companySentiment = .6;            //the average customer's thoughts on the user's company
        double marketSentiment = .8;             //the customer's willingness to spend money based on the current market and outlook
    }
}
