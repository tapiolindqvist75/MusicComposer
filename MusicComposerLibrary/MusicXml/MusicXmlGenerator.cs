using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using MusicComposerLibrary.Storage;
using MusicComposerLibrary.Structures;
using MusicComposerLibrary.Extensions;

namespace MusicComposerLibrary.MusicXml
{
    public class MusicXmlGenerator : FileGeneratorBase
    {
        public MusicXmlGenerator(SongData songData) : base(songData) { }
        private List<scorepartwisePartMeasure> GetMeasures(int fifths, List<Note> notes)
        {
            List<scorepartwisePartMeasure> measures = new List<scorepartwisePartMeasure>();
            int noteLoop = 0;
            bool firstNote = true;
            int measureNumber = 1;
            while(noteLoop < notes.Count)
            {
                decimal measureLength = 0;
                scorepartwisePartMeasure measure = new scorepartwisePartMeasure()
                {
                    number = (measureNumber).ToString()
                };
                List<object> measureItems = new List<object>();
                if (firstNote)
                {
                    measureItems.Add(new attributes()
                    {
                        divisions = 2,
                        key = new key[] { new key() { ItemsElementName = new ItemsChoiceType8[] { ItemsChoiceType8.fifths }, Items = new object[] { fifths.ToString() } } },
                        clef = new clef[]
                        {
                            new clef()
                            {
                                sign = clefsign.G,
                                line = "2"
                            }
                        },
                        time = new time[]
                        {
                            new time()
                            {
                                ItemsElementName = new ItemsChoiceType9[] { ItemsChoiceType9.beats, ItemsChoiceType9.beattype },
                                Items = new object[] 
                                { 
                                    SongData.BeatsPerMeasure.ToString(), 
                                    SongData.GetBeatType().ToString()
                                }
                            }
                        }
                    });
                    firstNote = false;
                }
                while (measureLength < 1)
                {
                    NoteConverter current = new NoteConverter(notes[noteLoop]);
                    measureItems.Add(current.GetMusicXmlNote());
                    measureLength += current.GetDuration();
                    noteLoop++;
                }
                measure.Items = measureItems.ToArray();
                measures.Add(measure);
                measureNumber++;
            }
            return measures;
        }

        public override void WriteToStream(List<Note> notes, Stream target)
        {
            List<scorepartwisePartMeasure> measures = GetMeasures(SongData.GetKey(), notes);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(scorepartwise));
            scorepartwise root = new scorepartwise() { version = "3.1" };
            root.work = new work() { worktitle = SongData.SongName };
            root.identification = new identification()
            {
                creator = new typedtext[]
                {
                    new typedtext() { type = "composer", Value = "MusicComposer" },
                    new typedtext() { type = "programmer", Value = "Tapio Lindqvist" },
                    new typedtext() { type = "clicker", Value = SongData.Name }
                }
            };
            root.partlist = new partlist()
            {
                scorepart = new scorepart()
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
                            volume = 75
                        }
                    }
                },
            };
            root.part = new scorepartwisePart[]
            {
                new scorepartwisePart()
                {
                    id = "P1",
                }
            };
            root.part[0].measure = measures.ToArray();
            xmlSerializer.Serialize(target, root);
        }
    }
}
