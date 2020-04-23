using System;
using System.Collections.Generic;
using System.Text;
using MusicComposerLibrary.Structures;

namespace MusicComposerLibrary.MusicXml
{
    public class NoteConverter
    {
        private readonly Note _note;
        public NoteConverter(Note note)
        {
            _note = note;
            GetStep();
        }
        private step GetStep()
        {
            return _note.Pitch.Name switch
            {
                'C' => step.C,
                'D' => step.D,
                'E' => step.E,
                'F' => step.F,
                'G' => step.G,
                'A' => step.A,
                'B' => step.B,
                _ => throw new ArgumentException("Invalid Note " + _note.Pitch.Name.ToString()),
            };
        }
        private static notetypevalue GetNotetypevalue(Note note)
        {
            return note.NoteLength switch
            {
                NoteDuration.NoteLengthType.Half => notetypevalue.half,
                NoteDuration.NoteLengthType.Quarter => notetypevalue.quarter,
                NoteDuration.NoteLengthType.Eigth => notetypevalue.eighth,
                NoteDuration.NoteLengthType.Sixteenth => notetypevalue.Item16th,
                _ => throw new ArgumentException("Note.NoteLength"),
            };
        }

        private beamvalue GetBeamvalue()
        {
            return _note.Beam switch
            {
                NoteDuration.LinkType.Start => beamvalue.begin,
                NoteDuration.LinkType.End => beamvalue.end,
                NoteDuration.LinkType.Continue => beamvalue.@continue,
                _ => throw new Exception("Invalid usage"),
            };
        }
        public note GetMusicXmlNote(bool chord, int staff, int voice)
        {
            note note = new note();
            if (_note.Beam != NoteDuration.LinkType.None)
                note.beam = new beam[] { new beam() { number = "1", Value = GetBeamvalue() } };
            List<ItemsChoiceType1> itemChoices = new List<ItemsChoiceType1>();
            List<object> items = new List<object>();
            if (chord)
            {
                itemChoices.Add(ItemsChoiceType1.chord);
                items.Add(new empty());
            }
            itemChoices.Add(ItemsChoiceType1.pitch);
            pitch pitch = new pitch() { step = GetStep(), octave = _note.Pitch.Octave.ToString() };
            items.Add(pitch);
            itemChoices.Add(ItemsChoiceType1.duration);
            items.Add((decimal)GetDuration());
            note.staff = staff.ToString();
            note.voice = voice.ToString();
            if (_note.Tie != NoteDuration.LinkType.None)
            {
                ///In MuseScore note tie's are specified both in notatation/tied and tie elements.
                List<object> notations = new List<object>();
                itemChoices.Add(ItemsChoiceType1.tie);
                tie tie = new tie();
                tied tied = new tied();
                if (_note.Tie == NoteDuration.LinkType.End)
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
                if (_note.Tie == NoteDuration.LinkType.Continue)
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
            if (_note.Pitch.Offset != 0)
            {
                pitch.alterSpecified = true;
                pitch.alter = _note.Pitch.Offset;
            }
            note.type = new notetype() { Value = NoteConverter.GetNotetypevalue(_note) };
            return note;
        }

        public decimal GetDuration()
        { 
            return _note.Duration * 4;
        }
    }
}
