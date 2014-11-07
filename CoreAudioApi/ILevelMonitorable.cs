using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreAudioApi
{
    static public class ILevelMonitorableExtensions
    {
        public static void GetChannelLevels(this ILevelMonitorable ilm, float[] levels)
        {
            int count = ilm.GetChannelCount();
            if (levels.Length < count)
                throw new ArgumentException("The levels array is too small to contain all channnels.");

            for (int i = 0; i < count; i++)
            {
                levels[i] = ilm.GetChannelLevel(i);
            }
        }

        public static float[] GetChannelLevels(this ILevelMonitorable ilm)
        {
            float[] levels = new float[ilm.GetChannelCount()];
            GetChannelLevels(ilm, levels);

            return levels;
        }
    }

    public interface ILevelMonitorable
    {
        int GetChannelCount();
        float GetMasterLevel();
        float GetChannelLevel(int channelIndex);
    }
}
