using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace S6_BDI_probleme
{
    internal class Bouquet
    {
        private string name;
        private string description;
        private List<string> flowers;
        private List<string> accessories;
        private double price;

        public Bouquet(string name, string description, double price, List<string> flowers, List<string> accessories)
        {
            this.name = name;
            this.description = description;
            this.price = price;
            this.flowers = flowers ?? new List<string>();
            this.accessories = accessories ?? new List<string>();
        }
        public Bouquet()
        {
            this.name = "";
            this.description = "";
            this.price = 0;
            this.flowers = new List<string> { "test", "tes t" };
            this.accessories = new List<string> { "testa", "df" };
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string Description
        {
            get => description;
            set => description = value;
        }
        public List<string> Flowers
        {
            get => flowers;
            set => flowers = value;
        }
        public string FlowersString
        {
            get => string.Join(",", flowers);
            set => flowers = value.Split(',').ToList();
        }
        
        public List<string> Accessories
        {
            get => accessories;
            set => accessories = value;
        }
        
        public string AccessoriesString
        {
            get => string.Join(",", accessories);
            set => accessories = value.Split(',').ToList();
        }

        public double Price
        {
            get => price;
            set => price = value;
        }

        public override string ToString()
        {
            return $"Name: {name}\nDescription: {description}\nflowers: {string.Join(", ", flowers)}\nAccessories: {string.Join(", ", accessories)}\nPrice: {price}";
        }
    }
}
