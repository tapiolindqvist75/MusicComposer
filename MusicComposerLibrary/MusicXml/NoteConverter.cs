using System;
using System.Collections.Generic;
using System.Text;
using MusicComposerLibrary.Structures;

namespace MusicComposerLibrary.MusicXml
{
    public class NoteConverter
    {
        Note _note;
        public NoteConverter(Note note)
        {
            _note = note;
            GetStep();
        }
        private step GetStep()
        {
            switch(_note.Name)
            {
                case 'C': return step.C;
                case 'D': return step.D;
                case 'E': return step.E;
                case 'F': return step.F;
                case 'G': return step.G;
                case 'A': return step.A;
                case 'B': return step.B;
                default: throw new ArgumentException("Invalid Note " + _note.Name.ToString());
            }
        }
        private static notetypevalue GetNotetypevalue(Note note)
        {
            switch (note.NoteLength)
            {
                case NoteDuration.NoteLengthType.Half:
                    return notetypevalue.half;
                case NoteDuration.NoteLengthType.Quarter:
                    return notetypevalue.quarter;
                case NoteDuration.NoteLengthType.Eigth:
                    return notetypevalue.eighth;
                case NoteDuration.NoteLengthType.Sixteenth:
                    return notetypevalue.Item16th;
                default:
                    throw new ArgumentException("Note.NoteLength");
            }
        }
        public note GetMusicXmlNote()
        {
            note note = new note();
            List<ItemsChoiceType1> itemChoices = new List<ItemsChoiceType1>();
            List<object> items = new List<object>();
            itemChoices.Add(ItemsChoiceType1.pitch);
            pitch pitch = new pitch() { step = GetStep(), octave = "4" };
            items.Add(pitch);
            itemChoices.Add(ItemsChoiceType1.duration);
            items.Add((decimal)_note.Duration);
            if (_note.Tie != NoteDuration.TieType.None)
            {
                ///In MuseScore note tie's are specified both in notatation/tied and tie elements.
                List<object> notations = new List<object>();
                itemChoices.Add(ItemsChoiceType1.tie);
                tie tie = new tie();
                tied tied = new tied();
                if (_note.Tie == NoteDuration.TieType.End)
                {
                    tie.type = startstop.stop;
                    tied.type = startstopcontinue.stop;
                }
                else
                {
                    tie.type = startstop.start;
                    tied.type = startstopcontinue.start;
                }
                items.Add(tie);
                notations.Add(tied);
                if (_note.Tie == NoteDuration.TieType.Both)
                {
                    itemChoices.Add(ItemsChoiceType1.tie);
                    items.Add(new tie() { type = startstop.stop });
                    notations.Add(new tied() { type = startstopcontinue.stop });
                }
                note.notations = new notations[] { new notations() { Items = notations.ToArray() } };
            }
            note.ItemsElementName = itemChoices.ToArray();
            note.Items = items.ToArray();
            if (_note.Dot)
                note.dot = new emptyplacement[] { };
            if (_note.Offset != 0)
            {
                pitch.alterSpecified = true;
                pitch.alter = _note.Offset;
            }
            note.type = new notetype() { Value = NoteConverter.GetNotetypevalue(_note) };
            return note;
        }

        public decimal GetDuration()
        { 
            return _note.Duration;
        }
    }
}
