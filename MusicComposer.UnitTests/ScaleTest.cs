using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicComposerLibrary.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace MusicComposer.UnitTests
{
    [TestClass]
    public class ScaleTest
    {
        [TestMethod]
        public void GetScaleNote_ScaleDMajorFsharp_Mediant()
        {
            Scale scale = new Scale(new NotePitch("D"), true);
            ScaleNotePitch scaleNotePitch = scale.GetScaleNote(new NotePitch("F#"));
            Assert.AreEqual(NoteScaleType.Mediant, scaleNotePitch.ScaleType);
        }
        [TestMethod]
        public void GetDegreePitch_EMinorAll_Ok()
        {
            Scale scale = new Scale(new NotePitch("E"), false);
            Assert.AreEqual("E", scale.GetDegreePitch(1).FullName);
            Assert.AreEqual("F#", scale.GetDegreePitch(2).FullName);
            Assert.AreEqual("G", scale.GetDegreePitch(3).FullName);
            Assert.AreEqual("A", scale.GetDegreePitch(4).FullName);
            Assert.AreEqual("B", scale.GetDegreePitch(5).FullName);
            Assert.AreEqual("C", scale.GetDegreePitch(6).FullName);
            Assert.AreEqual("D", scale.GetDegreePitch(7).FullName);
            Assert.AreEqual("E", scale.GetDegreePitch(8).FullName);

        }
        [TestMethod]
        public void GetScaleNote_TwoOctavesCMajor_Ok()
        {
            Scale scale = new Scale(new NotePitch("C"), true);
            for(int midiLoop = 48; midiLoop <= 72; midiLoop++)
            {
                ScaleNotePitch scaleNotePitch = scale.GetScaleNote(new NotePitch(midiLoop, true));
                switch (midiLoop)
                {
                    case 48: case 60: case 72:
                        Assert.AreEqual(NoteScaleType.Tonic, scaleNotePitch.ScaleType);
                        break;
                    case 52: case 64:
                        Assert.AreEqual(NoteScaleType.Mediant, scaleNotePitch.ScaleType);
                        break;
                    case 55: case 67:
                        Assert.AreEqual(NoteScaleType.Dominant, scaleNotePitch.ScaleType);
                        break;
                    case 50: case 53: case 57: case 59: 
                    case 62: case 65: case 69: case 71:
                        Assert.AreEqual(NoteScaleType.ScaleNote, scaleNotePitch.ScaleType);
                        break;
                    default:
                        Assert.AreEqual(NoteScaleType.NonScaleNote, scaleNotePitch.ScaleType);
                        break;
                }
            }
        }
        [TestMethod]
        public void GetDegreeTriad_3rdDegreeCMajor_CEG()
        {
            Scale scale = new Scale(new NotePitch("A"), false);
            Chord chord = scale.GetDegreeTriad(3);
            Assert.IsTrue(new NotePitch("C").IsSameIgnoreOctave(chord.NotePitches[0]));
            Assert.IsTrue(new NotePitch("E").IsSameIgnoreOctave(chord.NotePitches[1]));
            Assert.IsTrue(new NotePitch("G").IsSameIgnoreOctave(chord.NotePitches[2]));
            Assert.AreEqual("C", chord.FullName);
        }
        [TestMethod]
        public void GetDegreeSeventhChord_EMinor4thDegree_ACEG()
        {
            Scale scale = new Scale(new NotePitch("E"), false);
            Chord chord = scale.GetDegreeSeventhChord(4);
            Assert.IsTrue(new NotePitch("A").IsSameIgnoreOctave(chord.NotePitches[0]));
            Assert.IsTrue(new NotePitch("C").IsSameIgnoreOctave(chord.NotePitches[1]));
            Assert.IsTrue(new NotePitch("E").IsSameIgnoreOctave(chord.NotePitches[2]));
            Assert.IsTrue(new NotePitch("G").IsSameIgnoreOctave(chord.NotePitches[3]));
            Assert.AreEqual("Am7", chord.FullName);
        }
        [TestMethod]
        public void GetDegreeSeventhChord_AbMajor7thDegree_GBbDbF()
        {
            Scale scale = new Scale(new NotePitch("Ab"), true);
            Chord chord = scale.GetDegreeSeventhChord(7);
            Assert.IsTrue(new NotePitch("G").IsSameIgnoreOctave(chord.NotePitches[0]));
            Assert.IsTrue(new NotePitch("Bb").IsSameIgnoreOctave(chord.NotePitches[1]));
            Assert.IsTrue(new NotePitch("Db").IsSameIgnoreOctave(chord.NotePitches[2]));
            Assert.IsTrue(new NotePitch("F").IsSameIgnoreOctave(chord.NotePitches[3]));
            Assert.AreEqual("Gm7(b5)", chord.FullName);
        }

        [TestMethod]
        public void GetDegreeTriad_AMinor1stDegree_A3C4E4()
        {
            Scale scale = new Scale(new NotePitch("A"), false);
            Chord chord = scale.GetDegreeTriad(1, 3);
            Assert.AreEqual(0, new NotePitch("A", 3).CompareTo(chord.NotePitches[0]));
            Assert.AreEqual(0, new NotePitch("C", 4).CompareTo(chord.NotePitches[1]));
            Assert.AreEqual(0, new NotePitch("E", 4).CompareTo(chord.NotePitches[2]));
        }
        [TestMethod]
        public void GetDegreeTriad_DMajor5stDegree_A3Cis4E4()
        {
            Scale scale = new Scale(new NotePitch("D"), true);
            Chord chord = scale.GetDegreeTriad(5, 3);
            Assert.AreEqual(0, new NotePitch("A", 3).CompareTo(chord.NotePitches[0]));
            Assert.AreEqual(0, new NotePitch("C#", 4).CompareTo(chord.NotePitches[1]));
            Assert.AreEqual(0, new NotePitch("E", 4).CompareTo(chord.NotePitches[2]));
        }
    }
}
