# Lighting Scenario Switcher for Unity 3D - Beta 1.0
### Change Lightmaps, Lightprobes, Real-Time Lights, Lens Flares And Materials From Pre-Saved Profiles.

I built this tool so I could dynamically change the global lighting scenario for the entire scene at runtime. It allows the user to first generate lightmaps and setup the scene lighting elements, it then coplies and stores the required files in a resources directory so they can be loaded at any time by calling a simple script and specifying the directory to load from.

_NOTE: Static Batching - I found that if I enable Project Settings>Player>Static Batching then the lightmap takes on different offsets at runtime causing misalignment. I have disabled static batching in my project, If I figure out how to compensate for the batch offsets I will update the example with a fix._

**Video:**

_An example and tutorial video on how to use the tool:_

[![Youtube Video](https://img.youtube.com/vi/Ewt97OM19Wg/0.jpg)](https://www.youtube.com/watch?v=Ewt97OM19Wg)




Please feel free to use this example in any way you see fit (without restriction of any kind) as well as redistribute, modify and share it with all of your friends and co-workers.

Legal notice:
By downloading or using any resource from this example you agree that I (the author) am not liable for any losses or damages due to the use of any part(s) of the content in this example. It is distributed as is and without any warranty or guarantees. 

*Project by: Grant Olsen (jython.scripts@gmail.com)
Creation year: 2017*

_Big thanks to:_

* Blendswap.com for providing free models!
* Diarmid Campbell (dishmop) for scripts that create persistent Unique IDs (http://answers.unity3d.com/answers/1333189/view.html).
* robertcupisz for creating the lightshafts script (https://github.com/robertcupisz/LightShafts).





