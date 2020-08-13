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
    public class MelodyGeneratorTest
    {
        private readonly WeightedRandom _weightedRandom;
        private readonly Weights _weights;
        public MelodyGeneratorTest()
        {
            _weightedRandom = new WeightedRandom(WeightedRandom.GetRandomValues(10), WeightedRandom.GetRandomValues(10));
            _weights = new Weights(Storage.WeightData.GetDefaults());
        }
        public Generators.MelodyGenerator GetMelodyGenerator(string scaleKeyFullName, string melodyLowestNoteFullNameWithOctave, string melodyHighestNoteFullNameWithOctave)
        {
            return  new Generators.MelodyGenerator(
                _weightedRandom, _weights,
                new Structures.Scale(new Structures.NotePitch(scaleKeyFullName, 4), false),
                melodyLowestNoteFullNameWithOctave, melodyHighestNoteFullNameWithOctave);
        }

        [TestMethod]
        public void GetLastNotePitch_StandardC_C5()
        {
            Generators.MelodyGenerator generator = GetMelodyGenerator("C", null, null);
            Structures.NotePitch pitch = generator.GetLastNotePitch();
            Assert.AreEqual("C5", pitch.FullNameWithOctave);
        }

        [TestMethod]
        public void GetLastNotePitch_StandardB_B4()
        {
            Generators.MelodyGenerator generator = GetMelodyGenerator("B", null, null);
            Structures.NotePitch pitch = generator.GetLastNotePitch();
            Assert.AreEqual("B4", pitch.FullNameWithOctave);
        }

        [TestMethod]
        public void GetLastNotePitch_FromC3toC4KeyE_E3()
        {
            Generators.MelodyGenerator generator = GetMelodyGenerator("E", "C3", "C4");
            Structures.NotePitch pitch = generator.GetLastNotePitch();
            Assert.AreEqual("E3", pitch.FullNameWithOctave);
        }
    }
}