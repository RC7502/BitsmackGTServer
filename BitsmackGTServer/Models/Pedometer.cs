using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsmackGTServer.Models
{
    class Pedometer
    {

        public int id { get; set; }
        public int steps { get; set; }
        public int sleep { get; set; }
        public System.DateTime trandate { get; set; }
        public double weight { get; set; }
        public double bodyfat { get; set; }
        public Nullable<System.DateTime> createddate { get; set; }
        public Nullable<System.DateTime> lastupdateddate { get; set; }
        public Nullable<int> calconsumed { get; set; }

        public void Copy(Pedometer from)
        {
            steps = from.steps;
            sleep = from.sleep;
            trandate = from.trandate;
            weight = from.weight;
            bodyfat = from.bodyfat;
            createddate = createddate ?? DateTime.UtcNow;
            lastupdateddate = DateTime.UtcNow;
            calconsumed = from.calconsumed;
        }
    }
}
