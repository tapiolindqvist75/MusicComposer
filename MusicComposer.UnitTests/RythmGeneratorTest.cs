using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using MusicComposerLibrary;
using Structures = MusicComposerLibrary.Structures;
using Generators = MusicComposerLibrary.Generators;
using Storage = MusicComposerLibrary.Storage;

namespace MusicComposer.UnitTests
{
    [TestClass]
    public class RythmGeneratorTest
    {
        private readonly MusicComposerLibrary.Generators.RythmGenerator _rythmGenerator;
        private readonly WeightedRandom _weightedRandom;
        private readonly Weights _weights;
        public RythmGeneratorTest()
        {
            _weightedRandom = new WeightedRandom(WeightedRandom.GetRandomValues(10), WeightedRandom.GetRandomValues(10));
            _weights = new Weights(Storage.WeightData.GetDefaults());
            _rythmGenerator = new Generators.RythmGenerator(
                _weightedRandom, _weights, 
                4, 4, Structures.NoteDuration.NoteLengthType.Quarter);
        }

        [TestMethod]
        public void RemainingBeforeStop_4of4fromStart_025()
        {
            decimal result = _rythmGenerator.RemainingBeforeStop();
            Assert.AreEqual(0.25M, result);
        }
        [TestMethod]
        public void RemainingBeforeStop_4of4from225_025()
        {
            _rythmGenerator._totalLength = 2.25M;
            decimal result = _rythmGenerator.RemainingBeforeStop();
            Assert.AreEqual(0.25M, result);
        }
        [TestMethod]
        public void RemainingBeforeStop_4of4from3125_0125()
        {
            _rythmGenerator._totalLength = 2.125M;
            decimal result = _rythmGenerator.RemainingBeforeStop();
            Assert.AreEqual(0.125M, result);
        }
        [TestMethod]
        public void RemainingBeforeStop_3of4from0875_0125()
        {
            Generators.RythmGenerator rythmGenerator = new Generators.RythmGenerator(
                _weightedRandom, _weights, 4, 3, Structures.NoteDuration.NoteLengthType.Quarter)
            {
                _totalLength = 0.875M
            };
            decimal result = rythmGenerator.RemainingBeforeStop();
            Assert.AreEqual(0.125M, result);
        }
        [TestMethod]
        public void RemainingBeforeNextMeasure_4of4fromStart_1()
        {
            decimal result = _rythmGenerator.RemainingBeforeNextMeasure();
            Assert.AreEqual(1M, result);
        }
        [TestMethod]
        public void RemainingBeforeNextMeasure_4of4from025_075()
        {
            _rythmGenerator._totalLength = 0.25M;
            decimal result = _rythmGenerator.RemainingBeforeNextMeasure();
            Assert.AreEqual(0.75M, result);
        }
        [TestMethod]
        public void RemainingBeforeNextMeasure_4of4from2125_0875()
        {
            _rythmGenerator._totalLength = 2.125M;
            decimal result = _rythmGenerator.RemainingBeforeNextMeasure();
            Assert.AreEqual(0.875M, result);
        }
        [TestMethod]
        public void RemainingBeforeNextMeasure_3of4from325_05()
        {
            Generators.RythmGenerator rythmGenerator = new Generators.RythmGenerator(
                _weightedRandom, _weights, 4, 3, Structures.NoteDuration.NoteLengthType.Quarter)
            {
                _totalLength = 2.5M
            };
            decimal result = rythmGenerator.RemainingBeforeNextMeasure();
            Assert.AreEqual(0.5M, result);
        }

        [TestMethod]
        public void AddNote_HalfNoteAt0125_TiedEigthQuarterEight()
        {
            List<MusicComposerLibrary.Structures.NoteDuration> noteDurations = new List<MusicComposerLibrary.Structures.NoteDuration>();
            _rythmGenerator._totalLength = 0.125M;
            _rythmGenerator._noteDurations = noteDurations;
            _rythmGenerator.AddNote(0.5M, false, false);
            Assert.AreEqual(3, noteDurations.Count);
            Assert.AreEqual(0.125M, noteDurations[0].Duration);
            Assert.AreEqual(MusicComposerLibrary.Structures.NoteDuration.LinkType.Start, noteDurations[0].Tie);
            Assert.AreEqual(0.25M, noteDurations[1].Duration);
            Assert.AreEqual(MusicComposerLibrary.Structures.NoteDuration.LinkType.Continue, noteDurations[1].Tie);
            Assert.AreEqual(0.125M, noteDurations[2].Duration);
            Assert.AreEqual(MusicComposerLibrary.Structures.NoteDuration.LinkType.End, noteDurations[2].Tie);
        }
        [TestMethod]
        public void AddNote_HalfNoteAt00625M_TiedEightDotQuarter16th()
        {
            List<Structures.NoteDuration> noteDurations = new List<Structures.NoteDuration>();
            ///stop 1 0.0625 + (0.1875) = 0.25 remain 0.5 - 0.1875 = 0.3125. stop 2 0.25 Remain 0.3125 - 0.25 = 0,625
            ///stop 3 0.0625
            _rythmGenerator._totalLength = 0.0625M;
            _rythmGenerator._noteDurations = noteDurations;
            _rythmGenerator.AddNote(0.5M, false, false);
            Assert.AreEqual(3, noteDurations.Count);
            Assert.AreEqual(0.1875M, noteDurations[0].Duration);
            Assert.AreEqual(Structures.NoteDuration.LinkType.Start, noteDurations[0].Tie);
            Assert.AreEqual(0.25M, noteDurations[1].Duration);
            Assert.AreEqual(Structures.NoteDuration.LinkType.Continue, noteDurations[1].Tie);
            Assert.AreEqual(0.0625M, noteDurations[2].Duration);
            Assert.AreEqual(Structures.NoteDuration.LinkType.End, noteDurations[2].Tie);
        }
    }
}