using System;
using System.Linq;
using System.Collections.Generic;

namespace MusicComposerLibrary
{
    public class WeightedRandom
    {
        double[] _durationValues { get; set; }
        int _durationIndex;
        double[] _pitchValues { get; set; }
        int _pitchIndex;
        
        public WeightedRandom(double[] durationValues, double[] pitchValues)
        {
            _durationValues = durationValues;
            _pitchValues = pitchValues;
        }
        public void Reset()
        {
            _durationIndex = 0;
            _pitchIndex = 0;
        }

        public enum RandomType { Duration, Pitch }
        public T GetRandomKey<T>(RandomType randomType, Dictionary<T, int> weights)
        {
            return randomType switch
            {
                RandomType.Duration => GetRandomKey<T>(weights, ref _durationIndex, _durationValues),
                RandomType.Pitch => GetRandomKey<T>(weights, ref _pitchIndex, _pitchValues),
                _ => throw new ArgumentException("Type must be either Duration or Pitch"),
            };
        }

        private T GetRandomKey<T>(Dictionary<T, int> weights, ref int index, double[] values)
        {
            int totalWeights = weights.Values.Sum();
            double randomDbl = (double)totalWeights * values[index];
            index++;
            if (index >= values.Length)
                index = 0;
            int randomInt = Convert.ToInt32(Math.Floor(randomDbl));
            foreach(T key in weights.Keys)
            {
                randomInt -= weights[key];
                if (randomInt <= 0)
                    return key;
            }
            return weights.Keys.Last();
        }
        public static double[] GetRandomValues(int count)
        {
            double[] randomValues = new double[count];
            Random random = new Random();
            for (int loop = 0; loop < count; loop++)
                randomValues[loop] = random.NextDouble();
            return randomValues;
        }
    }
}
