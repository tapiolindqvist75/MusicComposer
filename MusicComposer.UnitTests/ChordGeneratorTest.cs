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
    public class ChordGeneratorTest
    {
        public ChordGeneratorTest()
        {
        }
        public Generators.ChordGenerator GetChordGenerator(string scaleKeyFullName, bool major)
        {
            return new Generators.ChordGenerator(
                Storage.WeightData.GetDefaults(),
                new Structures.Scale(new Structures.NotePitch(scaleKeyFullName, 4), major));
        }

        [TestMethod]
        public void DetermineChords_CEGB_Cmajor7()
        {
            Generators.ChordGenerator generator = GetChordGenerator("C", true);
            List<Structures.Chord> chords = generator.DetermineChords(new List<Structures.Note>()
            {
                new Structures.Note(new Structures.NoteDuration(Structures.NoteDuration.NoteLengthType.Quarter, false, Structures.NoteDuration.LinkType.None), new Structures.NotePitch("C4")) { MeasureNumber = 1 },
                new Structures.Note(new Structures.NoteDuration(Structures.NoteDuration.NoteLengthType.Quarter, false, Structures.NoteDuration.LinkType.None), new Structures.NotePitch("G4")) { MeasureNumber = 1 },
                new Structures.Note(new Structures.NoteDuration(Structures.NoteDuration.NoteLengthType.Quarter, false, Structures.NoteDuration.LinkType.None), new Structures.NotePitch("E4")) { MeasureNumber = 1 },
                new Structures.Note(new Structures.NoteDuration(Structures.NoteDuration.NoteLengthType.Quarter, false, Structures.NoteDuration.LinkType.None), new Structures.NotePitch("B4")) { MeasureNumber = 1 }
            });
            Assert.AreEqual("Cmaj7", chords[0].FullName);
        }
        [TestMethod]
        public void DetermineChords_EGB_Cmajor()
        {
            Generators.ChordGenerator generator = GetChordGenerator("C", true);
            List<Structures.Chord> chords = generator.DetermineChords(new List<Structures.Note>()
            {
                new Structures.Note(new Structures.NoteDuration(Structures.NoteDuration.NoteLengthType.Quarter, false, Structures.NoteDuration.LinkType.None), new Structures.NotePitch("C4")) { MeasureNumber = 1 },
                new Structures.Note(new Structures.NoteDuration(Structures.NoteDuration.NoteLengthType.Quarter, false, Structures.NoteDuration.LinkType.None), new Structures.NotePitch("D4")) { MeasureNumber = 1 },
                new Structures.Note(new Structures.NoteDuration(Structures.NoteDuration.NoteLengthType.Quarter, false, Structures.NoteDuration.LinkType.None), new Structures.NotePitch("G4")) { MeasureNumber = 1 },
                new Structures.Note(new Structures.NoteDuration(Structures.NoteDuration.NoteLengthType.Quarter, false, Structures.NoteDuration.LinkType.None), new Structures.NotePitch("E4")) { MeasureNumber = 1 },
            });
            Assert.AreEqual("C", chords[0].FullName);
        }
        [TestMethod]
        public void DetermineChords_AbDbCDb_Am7()
        {
            // s p  s  s p  s  s
            //F G Ab Bb C Db Eb F
            //Bb Db F Ab
            Generators.ChordGenerator generator = GetChordGenerator("F", false);
            List<Structures.Chord> chords = generator.DetermineChords(new List<Structures.Note>()
            {
                new Structures.Note(new Structures.NoteDuration(Structures.NoteDuration.NoteLengthType.Quarter, false, Structures.NoteDuration.LinkType.None), new Structures.NotePitch("Ab4")) { MeasureNumber = 1 },
                new Structures.Note(new Structures.NoteDuration(Structures.NoteDuration.NoteLengthType.Quarter, false, Structures.NoteDuration.LinkType.None), new Structures.NotePitch("Bb4")) { MeasureNumber = 1 },
                new Structures.Note(new Structures.NoteDuration(Structures.NoteDuration.NoteLengthType.Quarter, false, Structures.NoteDuration.LinkType.None), new Structures.NotePitch("F4")) { MeasureNumber = 1 },
                new Structures.Note(new Structures.NoteDuration(Structures.NoteDuration.NoteLengthType.Quarter, false, Structures.NoteDuration.LinkType.None), new Structures.NotePitch("Db4")) { MeasureNumber = 1 },
            });
            Assert.AreEqual("Bbm7", chords[0].FullName);
        }
        [TestMethod]
        public void DetermineChords_AbDbCD_Bbm()
        {
            //Chord would be Bbmmaj7, but it is not in Chord F minor degrees
            Generators.ChordGenerator generator = GetChordGenerator("F", false);
            List<Structures.Chord> chords = generator.DetermineChords(new List<Structures.Note>()
            {
                new Structures.Note(new Structures.NoteDuration(Structures.NoteDuration.NoteLengthType.Quarter, false, Structures.NoteDuration.LinkType.None), new Structures.NotePitch("Bb4")) { MeasureNumber = 1 },
                new Structures.Note(new Structures.NoteDuration(Structures.NoteDuration.NoteLengthType.Quarter, false, Structures.NoteDuration.LinkType.None), new Structures.NotePitch("B4")) { MeasureNumber = 1 },
                new Structures.Note(new Structures.NoteDuration(Structures.NoteDuration.NoteLengthType.Quarter, false, Structures.NoteDuration.LinkType.None), new Structures.NotePitch("F4")) { MeasureNumber = 1 },
                new Structures.Note(new Structures.NoteDuration(Structures.NoteDuration.NoteLengthType.Quarter, false, Structures.NoteDuration.LinkType.None), new Structures.NotePitch("Db4")) { MeasureNumber = 1 },
            });
            Assert.AreEqual("Bbm", chords[0].FullName);
        }

    }
}