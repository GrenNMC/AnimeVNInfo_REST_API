using System.Security.Cryptography;

namespace AnimeVnInfoBackend.Utilities.Helpers
{
    public class RandomStringGenerator
    {
        private string buffer = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz";
        private string numericBuffer = "0A7a1B6b2C5c3D4d4E3e5F2f6G1g7H0h8I1i9J2j8K3k7L4l6M5m5N6n4O7o3P8p2Q9q1R8r0S7s1T6t2U5u3V4v4W3w5X2x6Y1y7Z0z8";
        private string seed = "";

        public RandomStringGenerator()
        {
            seed = buffer;
        }

        public string GetRandom(int stringLength)
        {
            var res = "";
            for (var i = 0; i < stringLength; i++)
            {
                res += seed[RandomNumberGenerator.GetInt32(seed.Length)];
            }
            return res;
        }

        public string GetRandom(int stringLength, bool hasNumber)
        {
            if (hasNumber)
            {
                seed = numericBuffer;
            }
            return GetRandom(stringLength);
        }
    }
}
