using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MusicComposerLibrary.Structures
{
    public class NotePitch : IComparable<NotePitch>
    {
        public NotePitch(string fullNameWithOctave)
        {
            string fullName = fullNameWithOctave.Substring(0, fullNameWithOctave.Length - 1);
            int octave = Convert.ToInt32(fullNameWithOctave.Substring(fullName.Length));
            InitFromFullName(fullName, octave);
            MidiNumber = NoteToMidi(Name, Offset, octave);
        }
        public NotePitch(string fullName, int octave)
        {
            InitFromFullName(fullName, octave);
            MidiNumber = NoteToMidi(Name, Offset, octave);
        }
        public NotePitch(char name, short offset, int octave)
        {
            Name = name;
            Offset = offset;
            MidiNumber = NoteToMidi(name, offset, octave);
        }
        public NotePitch(int midiNumber, bool sharp)
        {
            Octave = (midiNumber / 12) - 1;
            MidiNumber = midiNumber;
            int note = midiNumber % 12;
            switch(note)
            {
                case 0: Name = 'C'; break;
                case 1: SetNameAndOffset('C', 'D', sharp); break;
                case 2: Name = 'D'; break;
                case 3: SetNameAndOffset('D', 'E', sharp); break;
                case 4: Name = 'E'; break;
                case 5: Name = 'F'; break;
                case 6: SetNameAndOffset('F', 'G', sharp); break;
                case 7: Name = 'G'; break;
                case 8: SetNameAndOffset('G', 'A', sharp); break;
                case 9: Name = 'A'; break;
                case 10: SetNameAndOffset('A', 'B', sharp); break;
                case 11: Name = 'B'; break;
                default: throw new ArgumentException("invalid arguments");
            }
        }
        private void SetNameAndOffset(char sharpNote, char flatNote, bool sharp)
        {
            if (sharp)
            {
                Name = sharpNote;
                Offset = 1;
            }
            else
            {
                Name = flatNote;
                Offset = -1;
            }
        }
        public static byte NoteToMidi(char name, short offset, int octave)
        {
            byte root = Convert.ToByte((octave + 1) * 12); 
            switch (name)
            {
                case 'C': return Convert.ToByte(root + offset);
                case 'D': return Convert.ToByte(root + 2 + offset);
                case 'E': return Convert.ToByte(root + 4 + offset);
                case 'F': return Convert.ToByte(root + 5 + offset);
                case 'G': return Convert.ToByte(root + 7 + offset);
                case 'A': return Convert.ToByte(root + 9 + offset);
                case 'B': return Convert.ToByte(root + 11 + offset);
                default: throw new ArgumentException("Invalid note, valid values 'C','D','E','F','G','A','B'");
            }
        }
        public int MidiNumber { get; private set; }
        public short Offset { get; private set; }
        public char Name { get; set; }
        public int Octave { get; set; }
        public string FullName
        {
            get
            {
                string fullName = Name.ToString();
                switch(Offset)
                {
                    case 0: return fullName;
                    case 1: return fullName + "#";
                    case -1: return fullName + "b";
                    case 2: return fullName + "##";
                    case -2: return fullName + "bb";
                    default: throw new Exception("Invalid Offset");
                }
            }
        }

        public string FullNameWithOctave
        {
            get { return FullName + Octave.ToString(); }
        }
        private void InitFromFullName(string fullName, int octave)
        {
            Name = fullName[0];
            if (fullName.Length > 1)
            {
                string accidental = fullName.Substring(1);
                switch(accidental)
                {
                    case "#": Offset = 1; break;
                    case "##": Offset = 2; break;
                    case "b": Offset = -1; break;
                    case "bb": Offset = -2; break;
                    default: throw new ArgumentException("Invalid fullName");
                }
            }
            Octave = octave;
        }
        public int CompareTo([AllowNull] NotePitch other)
        {
            if (other == null)
                return -1;
            return other.MidiNumber.CompareTo(MidiNumber);
        }
        public bool IsSameIgnoreOctave(NotePitch notePitch)
        {
            if (notePitch == null)
                throw new ArgumentNullException(nameof(notePitch));
            if (MidiNumber % 12 == notePitch.MidiNumber % 12)
                return true;
            return false;
        }
    }
}
