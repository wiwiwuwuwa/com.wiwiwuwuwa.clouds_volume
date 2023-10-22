using Unity.Mathematics;

namespace Wiwiwuwuwa.Utilities
{
    public static class MathUtils
    {
        // ------------------------------------------------

        public static int3 Wrap(int3 val, int3 min, int3 max)
        {
            for (var i = 0; i < 3; i++)
            {
                val[i] = Wrap(val[i], min[i], max[i]);
            }

            return val;
        }

        public static int Wrap(int val, int min, int max)
        {
            (min, max) = min <= max ? (min, max) : (max, min);

            var dif = max - min + 1;
            if (dif <= 0)
            {
                return min;
            }

            var mod = (val - min) % dif;
            if (mod < 0)
            {
                mod += dif;
            }

            return mod + min;
        }

        // ------------------------------------------------
    }
}
