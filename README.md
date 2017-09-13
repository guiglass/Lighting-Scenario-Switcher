# Lighting Scenario Switcher for Unity 3D - Beta 1.0
### Change Lightmaps, Lightprobes, Real-Time Lights, Lens Flares And Materials From Pre-Saved Profiles.

This is a tool I built so I could switch the global lighting scenario of my scene in real-time. It allows the user to generate lightmaps and setup the lighting, then stor all the required information in a resources directory to be loaded at runtime.

**Video:**
_An example and tutorial video on how to use this tool:_

[![Youtube Video](https://img.youtube.com/vi/Ewt97OM19Wg/0.jpg)](https://www.youtube.com/edit?o=U&video_id=Ewt97OM19Wg)


**Here’s how it works:**
![alt tag](https://raw.githubusercontent.com/guiglass/StationaryStereoCamera/master/SceneLayout.png)

The “Camera (HUD)” is a tracked camera, meaning that Unity will assume that it should follow the gyro of the HMD. This example shows a way to spoof a tracked camera and feed it renderTextures from our own right and left cameras. Anything that this spoofed head-tracked camera “sees” will be irrelevant and is completely overridden by renderTextures from two stationary (left and right) cameras in a custom stereo camera rig that is constructed in the example. By setting the spoofed camera’s culling mask to “Nothing” and then feeding in the two renderTextures from the right and left eye cameras, the spoofed head-tracked camera will display the two stationary camera's images as it's own left and right eye and display them in the HMD while being overlaid on top of the main camera.

This example allows building a stereo camera rig from the ground up and gives more control over parameters such as camera position/rotation, eye separation, field of view, convergence plane, and even some renderTexture specific stuff such as anti-aliasing and resolution.

The HUD layer support transparency as well as lights and shadows, so it acts and looks pretty much like any other camera would.


**Configure the scene from scratch (OpenVR already installed):**

![alt tag](https://raw.githubusercontent.com/guiglass/StationaryStereoCamera/master/Step1.png)
**_Step 1 - Scene creation and virtual reality setup._**

* Create a new Unity project and name it StereoCameraOverlay.
* Press Ctrl+S and save the scene as “ExampleScene”
* Delete the “MainCamera” from the project hierarchy.
* Navigate to Edit > Project Settings > Player.
* Check the box “Virtual Reality Supported”.
* In “Virtual Reality SDKs” ensure OpenVR is the only item in the list.



![alt tag](https://raw.githubusercontent.com/guiglass/StationaryStereoCamera/master/Step2.png)
**_Step 2 - Creating the custom stereo camera rig and camera objects._**

* Add the two scripts (StereoCameraController.cs and StereoCameraPreRendere.cs) to the assets folder.
* Create an empty game object and name it “HUD” (This will be the parent of our HUD world and stationary cameras).
* Ensure the transform position and rotation are all set to zero.
* Create a camera and name it “Camera (HUD)” (this is the tracked hud camera that renders nothing of it’s own and has renderTextues fed into it).
* Remove “GUI Layer”, “Flare Layer” and “Audio Listener” components from the camera as they are not required.
* Drag that camera object into and make it a child of the “HUD” object.
* Ensure the transform position and rotation are all set to zero.
* Set the Clear Flags to Don’t Clear so it doesn’t render any colors or sky.
* Set Culling Mask to Nothing so it doesn’t render any scene objects.



![alt tag](https://raw.githubusercontent.com/guiglass/StationaryStereoCamera/master/Step3.png)
**_Step 3 - Create and setting up the stationary stereo camera._**

* Create an empty game object and name it “StereoCameras” (This will be the parent of our left and right eye stationary cameras).
* Drag that object into and make it a child of the “HUD” object.
* Add two new cameras and name them as “Camera (Left)” and “Camera (Right)” (these are the cameras that only see things on the HUD layer).
* Drag both cameras into the “StereoCameras” object.
* Ensure the transform positions and rotations are all set to zero (you may separate the two cameras on the x axis a little if you wish - this will not affect how the script positions them at runtime).
* Remove “GUI Layer”, “Flare Layer” and “Audio Listener” components from both cameras as they are not required.
* Set the Clear Flags to solid color, and select a color of black and no alpha (#00000000).
* Add a new layer called “HUD” (any object on is layer will be visible to these cameras only).
* Set the Culling mask to be “HUD” layer only. 



![alt tag](https://raw.githubusercontent.com/guiglass/StationaryStereoCamera/master/Step4.png)
**_Step 4 - Create renderTextures for left and right eyes._**

* Right click in your project’s asset folder and create a single new renderTexture (we will copy it  for both eyes later on).
* Set the Size parameter to 1920x1920 (This is what I chose but can be changed as desired).
* Set Anti-Aliasing to some value (I chose 8 samples but again, you can decide).
* Set Color Format to ARGB Half, (I believe this is the native format used by cameras??).
* Now, just copy that renderTexture (Ctrl+d) and name the two as “camLeft” and “camRight”.
* Finally, select each of the two cameras in StereoCameras and add the renderTextures to the to their Target Texture, ensure the “camRight” texture is placed in “Camera (Right)” and “camLeft” is placed in “Camera (Left)”.

*Now that those two cameras have renderTextures attached, Unity will no longer try to  change their positions or any other parameters. So all we have to do now is use those renderTextures as the inputs for our tracked camera that is sending video to the HMD.*



![alt tag](https://raw.githubusercontent.com/guiglass/StationaryStereoCamera/master/Step5.png)
**_Step 5 - Adding the StereoCameraController script and linking the two stereo cameras._**

* Place the StereoCameraController.cs script onto the StereoCameras object.
* Drag each Camera (Left) and Camera (Right) and place them in the script’s "Camera" variables.
* The other default values are there to be adjusted but should work ok for most projects (as well as this example).



![alt tag](https://raw.githubusercontent.com/guiglass/StationaryStereoCamera/master/Step6.png)
**_Step 6 - Adding the StereoCameraPreRenderer script and linking the two renderTextures._**

* Drag the StereoCameraPreRenderer.cs script onto the “Camera (HUD)” object.
* Drag each “camLeft” and “camRight” renderTextures and place them in the script’s corresponding “TexLeft“ and “TexRight” variables.
* Click the small circle button to the right of “Mat” and select “Default-Particle”.



![alt tag](https://raw.githubusercontent.com/guiglass/StationaryStereoCamera/master/Step7.png)
**_Step 7 - Adding the main camera._**

* Create a camera and name it “Camera (eye)” (this is the tracked main camera that renders the world to the HMD).
* Set the Culling Mask however you wish except make sure that the “HUD” layer is not selected.
* Set the depth to be lower than the depth of "Camera (HUD)", I am using a value of -1.
* The rest of that camera’s configuration is totally up to you.



![alt tag](https://raw.githubusercontent.com/guiglass/StationaryStereoCamera/master/Step8.png)
**_Step 8 - Adding some visible HUD objects._**

* Add some 3dText or meshes to your scene, then for each of them simply change their layer to “HUD”. This will make them visible to only the two stereo cameras.
* Make those object children of the “HUD” object in the hierarchy (optional really..).
* Remember that this camera is stationary (in this example), so you must place all of your HUD object in front of the camera so they are in it’s field of view.



![alt tag](https://raw.githubusercontent.com/guiglass/StationaryStereoCamera/master/Step9.png)
**_Step 9 - Adding some visible world objects._**

* Just as you normally would add objects to your scene, do so and place then anywhere in the scene (I have placed them near my HUD to demonstrate how the two cameras can’t see the various layers).
* Now press the Play button and if everything went well you should see that TextHUD is visible overtop of the main camera and best of all, it’s completely stationary and free from any noticeable jerky motion when rotating your head quickly.



Something to consider when it come to lighting is that you may want to manage the Culling Masks for lights and make sure your main world’s lights do not include the HUD layer (especially the Directional Lights and area lamps). Additionally you can add lighting to you HUD layer by setting the lamp’s Culling Mask to only HUD layer.

Please feel free to use this example in any way you see fit (without restriction of any kind) as well as redistribute, modify and share it with all of your friends and co-workers.

Legal notice:
By downloading or using any resource from this example you agree that I (the author) am not liable for any losses or damages due to the use of any part(s) of the content in this example. It is distributed as is and without any warranty or guarantees. 

*Project by: Grant Olsen (jython.scripts@gmail.com)
Creation year: 2017*

Big thanks to:

Blendswap.com for providing free models!
robertcupisz for creating the lightshafts script (https://github.com/robertcupisz/LightShafts).





