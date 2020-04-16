# MusicComposer

Application / library which generates random music 

## Current version

Initial release composes random music using some rudimentary music theory rules. Rules currently include 
-	It is more likely that pitch of concecutive notes is closer to each other
-	last note of each measure is more likely to be eiter Tonic, Mediant or Dominant. 

Program generates MusicXml file or Midi file. Files have been made and tested only with MuseScore application. Music is generated only in C-major and C-minor. 

## Future versions

Roadmap / features to be implemented. Not necessary in this order
1.	AI analysis finding correlations between parameters and ratings. 
2.	Improving generation to work with other scales than C-minor and C-major
a.	Defining pitch range
b.	Adding octave handling 
3.	Generating music in different types of scale
