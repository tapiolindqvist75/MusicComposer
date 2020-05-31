using System;
using System.Collections.Generic;
using System.Text;

namespace MusicComposerLibrary.Storage
{
    public class WeightData
    {
        public WeightData()
        {
        }
        public int LengthHalfWeight { get; set; }
        public int LengthQuarterWeight { get; set; }
        public int LengthEightWeight { get; set; }
        public int LengthSixteenthWeight { get; set; }
        public int MiddleNonScale { get; set; }
        public int MiddleScale { get; set; }
        public int MiddleTonic_1 { get; set; }
        public int MiddleMediant_3 { get; set; }
        public int MiddleDominant_5 { get; set; }
        public int LastNonScale { get; set; }
        public int LastScale { get; set; }
        public int LastTonic_1 { get; set; }
        public int LastMediant_3 { get; set; }
        public int LastDominant_5 { get; set; }
        public int Distance_0 { get; set; }
        public int Distance_1 { get; set; }
        public int Distance_2 { get; set; }
        public int Distance_3 { get; set; }
        public int Distance_4 { get; set; }
        public int Distance_5 { get; set; }
        public int Distance_6 { get; set; }
        public int Distance_7 { get; set; }
        public int ChordTonic_1 { get; set; }
        public int ChordDominant_3 { get; set; }
        public int ChordMediant_5 { get; set; }
        public int ChordLedging_7 { get; set; }
        public int ChordScale { get; set; }
        public int ChordNonScale { get; set; }
        public int ChordDetermination_ChordNote { get; set; }
        public int ChordDetermination_ScaleNote { get; set; }
        public int ChordDetermination_NonScaleNote { get; set; }
        public static WeightData GetDefaults()
        {
            WeightData weightData = new WeightData()
            {
                LengthHalfWeight = 1,
                LengthQuarterWeight = 5,
                LengthEightWeight = 4,
                LengthSixteenthWeight = 0,
                MiddleNonScale = 1,
                MiddleScale = 50,
                MiddleTonic_1 = 50,
                MiddleMediant_3 = 50,
                MiddleDominant_5 = 50,
                LastNonScale = 1,
                LastScale = 50,
                LastTonic_1 = 200,
                LastMediant_3 = 100,
                LastDominant_5 = 150,
                Distance_0 = 5,
                Distance_1 = 10,
                Distance_2 = 8,
                Distance_3 = 4,
                Distance_4 = 2,
                Distance_5 = 1,
                Distance_6 = 1,
                Distance_7 = 1,
                ChordDetermination_ChordNote = 20,
                ChordDetermination_ScaleNote = 5,
                ChordDetermination_NonScaleNote = 0
            };
            return weightData;
        }
    }
}
