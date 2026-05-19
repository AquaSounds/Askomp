# Askomp

Askomp is an **asymmetrical** compressor.
It will compress the positive signal (above zero) and negative signal (below zero) separately.
Using this plugin can make your audio kept in center, and naturally gain high perceived loudness.

Also you can disable the link of positive threshold and negative threshold and you will hear harmonics added to your audio.
This is because the positive signal and negative signal are compressed at different threshold.
As odd function is origin-symmetric and compress asymmetrically will destroy the signal symmetry.
As a result the even harmonics will be added to the signal, which makes its tone more tube-style.
This sounds very similar to asymmetric distortion, but more smooth and natural.

Askomp uses FFCompressor with 3dB knee and 0 link of left and right channel.
This is fixed and not editable.

## Install

#### Manually
Extract Askomp.vst3 folder and copy it to your VST3 directory.
#### Installer
Open Installer and install.

## Platforms
Askomp currently only support windows because it is based on [AudioPlugSharp](https://github.com/mikeoliphant/AudioPlugSharp).
Maybe in the future it will support platforms like linux or macOS.

## Compile

### C#

The AudioPlugSharp NuGet Packages were added to the project.

For build, "AudioPlugSharpWPF.dll" will be added to the build folder.
You can copy it manually, and in this project it was handled in the csproj.
Which like:
```
<Target Name="CopyAudioPlugSharpWPF" AfterTargets="Build">
    <Exec Command="copy &quot;$(NuGetPackageRoot)audioplugsharpwpf\0.7.9\lib\net8.0-windows7.0\AudioPlugSharpWPF.dll&quot; &quot;$(OutputPath)&quot; /Y" />
</Target>
```

Open Rider and choose x64, press build button.

### Faust

The Faust code is in dsp folder, you need faust to build [askomp_dsp.dsp](dsp/askomp_dsp.dsp).

Open cmd and use `faust -archdir` to find your faust architecture folder.
You should find "CSharpFaustBase.cs" and copy it into the dsp folder.
And you need to use cmd like
`faust -lang csharp -i -a CSharpFaustBase.cs askomp_dsp.dsp -o <filename>.cs -double`
and copy the file you build to Askomp folder.
You also need to make sliders public or internal, it should be like:
```
internal double fVslider0;  
internal double fHslider0;  
int fSampleRate;  
double fConst0;  
internal double fHslider1;  
double[] fRec0 = new double[2];  
internal double fHslider2;  
internal double fVslider1;  
double[] fRec1 = new double[2];  
internal double fHslider3;  
double[] fRec2 = new double[2];  
double[] fRec3 = new double[2];  
```
For this project, the results is in [faustdsp.cs](Askomp/faustdsp.cs).



