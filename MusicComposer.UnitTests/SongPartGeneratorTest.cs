using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicComposerLibrary;
using MusicComposerLibrary.Storage;
using MusicComposerLibrary.Structures;
using System.Collections.Generic;

namespace MusicComposer.UnitTests
{
    [TestClass]
    public class SongPartGeneratorTest
    {
        private SongInput GetSongInput(int beatsPerMeasure, string scaleKeyFullName = "C",
            string melodyLowestNoteFullNameWithOctave = null, string melodyHighestNoteFullNameWithOctave = null)
        {
            return new SongInput()
            {
                PartLength = 4,
                BeatsPerMeasure = beatsPerMeasure,
                BeatUnit = NoteDuration.NoteLengthType.Quarter,
                SongName = "Test",
                Name = "Test",
                MelodyLowestNoteFullNameWithOctave = melodyLowestNoteFullNameWithOctave,
                MelodyHighestNoteFullNameWithOctave = melodyHighestNoteFullNameWithOctave,
                ScaleKeyFullName = scaleKeyFullName,
                DurationValues = WeightedRandom.GetRandomValues(10),
                PitchValues = WeightedRandom.GetRandomValues(10),
                WeightData = WeightData.GetDefaults(),
                Chords = true
            };
        }
        private readonly SongPartGenerator _songPartGenerator;
        public SongPartGeneratorTest()
        {
            SongInput songData = GetSongInput(4);
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
            SongInput songData = GetSongInput(3);
            SongPartGenerator songPartGenerator = new SongPartGenerator(songData)
            {
                TotalLength = 0.875M
            };
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
            SongPartGenerator songPartGenerator = new SongPartGenerator(GetSongInput(3))
            {
                TotalLength = 2.5M
            };
            decimal result = songPartGenerator.RemainingBeforeNextMeasure();
            Assert.AreEqual(0.5M, result);
        }

        [TestMethod]
        public void AddNote_HalfNoteAt0125_TiedEigthQuarterEight()
        {
            List<NoteDuration> noteDurations = new List<NoteDuration>();
            _songPartGenerator.TotalLength = 0.125M;
            _songPartGenerator.AddNote(0.5M, false, false, noteDurations);
            Assert.AreEqual(3, noteDurations.Count);
            Assert.AreEqual(0.125M, noteDurations[0].Duration);
            Assert.AreEqual(NoteDuration.LinkType.Start, noteDurations[0].Tie);
            Assert.AreEqual(0.25M, noteDurations[1].Duration);
            Assert.AreEqual(NoteDuration.LinkType.Continue, noteDurations[1].Tie);
            Assert.AreEqual(0.125M, noteDurations[2].Duration);
            Assert.AreEqual(NoteDuration.LinkType.End, noteDurations[2].Tie);
        }

        [TestMethod]
        public void AddNote_HalfNoteAt00625M_TiedEightDotQuarter16th()
        {
            List<NoteDuration> noteDurations = new List<NoteDuration>();
            ///stop 1 0.0625 + (0.1875) = 0.25 remain 0.5 - 0.1875 = 0.3125. stop 2 0.25 Remain 0.3125 - 0.25 = 0,625
            ///stop 3 0.0625
            _songPartGenerator.TotalLength = 0.0625M;
            _songPartGenerator.AddNote(0.5M, false, false, noteDurations);
            Assert.AreEqual(3, noteDurations.Count);
            Assert.AreEqual(0.1875M, noteDurations[0].Duration);
            Assert.AreEqual(NoteDuration.LinkType.Start, noteDurations[0].Tie);
            Assert.AreEqual(0.25M, noteDurations[1].Duration);
            Assert.AreEqual(NoteDuration.LinkType.Continue, noteDurations[1].Tie);
            Assert.AreEqual(0.0625M, noteDurations[2].Duration);
            Assert.AreEqual(NoteDuration.LinkType.End, noteDurations[2].Tie);
        }

