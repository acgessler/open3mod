Open 3D Model Viewer 
========

A platform-independent general-purpose 3D model viewer and a modernized replacement for the former AssimpView. Technically,
open3mod is based on the [Open Asset Import Library] (https://github.com/assimp/assimp).

__Some key points:__


 - Supports a huge variety (30+) of 3D file formats (incl. __FBX, DXF, Collada, Obj, 3DS, STL, IFC__). 
 - Efficient tools to inspect the scene (or parts of it - filtering should be as easy as possible because
   this is a frequent use-case). 
 - Tabbed UI, so many files can be open at once. 
 - Multiple viewports (up to 4) and different camera modi, such as orbit cameras or even First-Person-View.
 - Virtually all texture file formats are supported through DevIL. Textures are being loaded asynchronously, so there
   are no extra waiting times if you only care about geometry.
 - Export of scenes (or parts of scenes) is supported, too. Output formats include Collada, 3DS, PLY, STL, OBJ 
   (as of now).

This is _work in progress_. As soon as the first version is ready, I'll add a website and provide download packages.
I currently intend to support Windows, Linux and Mac (the two latter through Mono!).

