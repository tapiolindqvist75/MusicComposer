using Melanchall.DryWetMidi.Core;
using MusicComposerLibrary.Storage;
using MusicComposerLibrary.Structures;
using MusicComposerLibrary.Extensions;
using System.IO;
using Melanchall.DryWetMidi.Common;

namespace MusicComposerLibrary.Midi
{
    public class MidiGenerator : FileGeneratorBase
    {
        public MidiGenerator(SongInput songData) : base(songData) { }

        public override void WriteToStream(SongOutput songOutput, Stream target)
        {
            MidiFile file = new MidiFile
            {
                TimeDivision = new TicksPerQuarterNoteTimeDivision(480)
            };
            TrackChunk melodyTrack = new TrackChunk();
            InitEvents(melodyTrack.Events, false);
            MidiConverter.AddMelodyEvents(melodyTrack.Events, songOutput.Melody);
            file.Chunks.Add(melodyTrack);
            if (Input.Chords)
            {
                TrackChunk chordTrack = new TrackChunk();
                InitEvents(chordTrack.Events, true);
                MidiConverter.AddChordEvents(chordTrack.Events, songOutput.Chords, 3, songOutput.ChordDuration);
                file.Chunks.Add(chordTrack);
            }
            file.Write(target);
        }
        private void InitEvents(EventsCollection events, bool chords)
        {
            events.Add(new TimeSignatureEvent((byte)Input.BeatsPerMeasure, (byte)Input.GetBeatType(), 24, 8));
            events.Add(new KeySignatureEvent(
                (sbyte)new Structures.Scale(new Structures.NotePitch(Input.ScaleKeyFullName, 4), Input.Major).Key, 0));
            events.Add(new SetTempoEvent(500000));
            if (chords)
            {
                events.Add(new ControlChangeEvent() { Channel = new FourBitNumber(3), ControlNumber = new SevenBitNumber(121), ControlValue = new SevenBitNumber(0) });
                events.Add(new ProgramChangeEvent() { ProgramNumber = new SevenBitNumber(88), Channel = new FourBitNumber(3) });
                events.Add(new ControlChangeEvent() { Channel = new FourBitNumber(3), ControlNumber = new SevenBitNumber(7), ControlValue = new SevenBitNumber(100) });
                events.Add(new ControlChangeEvent() { Channel = new FourBitNumber(3), ControlNumber = new SevenBitNumber(10), ControlValue = new SevenBitNumber(64) });
                events.Add(new ControlChangeEvent() { Channel = new FourBitNumber(3), ControlNumber = new SevenBitNumber(91), ControlValue = new SevenBitNumber(0) });
                events.Add(new ControlChangeEvent() { Channel = new FourBitNumber(3), ControlNumber = new SevenBitNumber(93), ControlValue = new SevenBitNumber(0) });
                events.Add(new PortPrefixEvent() { Port = 0 });
            }
                
            else
                events.Add(new ProgramChangeEvent() { ProgramNumber = new SevenBitNumber(0), Channel = new FourBitNumber(0)});
        }
    }
}
