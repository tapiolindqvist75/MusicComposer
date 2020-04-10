using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicComposerLibrary.Midi;
using Struct = MusicComposerLibrary.Structures;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;
using System.Linq;

namespace MusicComposer.UnitTests
{
    [TestClass]
    public class MidiConverterTest
    {
        [TestMethod]
        public void DecimalToDuration_FullNote_Result1823()
        {
            int duration = MidiConverter.DecimalToDuration(1M);
            Assert.AreEqual(1823, duration);
        }
        [TestMethod]
        public void DecimalToDuration_HalfNote_Result911()
        {
            int duration = MidiConverter.DecimalToDuration(0.5M);
            Assert.AreEqual(911, duration);
        }
        [TestMethod]
        public void DecimalToDuration_QuarterNote_Result455()
        {
            int duration = MidiConverter.DecimalToDuration(0.25M);
            Assert.AreEqual(455, duration);
        }
        [TestMethod]
        public void DecimalToDuration_QuarterDotNote_Result683()
        {
            int duration = MidiConverter.DecimalToDuration(0.375M);
            Assert.AreEqual(683, duration);
        }
        [TestMethod]
        public void DecimalToDuration_EightNote_Result227()
        {
            int duration = MidiConverter.DecimalToDuration(0.125M);
            Assert.AreEqual(227, duration);
        }
        [TestMethod]
        public void DecimalToPause_FullNote_Result97()
        {
            int duration = MidiConverter.DecimalToPause(1M);
            Assert.AreEqual(97, duration);
        }
        [TestMethod]
        public void DecimalToPause_HalfNote_Result49()
        {
            int duration = MidiConverter.DecimalToPause(0.5M);
            Assert.AreEqual(49, duration);
        }
        [TestMethod]
        public void DecimalToPause_QuarterNote_Result25()
        {
            int duration = MidiConverter.DecimalToPause(0.25M);
            Assert.AreEqual(25, duration);
        }
        [TestMethod]
        public void DecimalToPause_QuarterDotNote_Result37()
        {
            int duration = MidiConverter.DecimalToPause(0.375M);
            Assert.AreEqual(37, duration);
        }
        [TestMethod]
        public void DecimalToPause_EightNote_Result13()
        {
            int duration = MidiConverter.DecimalToPause(0.125M);
            Assert.AreEqual(13, duration);
        }
        [TestMethod]
        public void NoteToMidi_C_60()
        {
            byte note = MidiConverter.NoteToMidi('C', 0);
            Assert.AreEqual(60, note);
        }
        [TestMethod]
        public void NoteToMidi_Cis_61()
        {
            byte note = MidiConverter.NoteToMidi('C', 1);
            Assert.AreEqual(61, note);
        }
        [TestMethod]
        public void NoteToMidi_Des_61()
        {
            byte note = MidiConverter.NoteToMidi('D', -1);
            Assert.AreEqual(61, note);
        }
        [TestMethod]
        public void NoteToMidi_D_62()
        {
            byte note = MidiConverter.NoteToMidi('D', 0);
            Assert.AreEqual(62, note);
        }
        [TestMethod]
        public void NoteToMidi_Dis_63()
        {
            byte note = MidiConverter.NoteToMidi('D', 1);
            Assert.AreEqual(63, note);
        }
        [TestMethod]
        public void NoteToMidi_Es_63()
        {
            byte note = MidiConverter.NoteToMidi('E', -1);
            Assert.AreEqual(63, note);
        }
        [TestMethod]
        public void NoteToMidi_E_64()
        {
            byte note = MidiConverter.NoteToMidi('E', 0);
            Assert.AreEqual(64, note);
        }
        [TestMethod]
        public void NoteToMidi_F_65()
        {
            byte note = MidiConverter.NoteToMidi('F', 0);
            Assert.AreEqual(65, note);
        }
        [TestMethod]
        public void NoteToMidi_Fis_66()
        {
            byte note = MidiConverter.NoteToMidi('F', 1);
            Assert.AreEqual(66, note);
        }
        [TestMethod]
        public void NoteToMidi_Ges_66()
        {
            byte note = MidiConverter.NoteToMidi('G', -1);
            Assert.AreEqual(66, note);
        }
        [TestMethod]
        public void NoteToMidi_G_67()
        {
            byte note = MidiConverter.NoteToMidi('G', 0);
            Assert.AreEqual(67, note);
        }
        [TestMethod]
        public void NoteToMidi_Gis_68()
        {
            byte note = MidiConverter.NoteToMidi('G', 1);
            Assert.AreEqual(68, note);
        }
        [TestMethod]
        public void NoteToMidi_As_68()
        {
            byte note = MidiConverter.NoteToMidi('A', -1);
            Assert.AreEqual(68, note);
        }
        [TestMethod]
        public void NoteToMidi_A_69()
        {
            byte note = MidiConverter.NoteToMidi('A', 0);
            Assert.AreEqual(69, note);
        }
        [TestMethod]
        public void NoteToMidi_Ais_70()
        {
            byte note = MidiConverter.NoteToMidi('A', 1);
            Assert.AreEqual(70, note);
        }
        [TestMethod]
        public void NoteToMidi_Bb_70()
        {
            byte note = MidiConverter.NoteToMidi('B', -1);
            Assert.AreEqual(70, note);
        }
        [TestMethod]
        public void NoteToMidi_B_71()
        {
            byte note = MidiConverter.NoteToMidi('B', 0);
            Assert.AreEqual(71, note);
        }
        [TestMethod]
        public void NoteToMidi_Gisis_69()
        {
            byte note = MidiConverter.NoteToMidi('G', 2);
            Assert.AreEqual(69, note);
        }

