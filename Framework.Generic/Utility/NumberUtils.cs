using System;
using System.Security.Cryptography;

namespace Framework.Generic.Utility
{
    public static class NumberUtils
    {
        /// <summary>
        /// Generates a random number between min and max, exclusive.
        /// </summary>
        /// <param name="min">The minimum value in the range that can be returned</param>
        /// <param name="max">The maximum value in the range that can be returned</param>
        /// <returns>Returns a random number between min and max, exclusive</returns>
        public static int GenerateRandomNumber(int min, int max)
        {
            var rand = new RNGCryptoServiceProvider();
            var scale = uint.MaxValue;

            while (scale == uint.MaxValue)
            {
                var byteArr = new byte[4];
                rand.GetBytes(byteArr);

                scale = BitConverter.ToUInt32(byteArr, 0);
            }

            return (int)(min + ((max - min + 1) * (scale / (double)uint.MaxValue)));
        }
    }
}