        private void Compare(SongOutput first, SongOutput second)
        {
            Assert.AreEqual(first.Melody.Count, second.Melody.Count);
            for (int loop = 0; loop < first.Melody.Count; loop++)
            {
                Note firstNote = first.Melody[loop];
                Note secondNote = second.Melody[loop];
                Assert.AreEqual(firstNote.Dot, secondNote.Dot);
                Assert.AreEqual(firstNote.Duration, secondNote.Duration);
                Assert.AreEqual(firstNote.LastOfMeasure, secondNote.LastOfMeasure);
                Assert.AreEqual(firstNote.NoteLength, secondNote.NoteLength);
                Assert.AreEqual(firstNote.Tie, secondNote.Tie);
                Assert.AreEqual(firstNote.Pitch.Name, secondNote.Pitch.Name);
                Assert.AreEqual(firstNote.Pitch.Offset, secondNote.Pitch.Offset);
                Assert.AreEqual(firstNote.Pitch.MidiNumber, secondNote.Pitch.MidiNumber);
            }
            if (first.Chords == null)
            {
                Assert.IsNull(second.Chords);
            }
            else
            {
                Assert.AreEqual(first.Chords.Count, second.Chords.Count);
                for (int loop = 0; loop < first.Chords.Count; loop++)
                {
                    Chord firstChord = first.Chords[loop];
                    Chord secondChord = second.Chords[loop];
                    Assert.AreEqual(firstChord.NotePitches.Count, secondChord.NotePitches.Count);
                    Assert.AreEqual(firstChord.FullName, secondChord.FullName);
                    Assert.AreEqual(firstChord.Classification, secondChord.Classification);
                    for (int noteLoop = 0; noteLoop < firstChord.NotePitches.Count; noteLoop++)
                    {
                        NotePitch firstNote = firstChord.NotePitches[noteLoop];
                        NotePitch secondNote = secondChord.NotePitches[noteLoop];
                        Assert.AreEqual(firstNote.FullName, secondNote.FullName);
                        Assert.AreEqual(firstNote.MidiNumber, secondNote.MidiNumber);
                        Assert.AreEqual(firstNote.Name, secondNote.Name);
                        Assert.AreEqual(firstNote.Octave, secondNote.Octave);
                        Assert.AreEqual(firstNote.Offset, secondNote.Offset);
                    }
                }
            }
        }

        [TestMethod]
        public void GetLastNotePitch_StandardC_C5()
        {
            SongInput songData = GetSongInput(4);
            SongPartGenerator generator = new SongPartGenerator(songData);
            NotePitch pitch = generator.GetLastNotePitch();
            Assert.AreEqual("C5", pitch.FullNameWithOctave);
        }

        [TestMethod]
        public void GetLastNotePitch_StandardB_B4()
        {
            SongInput songData = GetSongInput(4, scaleKeyFullName: "B");
            SongPartGenerator generator = new SongPartGenerator(songData);
            NotePitch pitch = generator.GetLastNotePitch();
            Assert.AreEqual("B4", pitch.FullNameWithOctave);
        }

        [TestMethod]
        public void GetLastNotePitch_FromC3toC4KeyE_E3()
        {
            SongInput songData = GetSongInput(4, scaleKeyFullName: "E", melodyLowestNoteFullNameWithOctave: "C3", melodyHighestNoteFullNameWithOctave: "C4");
            SongPartGenerator generator = new SongPartGenerator(songData);
            NotePitch pitch = generator.GetLastNotePitch();
            Assert.AreEqual("E3", pitch.FullNameWithOctave);
        }

        [TestMethod]
        public void CreateSongPart_TwoInvocationsDifferentInstance_SameResults()
        {
            SongInput songData = GetSongInput(4);
            SongPartGenerator generator = new SongPartGenerator(songData);
            SongOutput firstResult = generator.CreateSongPart();
            generator = new SongPartGenerator(songData);
            SongOutput secondResult = generator.CreateSongPart();
            Compare(firstResult, secondResult);
        }
        [TestMethod]
        public void CreateSongPart_TwoInvocationsSameInstance_SameResults()
        {
            SongInput songData = GetSongInput(4);
            SongPartGenerator generator = new SongPartGenerator(songData);
            SongOutput firstResult = generator.CreateSongPart();
            SongOutput secondResult = generator.CreateSongPart();
            Compare(firstResult, secondResult);
        }