        [TestMethod]
        public void AddNotesToChunks_2notes_result()
        {
            List<Struct.Note> notes = new List<Struct.Note>();
            notes.Add(new Struct.Note(
                new Struct.NoteDuration(
                    Struct.NoteDuration.NoteLengthType.Quarter,
                    false,
                    Struct.NoteDuration.TieType.None,
                    false), 'F', 1));
            notes.Add(new Struct.Note(
                new Struct.NoteDuration(
                    Struct.NoteDuration.NoteLengthType.Quarter,
                    false,
                    Struct.NoteDuration.TieType.None,
                    false), 'D', 0));
            TrackChunk trackChunk = new TrackChunk();
            EventsCollection target = trackChunk.Events;

            MidiConverter.AddNotesToChunks(target, notes);
            for(int loop=0;loop< target.Count;loop++)
            {
                NoteEvent noteEvent = target[loop] as NoteEvent;
                if (loop == 0 || loop == 2)
                    Assert.IsTrue(noteEvent is NoteOnEvent);
                else
                    Assert.IsTrue(noteEvent is NoteOffEvent);
                if (loop == 0)
                    Assert.AreEqual(66, noteEvent.NoteNumber);
                else if (loop == 1)
                    Assert.AreEqual(455, noteEvent.DeltaTime);
                else if (loop == 2)
                    Assert.AreEqual(25, noteEvent.DeltaTime);
                else
                    Assert.AreEqual(62, noteEvent.NoteNumber);
            }
        }

        [TestMethod]
        public void AddNotesToChunks_2notesTied_result()
        {
            List<Struct.Note> notes = new List<Struct.Note>();
            notes.Add(new Struct.Note(
                new Struct.NoteDuration(
                    Struct.NoteDuration.NoteLengthType.Quarter,
                    false,
                    Struct.NoteDuration.TieType.Start,
                    false), 'A', 1));
            notes.Add(new Struct.Note(
                new Struct.NoteDuration(
                    Struct.NoteDuration.NoteLengthType.Quarter,
                    false,
                    Struct.NoteDuration.TieType.End,
                    false), 'A', 1));
            TrackChunk trackChunk = new TrackChunk();
            EventsCollection target = trackChunk.Events;

            MidiConverter.AddNotesToChunks(target, notes);
            for (int loop = 0; loop < target.Count; loop++)
            {
                NoteEvent noteEvent = target[loop] as NoteEvent;
                if (loop == 0)
                    Assert.IsTrue(noteEvent is NoteOnEvent);
                else
                    Assert.IsTrue(noteEvent is NoteOffEvent);
                if (loop == 0)
                    Assert.AreEqual(70, noteEvent.NoteNumber);
                else if (loop == 1)
                    Assert.AreEqual(911, noteEvent.DeltaTime);
            }
        }
    }
}
