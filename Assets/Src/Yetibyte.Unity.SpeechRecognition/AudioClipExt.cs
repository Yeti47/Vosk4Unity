using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Yetibyte.Unity.Audio
{
    public static class AudioClipExt
    {
        public static byte[] GetWavData(this AudioClip clip, int maxSamples = int.MaxValue, int offsetSamples = 0)
        {
            if (clip == null)
                throw new ArgumentNullException(nameof(clip));

            float[] samples = new float[clip.samples * clip.channels];

            if (!clip.GetData(samples, offsetSamples))
                return null;

            return samples.Take(maxSamples).SelectMany(s => BitConverter.GetBytes((short)(s * short.MaxValue))).ToArray();

        }
    }
}
