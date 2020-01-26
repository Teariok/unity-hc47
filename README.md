# Unity Hitman C47 Tools
Editor tools for the Unity engine which can import SPK files for the game Hitman: Codename 47

## Usage
The repository contains a Unity demo project tested with Unity2019.2.8f1.
A "Hitman Tools" option is added to the toolbar with a single option of "Build Scene".
Selecting the option presents a file opener, which you can use to select the SPK file.
To find the SPK file, go to the directory where the game in installed and then into one of the numbered subfolders for the given level and then open one of the zip files. The playable levels are the ones that don't end in "Pre" or "Laptop". In the zipfile you will find "Pack.SPK" which can be extracted and then opened with these tools.
eg the level "Kowloon Triads in Gang War" is located at: "[...]/Hitman Codename 47/C1_HongKong/C1_1.zip/Pack.SPK"

## Features
The tool is currently able to import the object tree, object names, positions and meshes. When building the scene it will create a GameObject for each item it extracts from the spk file. If the object has a mesh then it will be set on that object's MeshFilter.

When the import completes you will likely just see a lot of magenta. Currently the easiest next step is to create a new default material, filter the scene hierarchy list to MeshRenderers (t:MeshRenderer in the search box), select everything and then apply the material.

A helpful next step would be to search for and disable anything with a name containing "insidebound", "gate" or "backdrop", which will allow you to see the scene more clearly.

## Limitations
There are currently 3 main problems that will be obvious after importing a scene:
Texture & UV data is not being extracted yet, which is why a material has to be manually applied in order to see anything.
Orientation is not being extracted yet, resulting in many objects being placed at strange angles.
All character meshes will appear incorrectly.