## RAVDESS audio files naming convention

Each of the 7356 RAVDESS files has a unique filename. The filename consists of a 7-part numerical identifier. These identifiers define the stimulus characteristics:

### Filename identifiers 

* Modality - 01 = full-AV, 02 = video, 03 = audio.
* Vocal channel - 01 = speech, 02 = song.
* Emotion - 01 = neutral, 02 = calm, 03 = happy, 04 = sad, 05 = angry, 06 = fearful, 07 = disgust, 08 = surprised.
* Emotional intensity - 01 = normal, 02 = strong. There is no strong intensity for the _neutral_ emotion.
* Statement - 01 = _Kids are talking by the door_, 02 = _Dogs are sitting by the door_.
* Repetition - 01 = 1st repetition, 02 = 2nd repetition.
* Actor - 01 to 24. Odd numbered actors are male, even numbered actors are female.
* Filename example: 

`03-01-06-01-02-01-12.wav` 

```
- Audio (03)
- Speech (01)
- Fearful (06)
- Normal intensity (01)
- Statement _dogs_ (02)
- 1st Repetition (01)
- 12th Actor (12)
- Female, as the actor ID number is even.
```
