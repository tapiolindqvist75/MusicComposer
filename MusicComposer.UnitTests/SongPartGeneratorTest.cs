using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicComposerLibrary;
using MusicComposerLibrary.Structures;

namespace MusicComposer.UnitTest
{
    [TestClass]
    public class SongPartGeneratorTest
    {
        [TestMethod]
        public void RemainingBeforeStop_4of4fromStart_025()
        {
            SongPartGenerator songPartGenerator = new SongPartGenerator(4, 4, NoteDuration.NoteLengthType.Quarter);
            decimal result = songPartGenerator.RemainingBeforeStop();
            Assert.AreEqual(0.25M, result);
        }
        [TestMethod]
        public void RemainingBeforeStop_4of4from225_025()
        {
            SongPartGenerator songPartGenerator = new SongPartGenerator(4, 4, NoteDuration.NoteLengthType.Quarter);
            songPartGenerator.TotalLength = 2.25M;
            decimal result = songPartGenerator.RemainingBeforeStop();
            Assert.AreEqual(0.25M, result);
        }
        [TestMethod]
        public void RemainingBeforeStop_4of4from3125_0125()
        {
            SongPartGenerator songPartGenerator = new SongPartGenerator(4, 4, NoteDuration.NoteLengthType.Quarter);
            songPartGenerator.TotalLength = 2.125M;
            decimal result = songPartGenerator.RemainingBeforeStop();
            Assert.AreEqual(0.125M, result);
        }
        [TestMethod]
        public void RemainingBeforeStop_3of4from0875_0125()
        {
            SongPartGenerator songPartGenerator = new SongPartGenerator(4, 3, NoteDuration.NoteLengthType.Quarter);
            songPartGenerator.TotalLength = 0.875M;
            decimal result = songPartGenerator.RemainingBeforeStop();
            Assert.AreEqual(0.125M, result);
        }
        [TestMethod]
        public void RemainingBeforeNextMeasure_4of4fromStart_1()
        {
            SongPartGenerator songPartGenerator = new SongPartGenerator(4, 4, NoteDuration.NoteLengthType.Quarter);
            decimal result = songPartGenerator.RemainingBeforeNextMeasure();
            Assert.AreEqual(1M, result);
        }
        [TestMethod]
        public void RemainingBeforeNextMeasure_4of4from025_075()
        {
            SongPartGenerator songPartGenerator = new SongPartGenerator(4, 4, NoteDuration.NoteLengthType.Quarter);
            songPartGenerator.TotalLength = 0.25M;
            decimal result = songPartGenerator.RemainingBeforeNextMeasure();
            Assert.AreEqual(0.75M, result);
        }
        [TestMethod]
        public void RemainingBeforeNextMeasure_4of4from2125_0875()
        {
            SongPartGenerator songPartGenerator = new SongPartGenerator(4, 4, NoteDuration.NoteLengthType.Quarter);
            songPartGenerator.TotalLength = 2.125M;
            decimal result = songPartGenerator.RemainingBeforeNextMeasure();
            Assert.AreEqual(0.875M, result);
        }
        [TestMethod]
        public void RemainingBeforeNextMeasure_3of4from325_05()
        {
            SongPartGenerator songPartGenerator = new SongPartGenerator(4, 3, NoteDuration.NoteLengthType.Quarter);
            songPartGenerator.TotalLength = 2.5M;
            decimal result = songPartGenerator.RemainingBeforeNextMeasure();
            Assert.AreEqual(0.5M, result);
        }

        [TestMethod]
        public void AddNote_HalfNoteAt0125_TiedEigthQuarterEight()
        {
            SongPartGenerator songPartGenerator = new SongPartGenerator(4, 4, NoteDuration.NoteLengthType.Quarter);
            songPartGenerator.TotalLength = 0.125M;
            songPartGenerator.AddNote(0.5M, false);
            Assert.AreEqual(3, songPartGenerator.NoteDurations.Count);
            Assert.AreEqual(0.125M, songPartGenerator.NoteDurations[0].Duration);
            Assert.AreEqual(NoteDuration.TieType.Start, songPartGenerator.NoteDurations[0].Tie);
            Assert.AreEqual(0.25M, songPartGenerator.NoteDurations[1].Duration);
            Assert.AreEqual(NoteDuration.TieType.Both, songPartGenerator.NoteDurations[1].Tie);
            Assert.AreEqual(0.125M, songPartGenerator.NoteDurations[2].Duration);
            Assert.AreEqual(NoteDuration.TieType.End, songPartGenerator.NoteDurations[2].Tie);
        }

        [TestMethod]
        public void AddNote_HalfNoteAt00625M_TiedEightDotQuarter16th()
        {
            ///stop 1 0.0625 + (0.1875) = 0.25 remain 0.5 - 0.1875 = 0.3125. stop 2 0.25 Remain 0.3125 - 0.25 = 0,625
            ///stop 3 0.0625
            SongPartGenerator songPartGenerator = new SongPartGenerator(4, 4, NoteDuration.NoteLengthType.Quarter);
            songPartGenerator.TotalLength = 0.0625M;
            songPartGenerator.AddNote(0.5M, false);
            Assert.AreEqual(3, songPartGenerator.NoteDurations.Count);
            Assert.AreEqual(0.1875M, songPartGenerator.NoteDurations[0].Duration);
            Assert.AreEqual(NoteDuration.TieType.Start, songPartGenerator.NoteDurations[0].Tie);
            Assert.AreEqual(0.25M, songPartGenerator.NoteDurations[1].Duration);
            Assert.AreEqual(NoteDuration.TieType.Both, songPartGenerator.NoteDurations[1].Tie);
            Assert.AreEqual(0.0625M, songPartGenerator.NoteDurations[2].Duration);
            Assert.AreEqual(NoteDuration.TieType.End, songPartGenerator.NoteDurations[2].Tie);
        }
    }
}
