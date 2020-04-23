using Melanchall.DryWetMidi.Core;
using MusicComposerLibrary.Storage;
using MusicComposerLibrary.Structures;
using MusicComposerLibrary.Extensions;
using System.IO;

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
            InitEvents(melodyTrack.Events);
            MidiConverter.AddMelodyEvents(melodyTrack.Events, songOutput.Melody);
            file.Chunks.Add(melodyTrack);
            if (Input.Chords)
            {
                TrackChunk chordTrack = new TrackChunk();
                InitEvents(chordTrack.Events);
                MidiConverter.AddChordEvents(chordTrack.Events, songOutput.Chords);
                file.Chunks.Add(chordTrack);
            }
            file.Write(target);
        }
        private void InitEvents(EventsCollection events)
        {
            events.Add(new TimeSignatureEvent((byte)Input.BeatsPerMeasure, (byte)Input.GetBeatType(), 24, 8));
            events.Add(new KeySignatureEvent(
                (sbyte)new Structures.Scale(new Structures.NotePitch(Input.ScaleKeyFullName), Input.Major).Key, 0));
            events.Add(new SetTempoEvent(500000));
        }
    }
}
