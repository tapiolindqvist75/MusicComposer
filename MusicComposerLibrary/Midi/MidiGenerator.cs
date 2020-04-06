using Melanchall.DryWetMidi.Core;
using MusicComposerLibrary.Storage;
using MusicComposerLibrary.Structures;
using MusicComposerLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Melanchall.DryWetMidi.Common;

namespace MusicComposerLibrary.Midi
{
    public class MidiGenerator : FileGeneratorBase
    {
        public MidiGenerator(SongData songData) : base(songData) { }

        public override void WriteToStream(List<Note> notes, Stream target)
        {
            MidiFile file = new MidiFile
            {
                TimeDivision = new TicksPerQuarterNoteTimeDivision(480)
            };
            TrackChunk trackChunk = new TrackChunk();
            file.Chunks.Add(trackChunk);
            AddToMidiChunk(notes, trackChunk);
            file.Write(target);
        }

        private void AddToMidiChunk(List<Note> notes, TrackChunk trackChunk)
        {
            trackChunk.Events.Add(new TimeSignatureEvent((byte)SongData.BeatsPerMeasure, (byte)SongData.GetBeatType(), 24, 8));
            trackChunk.Events.Add(new KeySignatureEvent((sbyte)SongData.GetKey(), 0));
            trackChunk.Events.Add(new SetTempoEvent(500000));
            MidiConverter.AddNotesToChunks(trackChunk.Events, notes);
        }
    }
}
