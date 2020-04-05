using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicComposerLibrary;
using MusicComposerLibrary.Storage;
using MusicComposerLibrary.Structures;
using System.Collections.Generic;

namespace MusicComposer.UnitTest
{
    [TestClass]
    public class SongPartGeneratorTest
    {
        private SongData GetSongData(int beatsPerMeasure)
        {
            return new SongData()
            {
                PartLength = 4,
                BeatsPerMeasure = beatsPerMeasure,
                BeatUnit = NoteDuration.NoteLengthType.Quarter,
                SongName = "Test",
                Name = "Test",
                ScaleKey = "C",
                Values = WeightedRandom.GetRandomValues()
            };
        }
        private SongPartGenerator _songPartGenerator;
        public SongPartGeneratorTest()
        {
            SongData songData = GetSongData(4);
            _songPartGenerator = new SongPartGenerator(songData);
        }
        [TestMethod]
        public void RemainingBeforeStop_4of4fromStart_025()
        {
            decimal result = _songPartGenerator.RemainingBeforeStop();
            Assert.AreEqual(0.25M, result);
        }
        [TestMethod]
        public void RemainingBeforeStop_4of4from225_025()
        {
            _songPartGenerator.TotalLength = 2.25M;
            decimal result = _songPartGenerator.RemainingBeforeStop();
            Assert.AreEqual(0.25M, result);
        }
        [TestMethod]
        public void RemainingBeforeStop_4of4from3125_0125()
        {
            _songPartGenerator.TotalLength = 2.125M;
            decimal result = _songPartGenerator.RemainingBeforeStop();
            Assert.AreEqual(0.125M, result);
        }
        [TestMethod]
        public void RemainingBeforeStop_3of4from0875_0125()
        {
            SongData songData = GetSongData(3);
            SongPartGenerator songPartGenerator = new SongPartGenerator(songData);
            songPartGenerator.TotalLength = 0.875M;
            decimal result = songPartGenerator.RemainingBeforeStop();
            Assert.AreEqual(0.125M, result);
        }
        [TestMethod]
        public void RemainingBeforeNextMeasure_4of4fromStart_1()
        {
            decimal result = _songPartGenerator.RemainingBeforeNextMeasure();
            Assert.AreEqual(1M, result);
        }
        [TestMethod]
        public void RemainingBeforeNextMeasure_4of4from025_075()
        {
            _songPartGenerator.TotalLength = 0.25M;
            decimal result = _songPartGenerator.RemainingBeforeNextMeasure();
            Assert.AreEqual(0.75M, result);
        }
        [TestMethod]
        public void RemainingBeforeNextMeasure_4of4from2125_0875()
        {
            _songPartGenerator.TotalLength = 2.125M;
            decimal result = _songPartGenerator.RemainingBeforeNextMeasure();
            Assert.AreEqual(0.875M, result);
        }
        [TestMethod]
        public void RemainingBeforeNextMeasure_3of4from325_05()
        {
            SongPartGenerator songPartGenerator = new SongPartGenerator(GetSongData(3));
            songPartGenerator.TotalLength = 2.5M;
            decimal result = songPartGenerator.RemainingBeforeNextMeasure();
            Assert.AreEqual(0.5M, result);
        }

        [TestMethod]
        public void AddNote_HalfNoteAt0125_TiedEigthQuarterEight()
        {
            _songPartGenerator.TotalLength = 0.125M;
            _songPartGenerator.AddNote(0.5M, false);
            Assert.AreEqual(3, _songPartGenerator.NoteDurations.Count);
            Assert.AreEqual(0.125M, _songPartGenerator.NoteDurations[0].Duration);
            Assert.AreEqual(NoteDuration.TieType.Start, _songPartGenerator.NoteDurations[0].Tie);
            Assert.AreEqual(0.25M, _songPartGenerator.NoteDurations[1].Duration);
            Assert.AreEqual(NoteDuration.TieType.Both, _songPartGenerator.NoteDurations[1].Tie);
            Assert.AreEqual(0.125M, _songPartGenerator.NoteDurations[2].Duration);
            Assert.AreEqual(NoteDuration.TieType.End, _songPartGenerator.NoteDurations[2].Tie);
        }

        [TestMethod]
        public void AddNote_HalfNoteAt00625M_TiedEightDotQuarter16th()
        {
            ///stop 1 0.0625 + (0.1875) = 0.25 remain 0.5 - 0.1875 = 0.3125. stop 2 0.25 Remain 0.3125 - 0.25 = 0,625
            ///stop 3 0.0625
            _songPartGenerator.TotalLength = 0.0625M;
            _songPartGenerator.AddNote(0.5M, false);
            Assert.AreEqual(3, _songPartGenerator.NoteDurations.Count);
            Assert.AreEqual(0.1875M, _songPartGenerator.NoteDurations[0].Duration);
            Assert.AreEqual(NoteDuration.TieType.Start, _songPartGenerator.NoteDurations[0].Tie);
            Assert.AreEqual(0.25M, _songPartGenerator.NoteDurations[1].Duration);
            Assert.AreEqual(NoteDuration.TieType.Both, _songPartGenerator.NoteDurations[1].Tie);
            Assert.AreEqual(0.0625M, _songPartGenerator.NoteDurations[2].Duration);
            Assert.AreEqual(NoteDuration.TieType.End, _songPartGenerator.NoteDurations[2].Tie);
        }

        private void Compare(List<Note> first, List<Note> second)
        {
            Assert.AreEqual(first.Count, second.Count);
            for (int loop = 0; loop < first.Count; loop++)
            {
                Note firstNote = first[loop];
                Note secondNote = second[loop];
                Assert.AreEqual(firstNote.Dot, secondNote.Dot);
                Assert.AreEqual(firstNote.Duration, secondNote.Duration);
                Assert.AreEqual(firstNote.LastOfMeasure, secondNote.LastOfMeasure);
                Assert.AreEqual(firstNote.Name, secondNote.Name);
                Assert.AreEqual(firstNote.NoteLength, secondNote.NoteLength);
                Assert.AreEqual(firstNote.Offset, secondNote.Offset);
                Assert.AreEqual(firstNote.Tie, secondNote.Tie);
            }
        }

        [TestMethod]
        public void CreateSongPart_TwoInvocationsDifferentInstance_SameResults()
        {
            SongData songData = GetSongData(4);
            SongPartGenerator generator = new SongPartGenerator(songData);
            List<Note> firstResult = generator.CreateSongPart();
            generator = new SongPartGenerator(songData);
            List<Note> secondResult = generator.CreateSongPart();
            Compare(firstResult, secondResult);
        }
        [TestMethod]
        public void CreateSongPart_TwoInvocationsSameInstance_SameResults()
        {
            SongData songData = GetSongData(4);
            SongPartGenerator generator = new SongPartGenerator(songData);
            List<Note> firstResult = generator.CreateSongPart();
            List<Note> secondResult = generator.CreateSongPart();
            Compare(firstResult, secondResult);
        }
    }
}
