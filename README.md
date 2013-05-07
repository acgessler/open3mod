_Release Date: June 2013 (est)_

Open 3D Model Viewer 
========

A general-purpose 3D model viewer. Based on 
[Open Asset Import Library] (https://github.com/assimp/assimp), it supports 
reading about __thirty 3D file formats__, including __FBX, DXF, Collada, Obj, X, PLY, 3DS, LWO, LWS, STL__ 
and  __IFC__ ([full list] (http://assimp.sourceforge.net/main_features_formats.html)).

![Screenshot of open3mod (14-03-2013)](http://www.greentoken.de/download/open3mod1.png)

Currently it is  _work in progress_ and there is no downloadable release yet, but the latest source
versions are good for productive use. At the time being only Windows is supported even though the 
long-term goal is to be platform-independent (through Mono).

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


