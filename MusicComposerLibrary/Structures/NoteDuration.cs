using System;
using System.Collections.Generic;
using System.Text;

namespace MusicComposerLibrary.Structures
{
    public class NoteDuration
    {
        public enum LinkType { None, Start, Continue, End }
        public enum NoteLengthType { Full, Half, Quarter, Eigth, Sixteenth }
        public NoteLengthType NoteLength { get; private set; }
        public decimal Duration { get; private set; }
        public bool Dot { get; private set; }
        public LinkType Tie { get; set; }
        public bool LastOfMeasure { get; set; }
        public int MeasureNumber { get; set; }
        public LinkType Beam { get; set; }
        public NoteDuration(NoteLengthType noteLenght, bool dot, LinkType tie)
        {
            NoteLength = noteLenght;
            Dot = dot;
            Duration = NoteToDuration(noteLenght, dot);
            Tie = tie;
        }

        protected NoteDuration(NoteDuration noteDuration)
        {
            NoteLength = noteDuration.NoteLength;
            Dot = noteDuration.Dot;
            Duration = noteDuration.Duration;
            Tie = noteDuration.Tie;
            LastOfMeasure = noteDuration.LastOfMeasure;
            MeasureNumber = noteDuration.MeasureNumber;
        }

        public static decimal NoteToDuration(NoteLengthType noteLength, bool dot = false)
        {
            return noteLength switch
            {
                NoteLengthType.Full => dot ? 1.5M : 1M,
                NoteLengthType.Half => dot ? 0.75M : 0.5M,
                NoteLengthType.Quarter => dot ? 0.375M : 0.25M,
                NoteLengthType.Eigth => dot ? 0.1875M : 0.125M,
                NoteLengthType.Sixteenth => dot ? 0.09375M : 0.0625M,
                _ => throw new Exception("Invalid input"),
            };
        }

        public static NoteDuration DurationToNote(decimal duration, out decimal extra)
        {
            NoteDuration noteDuration = CompareAndReturn(duration, 0.75M, NoteLengthType.Half, true, out extra);
            if (noteDuration != null) return noteDuration;
            noteDuration = CompareAndReturn(duration, 0.5M, NoteLengthType.Half, false, out extra);
            if (noteDuration != null) return noteDuration;
            noteDuration = CompareAndReturn(duration, 0.375M, NoteLengthType.Quarter, true, out extra);
            if (noteDuration != null) return noteDuration;
            noteDuration = CompareAndReturn(duration, 0.25M, NoteLengthType.Quarter, false, out extra);
            if (noteDuration != null) return noteDuration;
            noteDuration = CompareAndReturn(duration, 0.1875M, NoteLengthType.Eigth, true, out extra);
            if (noteDuration != null) return noteDuration;
            noteDuration = CompareAndReturn(duration, 0.125M, NoteLengthType.Eigth, false, out extra);
            if (noteDuration != null) return noteDuration;
            noteDuration = CompareAndReturn(duration, 0.09375M, NoteLengthType.Sixteenth, true, out extra);
            if (noteDuration != null) return noteDuration;
            noteDuration = CompareAndReturn(duration, 0.0625M, NoteLengthType.Sixteenth, false, out extra);
            if (noteDuration != null) return noteDuration;
            throw new Exception("Invalid duration");
        }

        private static NoteDuration CompareAndReturn(decimal duration, decimal value, NoteLengthType noteLength, bool dot,
            out decimal extra)
        {
            if (duration >= value)
            {
                extra = value - duration;
                return new NoteDuration(noteLength, dot, LinkType.None);
            }
            extra = 0;
            return null;
        }
    }
}
