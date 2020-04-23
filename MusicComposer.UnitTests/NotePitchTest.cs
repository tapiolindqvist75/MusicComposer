using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using MusicComposerLibrary.Structures;

namespace MusicComposer.UnitTests
{
    [TestClass]
    public class NotePitchTest
    {
        [TestMethod]
        public void MidiNumber_C_60()
        {
            NotePitch notePitch = new NotePitch("C");
            Assert.AreEqual(60, notePitch.MidiNumber);
        }
        [TestMethod]
        public void MidiNumber_Cis_61()
        {
            NotePitch notePitch = new NotePitch("C#");
            Assert.AreEqual(61, notePitch.MidiNumber);
        }
        [TestMethod]
        public void MidiNumber_Des_61()
        {
            NotePitch notePitch = new NotePitch("Db");
            Assert.AreEqual(61, notePitch.MidiNumber);
        }
        [TestMethod]
        public void MidiNumber_D_62()
        {
            NotePitch notePitch = new NotePitch("D");
            Assert.AreEqual(62, notePitch.MidiNumber);
        }
        [TestMethod]
        public void MidiNumber_Dis_63()
        {
            NotePitch notePitch = new NotePitch("D#");
            Assert.AreEqual(63, notePitch.MidiNumber);
        }
        [TestMethod]
        public void MidiNumber_Es_63()
        {
            NotePitch notePitch = new NotePitch("Eb");
            Assert.AreEqual(63, notePitch.MidiNumber);
        }
        [TestMethod]
        public void MidiNumber_E_64()
        {
            NotePitch notePitch = new NotePitch("E");
            Assert.AreEqual(64, notePitch.MidiNumber);
        }
        [TestMethod]
        public void MidiNumber_F_65()
        {
            NotePitch notePitch = new NotePitch("F");
            Assert.AreEqual(65, notePitch.MidiNumber);
        }
        [TestMethod]
        public void MidiNumber_Fis_66()
        {
            NotePitch notePitch = new NotePitch("F#");
            Assert.AreEqual(66, notePitch.MidiNumber);
        }
        [TestMethod]
        public void MidiNumber_Ges_66()
        {
            NotePitch notePitch = new NotePitch("Gb");
            Assert.AreEqual(66, notePitch.MidiNumber);
        }
        [TestMethod]
        public void MidiNumber_G_67()
        {
            NotePitch notePitch = new NotePitch("G");
            Assert.AreEqual(67, notePitch.MidiNumber);
        }
        [TestMethod]
        public void MidiNumber_Gis_68()
        {
            NotePitch notePitch = new NotePitch("G#");
            Assert.AreEqual(68, notePitch.MidiNumber);
        }
        [TestMethod]
        public void MidiNumber_As_68()
        {
            NotePitch notePitch = new NotePitch("Ab");
            Assert.AreEqual(68, notePitch.MidiNumber);
        }
        [TestMethod]
        public void MidiNumber_A_69()
        {
            NotePitch notePitch = new NotePitch("A");
            Assert.AreEqual(69, notePitch.MidiNumber);
        }
        [TestMethod]
        public void MidiNumber_Ais_70()
        {
            NotePitch notePitch = new NotePitch("A#");
            Assert.AreEqual(70, notePitch.MidiNumber);
        }
        [TestMethod]
        public void MidiNumber_Bb_70()
        {
            NotePitch notePitch = new NotePitch("Bb");
            Assert.AreEqual(70, notePitch.MidiNumber);
        }
        [TestMethod]
        public void MidiNumber_B_71()
        {
            NotePitch notePitch = new NotePitch("B");
            Assert.AreEqual(71, notePitch.MidiNumber);
        }
        [TestMethod]
        public void MidiNumber_GisisString_69()
        {
            NotePitch notePitch = new NotePitch("G##");
            Assert.AreEqual(69, notePitch.MidiNumber);
        }
        public void MidiNumber_GesesString_65()
        {
            NotePitch notePitch = new NotePitch("Gbb");
            Assert.AreEqual(69, notePitch.MidiNumber);
        }

        [TestMethod]
        public void MidiNumber_C3_48()
        {
            NotePitch notePitch = new NotePitch('C', 0, 3);
            Assert.AreEqual(48, notePitch.MidiNumber);
        }
        [TestMethod]
        public void MidiNumber_GOffset1Octave5_80()
        {
            NotePitch notePitch = new NotePitch('G', 1, 5);
            Assert.AreEqual(80, notePitch.MidiNumber);
        }
        [TestMethod]
        public void MidiNumber_BOffsetMinus1Octave2_46()
        {
            NotePitch notePitch = new NotePitch('B', -1, 2);
            Assert.AreEqual(46, notePitch.MidiNumber);
        }
        [TestMethod]
        public void NameOffsetFullNameOctave_FromMidi79Sharp_G0G5()
        {
            NotePitch notePitch = new NotePitch(79, true);
            Assert.AreEqual(79, notePitch.MidiNumber);
            Assert.AreEqual('G', notePitch.Name);
            Assert.AreEqual(0, notePitch.Offset);
            Assert.AreEqual("G", notePitch.FullName);
            Assert.AreEqual(5, notePitch.Octave);
        }
        [TestMethod]
        public void NameOffsetFullNameOctave_FromMidi58Sharp_A1Asharp3()
        {
            NotePitch notePitch = new NotePitch(58, true);
            Assert.AreEqual(58, notePitch.MidiNumber);
            Assert.AreEqual('A', notePitch.Name);
            Assert.AreEqual(1, notePitch.Offset);
            Assert.AreEqual("A#", notePitch.FullName);
            Assert.AreEqual(3, notePitch.Octave);
        }
        [TestMethod]
        public void NameOffsetFullNameOctave_FromMidi94Flat_Bminu1Bb6()
        {
            NotePitch notePitch = new NotePitch(94, false);
            Assert.AreEqual(94, notePitch.MidiNumber);
            Assert.AreEqual('B', notePitch.Name);
            Assert.AreEqual(-1, notePitch.Offset);
            Assert.AreEqual("Bb", notePitch.FullName);
            Assert.AreEqual(6, notePitch.Octave);
        }
    }
}
