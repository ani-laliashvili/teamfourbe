﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Google.OrTools.LinearSolver;

namespace EVOptimization
{
    class Program
    {
        public static void Main(string[] args)
        {
            // Initialize the solver
            Solver solver = Solver.CreateSolver("CBC_MIXED_INTEGER_PROGRAMMING");
            if (solver == null)
            {
                Console.WriteLine("Could not create solver.");
                return;
            }

            // Time slots (5 mins in a 24-hour period)
            int numTimeSlots = 24 * (60 / 5);
            int[] timeSlots = new int[numTimeSlots];
            for (int h = 0; h<numTimeSlots; h++)
            {
                timeSlots[h] = h;
            }

            // Create sample data for households, EVs, and appliances
            List<Household> households = Household.CreateHouseholds();
            List<EV> EVs = EV.CreateEVs(households);
            List<Appliance> appliances = Appliance.CreateAppliances();

            // Electricity price forecast (prices in $ per kWh)
            double[] P_price = CreatePriceForecast(numTimeSlots);

            // Call a separate method to set up and solve the optimization model
            SolveOptimization(solver, numTimeSlots, households, EVs, appliances, P_price);
        }
        static void SolveOptimization(Solver solver, int numTimeSlots, List<Household> households, List<EV> EVs, List<Appliance> appliances, double[] P_price)
        {
            // Battery degradation cost ($ per kWh)
            double DegCost = 0.05;

            // Charging and discharging efficiencies
            double Eff_charge = 0.9;
            double Eff_discharge = 0.9;

            // Charging and discharging power limits (kW)
            double P_charge_max = 11.0; // Maximum charging power
            double P_discharge_max = 7.0; // Maximum discharging power

            // Duration of each time slot (5 mins)
            double delta_t = 1.0;

            // Outage period (e.g., hours 18 to 20)
            int outageStart = 18;
            int outageEnd = 20;
            int[] OutagePeriod = new int[outageEnd - outageStart];
            for (int h = outageStart; h < outageEnd; h++)
            {
                OutagePeriod[h - outageStart] = h;
            }

            // Low-price periods (e.g., hours 0 to 6 and 22 to 23)
            bool[] LowPricePeriod = new bool[numTimeSlots];
            for (int h = 0; h < numTimeSlots; h++)
            {
                LowPricePeriod[h] = (h >= 0 && h <= 6) || (h >= 22 && h <= 23);
            }

            // Decision variables
            // P_grid[u, h]: Power drawn from the grid by household u at time h
            Dictionary<(int u, int h), Variable> P_grid = new();

            // P_charge[e, h]: Charging power for EV e at time h
            Dictionary<(int e, int h), Variable> P_charge = new();

            // P_discharge[e, h]: Discharging power from EV e at time h
            Dictionary<(int e, int h), Variable> P_discharge = new();

            // SoC[e, h]: State of Charge of EV e at time h
            Dictionary<(int e, int h), Variable> SoC = new();

            // y[e, h]: Binary variable indicating if EV e is available for discharging at time h
            Dictionary<(int e, int h), Variable> y = new();

            // z[e, h]: Binary variable indicating charging (1) or discharging (0) mode
            Dictionary<(int e, int h), Variable> z = new();

            // P_peak: Auxiliary variable representing peak EV charging load
            Variable P_peak = solver.MakeNumVar(0, double.PositiveInfinity, "P_peak");

            // P_EV[h]: Total EV charging power in the community at time h
            Dictionary<int, Variable> P_EV = new();

            // Initialize variables
            foreach (Household household in households)
            {
                for (int h = 0; h < numTimeSlots; h++)
                {
                    P_grid[(household.Id, h)] = solver.MakeNumVar(0, double.PositiveInfinity, $"P_grid_{household.Id}_{h}");
                }
            }

            foreach (EV ev in EVs)
            {
                for (int h = 0; h < numTimeSlots; h++)
                {
                    P_charge[(ev.Id, h)] = solver.MakeNumVar(0, P_charge_max, $"P_charge_{ev.Id}_{h}");
                    P_discharge[(ev.Id, h)] = solver.MakeNumVar(0, P_discharge_max, $"P_discharge_{ev.Id}_{h}");
                    SoC[(ev.Id, h)] = solver.MakeNumVar(ev.SoCMin * ev.BatteryCapacity, ev.SoCMax * ev.BatteryCapacity, $"SoC_{ev.Id}_{h}");
                    y[(ev.Id, h)] = solver.MakeIntVar(0, 1, $"y_{ev.Id}_{h}");
                    z[(ev.Id, h)] = solver.MakeIntVar(0, 1, $"z_{ev.Id}_{h}");
                }
            }

            for (int h = 0; h < numTimeSlots; h++)
            {
                P_EV[h] = solver.MakeNumVar(0, double.PositiveInfinity, $"P_EV_{h}");
            }


            // Power balance for each household
            foreach (Household household in households)
            {
                for (int h = 0; h < numTimeSlots; h++)
                {
                    // Initialize the left-hand side as the grid power for the household
                    LinearExpr lhs = P_grid[(household.Id, h)];

                    // Add terms for each EV in the household, handling charging and discharging
                    foreach (int evId in household.EVs)
                    {
                        // lhs += P_discharge * Eff_discharge - P_charge / Eff_charge
                        lhs += P_discharge[(evId, h)] * Eff_discharge;
                        lhs -= P_charge[(evId, h)] / Eff_charge;
                    }

                    // Get the total appliance power for this household at time h
                    double totalAppliancePower = Appliance.GetTotalAppliancePower(household, appliances, h, OutagePeriod);

                    // Add the constraint to the solver
                    solver.Add(lhs == totalAppliancePower);
                }
            }


            // SoC dynamics for each EV
            foreach (EV ev in EVs)
            {
                for (int h = 0; h < numTimeSlots; h++)
                {
                    if (h == 0)
                    {
                        solver.Add(SoC[(ev.Id, h)] == ev.SoCInitial * ev.BatteryCapacity + (P_charge[(ev.Id, h)] * Eff_charge - P_discharge[(ev.Id, h)] / Eff_discharge) * delta_t);
                    }
                    else
                    {
                        solver.Add(SoC[(ev.Id, h)] == SoC[(ev.Id, h - 1)] + (P_charge[(ev.Id, h)] * Eff_charge - P_discharge[(ev.Id, h)] / Eff_discharge) * delta_t);
                    }
                }
            }

            // Charging/discharging mutual exclusivity
            foreach (EV ev in EVs)
            {
                for (int h = 0; h < numTimeSlots; h++)
                {
                    solver.Add(P_charge[(ev.Id, h)] <= P_charge_max * z[(ev.Id, h)]);
                    solver.Add(P_discharge[(ev.Id, h)] <= P_discharge_max * (1 - z[(ev.Id, h)]));
                }
            }

            // User overrides and discharging availability
            foreach (EV ev in EVs)
            {
                for (int h = 0; h < numTimeSlots; h++)
                {
                    int availability = ev.IsAvailableForDischarge ? 1 : 0;
                    solver.Add(y[(ev.Id, h)] <= availability);
                    solver.Add(P_discharge[(ev.Id, h)] <= P_discharge_max * y[(ev.Id, h)]);
                }
            }

            // Emergency preparedness
            foreach (EV ev in EVs)
            {
                 solver.Add(SoC[(ev.Id, outageStart - 1)] >= ev.SoCEmergencyLevel * ev.BatteryCapacity);
            }

            // User acceptance of cost-saving recommendations
            foreach (EV ev in EVs)
            {
                Household household = households.Find(h => h.Id == ev.HouseholdId);
                if (household.AcceptsRecommendations)
                {
                    for (int h = 0; h < numTimeSlots; h++)
                    {
                        int lowPrice = LowPricePeriod[h] ? 1 : 0;
                        solver.Add(P_charge[(ev.Id, h)] <= P_charge_max * lowPrice);
                    }
                }
            }

            // Define P_EV[h] and P_peak constraints
            for (int h = 0; h < numTimeSlots; h++)
            {
                // Build total EV charging expression using LinearExpr
                LinearExpr totalEVCharging = new LinearExpr();

                foreach (EV ev in EVs)
                {
                    totalEVCharging += P_charge[(ev.Id, h)];
                }

                // Add constraint for total EV charging power at time slot h
                solver.Add(P_EV[h] == totalEVCharging);

                // Add constraint for the peak charging power
                solver.Add(P_EV[h] <= P_peak);
            }


            // Objective function
            // Weights for the multi-objective optimization
            double alpha = 1.0; // Weight for total cost
            double beta = 0.5;  // Weight for peak load (adjust as needed)

            Objective objective = solver.Objective();

            // Total cost components
            foreach (Household household in households)
            {
                for (int h = 0; h < numTimeSlots; h++)
                {
                    objective.SetCoefficient(P_grid[(household.Id, h)], alpha * P_price[h]);
                }
            }

            foreach (EV ev in EVs)
            {
                for (int h = 0; h < numTimeSlots; h++)
                {
                    objective.SetCoefficient(P_charge[(ev.Id, h)], alpha * DegCost);
                    objective.SetCoefficient(P_discharge[(ev.Id, h)], alpha * DegCost);
                }
            }

            // Peak load component
            objective.SetCoefficient(P_peak, beta);

            objective.SetMinimization();

            // Solve the model
            Solver.ResultStatus resultStatus = solver.Solve();

            if (resultStatus == Solver.ResultStatus.OPTIMAL)
            {
                Console.WriteLine("Optimal solution found!");

                // Display results
                Console.WriteLine($"\nTotal Cost: ${solver.Objective().Value():0.00}");
                Console.WriteLine($"Peak EV Charging Load: {P_peak.SolutionValue():0.00} kW");

                foreach (EV ev in EVs)
                {
                    Console.WriteLine($"\nEV {ev.Id} (Household {ev.HouseholdId}):");
                    Console.WriteLine("Hour\tSoC (kWh)\tP_charge (kW)\tP_discharge (kW)");
                    for (int h = 0; h < numTimeSlots; h++)
                    {
                        Console.WriteLine($"{h}\t{SoC[(ev.Id, h)].SolutionValue():0.00}\t\t{P_charge[(ev.Id, h)].SolutionValue():0.00}\t\t{P_discharge[(ev.Id, h)].SolutionValue():0.00}");
                    }
                }

                // Display community EV charging load profile
                Console.WriteLine("\nCommunity EV Charging Load Profile:");
                Console.WriteLine("Hour\tP_EV (kW)");
                for (int h = 0; h < numTimeSlots; h++)
                {
                    Console.WriteLine($"{h}\t{P_EV[h].SolutionValue():0.00}");
                }
            }
            else
            {
                Console.WriteLine("No optimal solution found.");
            }
        }
        
        // Function to create sample electricity price forecast
        static double[] CreatePriceForecast(int numTimeSlots)
        {
            double[] P_price = new double[numTimeSlots];

            for (int h = 0; h < numTimeSlots; h++)
            {
                if ((h >= 0 && h <= 6) || (h >= 22 && h <= 23))
                {
                    P_price[h] = 0.10; // Low price period
                }
                else if (h >= 17 && h <= 20)
                {
                    P_price[h] = 0.30; // Peak price period
                }
                else
                {
                    P_price[h] = 0.20; // Regular price
                }
            }

            return P_price;
        }

        
    }
}
