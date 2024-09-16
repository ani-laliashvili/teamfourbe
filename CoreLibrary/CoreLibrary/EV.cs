using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary
{
    public class EV
    {
        // Current charge of the EV in percentage (0 to 100)
        public double CurrentCharge { get; private set; }

        // Constructor to initialize the current charge
        public EV(double initialCharge)
        {
            if (initialCharge < 0 || initialCharge > 100)
            {
                throw new ArgumentException("Initial charge must be between 0 and 100.");
            }
            CurrentCharge = initialCharge;
        }

        // Method to charge the EV, increments the charge by the given amount
        public void Charge(double amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Charge amount cannot be negative.");
            }

            CurrentCharge += amount;

            if (CurrentCharge > 100)
            {
                CurrentCharge = 100; // Ensures the charge does not exceed 100%
            }
        }

        // Method to discharge the EV, decreases the charge by the given amount
        public void Discharge(double amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Discharge amount cannot be negative.");
            }

            CurrentCharge -= amount;

            if (CurrentCharge < 0)
            {
                CurrentCharge = 0; // Ensures the charge does not go below 0%
            }
        }

        // Method to send charge to another component in the home grid
        public void SendChargeTo(double amount, HomeGridComponent component)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Amount to send cannot be negative.");
            }

            if (CurrentCharge < amount)
            {
                throw new InvalidOperationException("Not enough charge to send.");
            }

            CurrentCharge -= amount;
            component.ReceiveCharge(amount);

            Console.WriteLine($"Sent {amount}% charge to {component.Name}. Remaining charge: {CurrentCharge}%.");
        }

        // Optional: Method to return the current charge
        public string GetCurrentCharge()
        {
            return CurrentCharge.ToString();
        }


    }
}
