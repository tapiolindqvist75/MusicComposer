using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using MusicComposerLibrary.Storage;
using MusicComposerLibrary.Structures;
using MusicComposerLibrary.Extensions;
using System;
using System.Linq;

namespace MusicComposerLibrary.MusicXml
{
    public class MusicXmlGenerator : FileGeneratorBase
    {
        public MusicXmlGenerator(SongInput songData) : base(songData) { }
        private attributes GetAttributes(int fifths, bool chord)
        {
            List<clef> clefs = new List<clef>();
            attributes target = new attributes
            {
                key = new key[] { new key() { ItemsElementName = new ItemsChoiceType8[] { ItemsChoiceType8.fifths }, Items = new object[] { fifths.ToString() } } }
            };
            if (chord)
                target.clef = new clef[] { new clef() { number = "2", sign = clefsign.F, line = "4" } };
            else
                target.clef = new clef[] { new clef() { number = "1", sign = clefsign.G, line = "2" } };
            target.time = new time[]
            {
                new time()
                {
                    ItemsElementName = new ItemsChoiceType9[] { ItemsChoiceType9.beats, ItemsChoiceType9.beattype },
                    Items = new object[]
                    {
                        Input.BeatsPerMeasure.ToString(),
                        Input.GetBeatType().ToString()
                    }
                }
            };
            return target;
        }

        private kindvalue GetKindValue(ChordClassification classification)
        {
            return classification switch
            {
                ChordClassification.Augmented => kindvalue.augmented,
                ChordClassification.AugmentedMajorSeventh => kindvalue.augmentedseventh,
                ChordClassification.Diminished => kindvalue.diminished,
                ChordClassification.DiminishedSeventh => kindvalue.diminishedseventh,
                ChordClassification.DominantSeventh => kindvalue.dominant,
                ChordClassification.HalfDiminishedSeventh => kindvalue.dominant,//Note Add degree
                ChordClassification.Major => kindvalue.major,
                ChordClassification.MajorSeventh => kindvalue.majorseventh,
                ChordClassification.Minor => kindvalue.minor,
                ChordClassification.MinorMajorSeventh => kindvalue.majorminor,
                ChordClassification.MinorSeventh => kindvalue.minorseventh,
                _ => throw new Exception("Invalid ChordClassification"),
            };
        }
        private List<scorepartwisePartMeasure> GetMeasures(int fifths, List<Note> notes)
        {
            List<scorepartwisePartMeasure> measures = new List<scorepartwisePartMeasure>();
            int noteLoop = 0;
            bool firstNote = true;
            IEnumerable<IGrouping<int, Note>> notesByMeasures = notes.GroupBy(n => n.MeasureNumber);
            foreach(IGrouping<int, Note> notesByMeasure in notesByMeasures)
            { 
                decimal measureLength = 0;
                scorepartwisePartMeasure measure = new scorepartwisePartMeasure()
                {
                    number = (notesByMeasure.Key + 1).ToString()
                };
                List<object> measureItems = new List<object>();
                if (firstNote)
                {
                    measureItems.Add(GetAttributes(fifths, false));
                    firstNote = false;
                }
                Note currentNote = notes[noteLoop];
                if (currentNote.Chord != null)
                {
                    string chordRoot = currentNote.Chord.NotePitches[0].Name.ToString();
                    int chordRootOffset = currentNote.Chord.NotePitches[0].Offset;
                    step rootStep = (step)Enum.Parse(typeof(step), chordRoot);
                    harmony harmony = new harmony
                    {
                        Items = new object[]
                        {
                            new root()
                            { rootstep = new rootstep() { Value = rootStep, text = chordRoot },
                              rootalter = new rootalter() { Value = Convert.ToDecimal(chordRootOffset) } }
                        },
                        kind = new kind[] { new kind() }
                    };
                    if (currentNote.Chord.Classification != ChordClassification.Major)
                        harmony.kind[0].text = currentNote.Chord.GetClassificationString();
                    harmony.kind[0].Value = GetKindValue(currentNote.Chord.Classification);
                    if (currentNote.Chord.Classification == ChordClassification.HalfDiminishedSeventh)
                    {
                        harmony.degree = new degree[]
                        {
                            new degree() 
                            { 
                                degreevalue = new degreevalue() { Value = "5" },
                                degreealter = new degreealter() { Value = -1M }, 
                                degreetype = new degreetype() { Value = degreetypevalue.alter }  
                            }
                        };
                    }
                    measureItems.Add(harmony);
                }
                foreach(Note note in notesByMeasure)
                { 
                    NoteConverter currentConverter = new NoteConverter(note);
                    measureItems.Add(currentConverter.GetMusicXmlNote(false, 1));
                    measureLength += notes[noteLoop].Duration;
                    noteLoop++;
                }
                measure.Items = measureItems.ToArray();
                measures.Add(measure);
            }
            return measures;
        }

