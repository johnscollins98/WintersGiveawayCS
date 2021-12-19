using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WintersGiveaway
{
    public class RandomNumberGenerator : IRandom
    {
        private readonly Random random = new Random();

        public int Next(int n)
        {
            return random.Next(n);
        }
    }
}
