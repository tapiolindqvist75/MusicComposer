# MusicComposer

Application / library which generates random music 

## Current version

Current release composes random music using some rudimentary music theory rules. Rules currently include 
-	It is more likely that pitch of concecutive notes is closer to each other
-	last note of each measure is more likely to be eiter Tonic, Mediant or Dominant. 
- chords are assigned by comparing chord notes to measure notes.

Program generates MusicXml file or Midi file. Files have been made and tested only with MuseScore application. Music is generated only in C-major and C-minor. 

## Future versions

Roadmap / features to be implemented. Not necessary in this order
1.	AI analysis finding correlations between parameters and ratings. 
2.  Generating music in different types of scale
3.  Generating music from by chord first order. Using common progression varioations etc. 
4.  Adding rests to music so melody can be sung
5.  Mobile application(s)
6.  Combining parts to make a full song applying cadences.
