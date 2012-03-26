using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monitis.Tests
{
    public static class Common
    {
        public static string GenerateRandomString(int length)
        {
            //Initiate objects & vars    Random random = new Random();
            String randomString = "a";
            int randNumber;
            Random random = new Random();
            //Loop ‘length’ times to generate a random number or character
            for (int i = 0; i < length - 1; i++)
            {
                if (random.Next(1, 3) == 1)
                    randNumber = random.Next(97, 123); //char {a-z}
                else
                    randNumber = random.Next(48, 58); //int {0-9}

                //append random char or digit to random string
                randomString = randomString + (char)randNumber;
            }
            //return the random string
            return randomString;
        }
    }
}
