using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Volkswagen.Dashboard.Repository
{
    public class CarModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DateRelease { get; set; }

        public static List<CarModel> GetCars()
        {
            return new List<CarModel>()
            {
        
            };
        }
    }
}
