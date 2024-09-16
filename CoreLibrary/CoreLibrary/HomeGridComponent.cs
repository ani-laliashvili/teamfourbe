using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary
{
    // Class to represent a component of the home grid
    public class HomeGridComponent
    {
        public string Name { get; private set; }
        public double ChargeReceived { get; private set; }
        public string Running { get; set; }

        // Constructor to initialize the component
        public HomeGridComponent(string name)
        {
            Name = name;
            ChargeReceived = 0;
        }

        // Method to receive charge from the EV
        public string ReceiveCharge(double amount)
        {
            ChargeReceived += amount;
            return $"{Name} received {amount}% charge. Total charge received: {ChargeReceived}%.";
        }
    }
}
