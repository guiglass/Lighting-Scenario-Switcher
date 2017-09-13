# Lighting Scenario Switcher for Unity 3D - Beta 1.0
### Change Lightmaps, Lightprobes, Real-Time Lights, Lens Flares And Materials From Pre-Saved Profiles.

This is a tool I built so I could switch the global lighting scenario of my scene in real-time. It allows the user to generate lightmaps and setup the lighting, then stor all the required information in a resources directory to be loaded at runtime.

**Video:**
_An example and tutorial video on how to use this tool:_

[![Youtube Video](https://img.youtube.com/vi/Ewt97OM19Wg/0.jpg)](https://www.youtube.com/watch?v=Ewt97OM19Wg)






Something to consider when it come to lighting is that you may want to manage the Culling Masks for lights and make sure your main world’s lights do not include the HUD layer (especially the Directional Lights and area lamps). Additionally you can add lighting to you HUD layer by setting the lamp’s Culling Mask to only HUD layer.

Please feel free to use this example in any way you see fit (without restriction of any kind) as well as redistribute, modify and share it with all of your friends and co-workers.

Legal notice:
By downloading or using any resource from this example you agree that I (the author) am not liable for any losses or damages due to the use of any part(s) of the content in this example. It is distributed as is and without any warranty or guarantees. 

*Project by: Grant Olsen (jython.scripts@gmail.com)
Creation year: 2017*

_Big thanks to:_

* Blendswap.com for providing free models!
* Diarmid Campbell (dishmop) for scripts that create persistent Unique IDs (http://answers.unity3d.com/answers/1333189/view.html).
* robertcupisz for creating the lightshafts script (https://github.com/robertcupisz/LightShafts).