        private List<scorepartwisePartMeasure> GetChordMeasures(int fifths, List<Chord> chords, NoteDuration chordDuration)
        {
            List<scorepartwisePartMeasure> measures = new List<scorepartwisePartMeasure>();
            bool firstNote = true;
            for(int measureLoop = 0; measureLoop < chords.Count; measureLoop++)
            {
                scorepartwisePartMeasure measure = new scorepartwisePartMeasure()
                {
                    number = (measureLoop + 1).ToString()
                };
                List<object> measureItems = new List<object>();
                if (firstNote)
                {
                    measureItems.Add(GetAttributes(fifths, true));
                    firstNote = false;
                }
                Chord current = chords[measureLoop];
                for (int noteLoop = 0; noteLoop < current.NotePitches.Count; noteLoop++)
                {
                    NoteConverter converter = new NoteConverter(new Note(chordDuration, current.NotePitches[noteLoop]));
                    measureItems.Add(converter.GetMusicXmlNote(noteLoop != 0, 5));
                }
                measure.Items = measureItems.ToArray();
                measures.Add(measure);
            }
            return measures;
        }
        public override void WriteToStream(SongOutput songOutput, Stream target)
        {
            List<scorepartwisePartMeasure> measures = GetMeasures(songOutput.Scale.Key, songOutput.Melody);
            List<scorepartwisePartMeasure> chordMeasures = null;
            if (songOutput.Chords != null)
                chordMeasures = GetChordMeasures(songOutput.Scale.Key, songOutput.Chords, songOutput.ChordDuration);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(scorepartwise));
            scorepartwise root = new scorepartwise() { version = "3.1" };
            root.work = new work() { worktitle = Input.SongName };
            root.identification = new identification()
            {
                creator = new typedtext[]
                {
                    new typedtext() { type = "composer", Value = "MusicComposer" },
                    new typedtext() { type = "programmer", Value = "Tapio Lindqvist" },
                    new typedtext() { type = "clicker", Value = Input.Name }
                }
            };
            root.partlist = new partlist();
            List<scorepart> scoreparts = new List<scorepart>
            {
                new scorepart()
                {
                    id = "P1",
                    partname = new partname() { Value = "Piano" },
                    partabbreviation = new partname() { Value = "Pno." },
                    scoreinstrument = new scoreinstrument[] { new scoreinstrument() { id = "P1-I1", instrumentname = "Piano" } },
                    mididevice = new mididevice[] { new mididevice() { id = "P1-I1", port = "1" } },
                    midiinstrument = new midiinstrument[]
                {
                    new midiinstrument()
                    {
                        id = "P1-I1",
                        midichannel = "1",
                        midiprogram = "1",
                        volume = 78.7402M
                    }
                }
                }
            };
            List<scorepartwisePart> parts = new List<scorepartwisePart>
            {
                new scorepartwisePart()
                {
                    id = "P1",
                    measure = measures.ToArray()
                }
            };
            if (chordMeasures != null)
            {
                scoreparts.Add(new scorepart()
                {
                    id = "P2",
                    partname = new partname() { Value = "Pad Synthesizer" },
                    partabbreviation = new partname() { Value = "Synth." },
                    scoreinstrument = new scoreinstrument[] { new scoreinstrument() { id = "P2-I1", instrumentname = "Pad Synthesizer" } },
                    mididevice = new mididevice[] { new mididevice() { id = "P2-I1", port = "1" } },
                    midiinstrument = new midiinstrument[]
                    {
                        new midiinstrument()
                        {
                            id = "P2-I1",
                            midichannel = "4",
                            midiprogram = "89",
                            volume = 78.7402M,
                            pan = 0
                        }
                    }
                });
                parts.Add(new scorepartwisePart()
                {
                    id = "P2",
                    measure = chordMeasures.ToArray()
                });
            }
            root.partlist.Items = scoreparts.ToArray();
            root.part = parts.ToArray();
            xmlSerializer.Serialize(target, root);
        }
    }
}
