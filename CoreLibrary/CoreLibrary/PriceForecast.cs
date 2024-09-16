namespace CoreLibrary
{
    public class PriceForecast
    {
        public static double[] CreatePriceForecast(int numTimeSlots)
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