using MusicComposerLibrary.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MusicComposerLibrary
{
    public class SongPartGenerator
    {
        private readonly WeightedRandom _weightedRandom;
        private readonly Structures.Scale _scale;
        private readonly Weights _weights;
        
        private readonly Generators.RythmGenerator _rythmGenerator;
        private readonly Generators.MelodyGenerator _melodyGenerator;
        private readonly Generators.ChordGenerator _chordGenerator;
        public SongInput Input { get; private set; }
        public SongPartGenerator(SongInput songInput)
        {
            if (songInput == null)
                throw new ArgumentNullException(nameof(songInput));
            if (string.IsNullOrWhiteSpace(songInput.Name))
                throw new ArgumentException($"{nameof(songInput.Name)} is empty or null");
            if (string.IsNullOrWhiteSpace(songInput.SongName))
                throw new ArgumentException($"{nameof(songInput.SongName)} is empty or null");
            if (songInput.PartLength <= 0)
                throw new ArgumentException($"{nameof(songInput.PartLength)} must be positive value");
            if (songInput.Major)
                _scale = new Structures.Scale(new Structures.NotePitch(songInput.ScaleKeyFullName, 4), true);
            else
                _scale = new Structures.Scale(new Structures.NotePitch(songInput.ScaleKeyFullName, 4), false);

            if (songInput.DurationValues == null)
                throw new ArgumentNullException(nameof(songInput.DurationValues));
            if (songInput.PitchValues == null)
                throw new ArgumentNullException(nameof(songInput.PitchValues));
            if (songInput.WeightData == null)
                throw new ArgumentNullException(nameof(songInput.WeightData));
            Input = songInput;
            _weightedRandom = new WeightedRandom(songInput.DurationValues, songInput.PitchValues);
            _weights = new Weights(songInput.WeightData);
            
            _rythmGenerator = new Generators.RythmGenerator(_weightedRandom, _weights, songInput.PartLength, songInput.BeatsPerMeasure, songInput.BeatUnit);
            _melodyGenerator = new Generators.MelodyGenerator(_weightedRandom, _weights, _scale, songInput.MelodyLowestNoteFullNameWithOctave, songInput.MelodyHighestNoteFullNameWithOctave);
            _chordGenerator = new Generators.ChordGenerator(songInput.WeightData, _scale);
        }

        public Structures.SongOutput CreateSongPart()
        {
            _weightedRandom.Reset();

            Structures.RythmOutput rythmOutput = _rythmGenerator.CreateNoteDurations();
            List<Structures.Note> notes = _melodyGenerator.AddMelody(rythmOutput.Durations);
            Structures.SongOutput songOutput = new Structures.SongOutput() { Scale = _scale, Melody = notes };
            if (Input.Chords)
            {
                songOutput.Chords = _chordGenerator.DetermineChords(notes);
                decimal duration = Structures.NoteDuration.NoteToDuration(Input.BeatUnit) * Input.BeatsPerMeasure;
                songOutput.ChordDuration = Structures.NoteDuration.DurationToNote(duration, out decimal extra);
            }
            return songOutput;
        }
        public void WriteToStream(FileGeneratorBase.FileType fileType, Structures.SongOutput songOutput, Stream target)
        {
            FileGeneratorBase generator;
            if (fileType == FileGeneratorBase.FileType.Midi)
                generator = new Midi.MidiGenerator(Input);
            else
                generator = new MusicXml.MusicXmlGenerator(Input);
            generator.WriteToStream(songOutput, target);
        }
    }
}
