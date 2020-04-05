using System;
using System.Linq;
using System.Collections.Generic;

namespace MusicComposerLibrary
{
    public class WeightedRandom
    {
        double[] _randomValues;
        int _index;
        const int COUNT = 50;
        public WeightedRandom(double[] values)
        {
            _randomValues = values;
            _index = 0;
        }
        public void Reset()
        {
            _index = 0;
        }
        public T GetRandomKey<T>(Dictionary<T, int> weights)
        {
            int totalWeights = weights.Values.Sum();
            double randomDbl = (double)totalWeights * _randomValues[_index];
            _index++;
            if (_index >= COUNT)
                _index = 0;
            int randomInt = Convert.ToInt32(Math.Floor(randomDbl));
            foreach(T key in weights.Keys)
            {
                randomInt -= weights[key];
                if (randomInt <= 0)
                    return key;
            }
            return weights.Keys.Last();
        }
        public int GetRandomIndex(List<int> weights)
        {
            int totalWeights = weights.Sum();
            double randomDbl = (double)totalWeights * _randomValues[_index];
            _index++;
            int randomInt = Convert.ToInt32(Math.Round(randomDbl, 0));
            for(int loop=0;loop < weights.Count; loop++)
            {
                randomDbl -= weights[loop];
                if (randomDbl < 0)
                    return loop;
            }
            return weights.Count - 1;    
        }
        public bool GetRandomBool()
        {
            bool random = _randomValues[_index] < 0.5;
            _index++;
            return random;
        }
        public static double[] GetRandomValues()
        {
            double[] randomValues = new double[100];
            Random random = new Random();
            for (int loop = 0; loop < COUNT; loop++)
                randomValues[loop] = random.NextDouble();
            return randomValues;
        }
    }
}
