# Open 3D Model Viewer 

A general-purpose 3D model viewer. Based on 
[Open Asset Import Library](https://github.com/assimp/assimp), it supports 
reading about __fourty 3D file formats__, including __FBX, DXF, Collada, Obj, X, PLY, 3DS, LWO, LWS, STL__ 
and  __IFC__ ([full list](http://assimp.sourceforge.net/main_features_formats.html)).

![Screenshot of open3mod (09-05-2013)](http://s1.directupload.net/images/130509/44lqi4p9.png)

__Some key features:__

 - Powerful 3D preview that leverages modern rendering and lighting technologies and thus gives a good impression of how
   scenes would look in a modern 3D game, or even in non-realtime renderings.
 - Skeletal animation playback at arbitrary speed or single-step.
 - Efficient tools to inspect the scene or parts of it. Filtering and isolating elements is made as easy as possible.
 - Tabbed UI, so multiple scenes can be open at the same time
 - Replace textures and fix missing paths by Drag & Drop
 - Multiple viewports (up to 4) and different camera modes such as orbit cameras or even First-Person
 - A multitude of texture file formats is supported through DevIL. Textures are being loaded asynchronously, so there
   are no extra waiting times if you only care about geometry
 - Export of scenes (or parts of scenes) is supported. Output formats include Collada, PLY, STL, OBJ 
   (as of now).

## License

3-clause BSD license. Read `LICENSE` for the details, but it boils down to "use as you like but reproduce the copyright notice".

## Contribute

Contributions to the projects are highly appreciated. Simply fork the project on Github, hack, and send me a PR.

As a starter, lots of open3mod features (such as high-quality rendering) have been started and are present in the codebase, but are currently disabled. Contact me if you'd like to help out, but don't know where to start.
