using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using MusicComposerLibrary;
using MusicComposerLibrary.Storage;
using MusicComposerLibrary.Structures;

namespace MusicComposer.UnitTests
{
    [TestClass]
    public class WeightsTest
    {
        [TestMethod]
        public void GetWeights_TwoOctaveCmajorNonScaleNotes0_CisDisFisGisAis0()
        {
            Weights weights = new MusicComposerLibrary.Weights(WeightData.GetDefaults());
            Scale scale = new Scale(new NotePitch("C", 4), true);
            List<ScaleNotePitch> scaleNotes = scale.GetChromaticNotes(new NotePitch("C", 3), new NotePitch("C", 5));
            Dictionary<NotePitch, int> weightValues = weights.GetWeights(scaleNotes, false, true);
            foreach(var weightValue in weightValues)
            {
                string fullname = weightValue.Key.FullName;
                if (fullname == "C#" || fullname == "D#" || fullname == "F#" || fullname == "G#" || fullname == "A#")
                    Assert.AreEqual(weightValue.Value, 0);
                else
                    Assert.AreNotEqual(weightValue.Value, 0);
            }
        }

        [TestMethod]
        public void GetWeights_EmajorTonicWeight123LastAndMiddle_E123()
        {
            WeightData weightData = new WeightData() { LastTonic_1 = 123, MiddleTonic_1 = 123, LastNonScale = 222, MiddleNonScale = 222 };
            Weights weights = new MusicComposerLibrary.Weights(weightData);
            Scale scale = new Scale(new NotePitch("E", 4), true);
            List<ScaleNotePitch> scaleNotes = scale.GetChromaticNotes(new NotePitch("E", 3), new NotePitch("E", 5));
            Dictionary<NotePitch, int> weightValuesFirst = weights.GetWeights(scaleNotes, false, true);
            Dictionary<NotePitch, int> weightValuesLast = weights.GetWeights(scaleNotes, true, false);
            foreach (var weightValue in weightValuesLast)
            {
                string fullname = weightValue.Key.FullName;
                if (fullname == "F" || fullname == "G" || fullname == "A#" || fullname == "C" || fullname == "D")
                    Assert.AreEqual(weightValue.Value, 222);
                else if (fullname == "E")
                    Assert.AreEqual(weightValue.Value, 123);
                else
                    Assert.AreEqual(weightValue.Value, 0);
            }
            foreach (var weightValue in weightValuesFirst)
            {
                string fullname = weightValue.Key.FullName;
                if (fullname == "E")
                    Assert.AreEqual(weightValue.Value, 123);
                else
                    Assert.AreEqual(weightValue.Value, 0);
            }
        }
    }
}