        [TestMethod]
        public void DetermineChords_CEGB_Cmajor7()
        {
            SongInput songInput = GetSongInput(4);
            songInput.Major = true;
            SongPartGenerator generator = new SongPartGenerator(songInput);
            List<Chord> chords = generator.DetermineChords(new List<Note>()
            {
                new Note(new NoteDuration(NoteDuration.NoteLengthType.Quarter, false, NoteDuration.LinkType.None), new NotePitch("C4")) { MeasureNumber = 1 },
                new Note(new NoteDuration(NoteDuration.NoteLengthType.Quarter, false, NoteDuration.LinkType.None), new NotePitch("G4")) { MeasureNumber = 1 },
                new Note(new NoteDuration(NoteDuration.NoteLengthType.Quarter, false, NoteDuration.LinkType.None), new NotePitch("E4")) { MeasureNumber = 1 },
                new Note(new NoteDuration(NoteDuration.NoteLengthType.Quarter, false, NoteDuration.LinkType.None), new NotePitch("B4")) { MeasureNumber = 1 }
            });
            Assert.AreEqual("Cmaj7", chords[0].FullName);
        }
        [TestMethod]
        public void DetermineChords_EGB_Cmajor()
        {
            SongInput songInput = GetSongInput(4);
            songInput.Major = true;
            SongPartGenerator generator = new SongPartGenerator(songInput);
            List<Chord> chords = generator.DetermineChords(new List<Note>()
            {
                new Note(new NoteDuration(NoteDuration.NoteLengthType.Quarter, false, NoteDuration.LinkType.None), new NotePitch("C4")) { MeasureNumber = 1 },
                new Note(new NoteDuration(NoteDuration.NoteLengthType.Quarter, false, NoteDuration.LinkType.None), new NotePitch("D4")) { MeasureNumber = 1 },
                new Note(new NoteDuration(NoteDuration.NoteLengthType.Quarter, false, NoteDuration.LinkType.None), new NotePitch("G4")) { MeasureNumber = 1 },
                new Note(new NoteDuration(NoteDuration.NoteLengthType.Quarter, false, NoteDuration.LinkType.None), new NotePitch("E4")) { MeasureNumber = 1 },
            });
            Assert.AreEqual("C", chords[0].FullName);
        }
        [TestMethod]
        public void DetermineChords_AbDbCDb_Am7()
        {
            // s p  s  s p  s  s
            //F G Ab Bb C Db Eb F
            //Bb Db F Ab
            SongInput songInput = GetSongInput(4);
            songInput.Major = false;
            songInput.ScaleKeyFullName = "F";
            SongPartGenerator generator = new SongPartGenerator(songInput);
            List<Chord> chords = generator.DetermineChords(new List<Note>()
            {
                new Note(new NoteDuration(NoteDuration.NoteLengthType.Quarter, false, NoteDuration.LinkType.None), new NotePitch("Ab4")) { MeasureNumber = 1 },
                new Note(new NoteDuration(NoteDuration.NoteLengthType.Quarter, false, NoteDuration.LinkType.None), new NotePitch("Bb4")) { MeasureNumber = 1 },
                new Note(new NoteDuration(NoteDuration.NoteLengthType.Quarter, false, NoteDuration.LinkType.None), new NotePitch("F4")) { MeasureNumber = 1 },
                new Note(new NoteDuration(NoteDuration.NoteLengthType.Quarter, false, NoteDuration.LinkType.None), new NotePitch("Db4")) { MeasureNumber = 1 },
            });
            Assert.AreEqual("Bbm7", chords[0].FullName);
        }
        [TestMethod]
        public void DetermineChords_AbDbCD_Bbm()
        {
            //Chord would be Bbmmaj7, but it is not in Chord F minor degrees
            SongInput songInput = GetSongInput(4);
            songInput.Major = false;
            songInput.ScaleKeyFullName = "F";
            SongPartGenerator generator = new SongPartGenerator(songInput);
            List<Chord> chords = generator.DetermineChords(new List<Note>()
            {
                new Note(new NoteDuration(NoteDuration.NoteLengthType.Quarter, false, NoteDuration.LinkType.None), new NotePitch("Bb4")) { MeasureNumber = 1 },
                new Note(new NoteDuration(NoteDuration.NoteLengthType.Quarter, false, NoteDuration.LinkType.None), new NotePitch("B4")) { MeasureNumber = 1 },
                new Note(new NoteDuration(NoteDuration.NoteLengthType.Quarter, false, NoteDuration.LinkType.None), new NotePitch("F4")) { MeasureNumber = 1 },
                new Note(new NoteDuration(NoteDuration.NoteLengthType.Quarter, false, NoteDuration.LinkType.None), new NotePitch("Db4")) { MeasureNumber = 1 },
            });
            Assert.AreEqual("Bbm", chords[0].FullName);
        }
    }
}
