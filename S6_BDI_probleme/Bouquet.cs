using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace S6_BDI_probleme
{
    public class Bouquet
    {
        private int id;
        private string description;
        private List<int> flowers;
        private List<int> accessories;
        private double price;

        public Bouquet(int id, string description, double price, List<int> flowers, List<int> accessories)
        {
            this.id = id;
            this.description = description;
            this.price = price;
            this.flowers = flowers ?? new List<int>();
            this.accessories = accessories ?? new List<int>();
        }
        public Bouquet()
        {
            this.description = "";
            this.flowers = new List<int> { };
            this.accessories = new List<int> { };
        }
        public int Id
        {
            get => id;
            set => id = value;
        }

        public string Description
        {
            get => description;
            set => description = value;
        }
        public List<int> Flowers
        {
            get => flowers;
            set => flowers = value;
        }
        public string FlowersString
        {
            get => string.Join(",", flowers);
        }
        
        public List<int> Accessories
        {
            get => accessories;
            set => accessories = value;
        }
        
        public string AccessoriesString
        {
            get => string.Join(",", accessories);
        }

        public double Price
        {
            get => price;
            set => price = value;
        }
    }
}
