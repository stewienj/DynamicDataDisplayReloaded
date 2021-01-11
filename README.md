# DynamicDataDisplayReloaded
This project is a fork of the DynamicDataDisplayReloaded project based on Microsoft's DynamicDataDisplay project for displaying graphics in a WPF display.

## Summary
In the DynamicDataDisplayReloaded project, the data display was expanded to utilise DirectX 9 via SharpDX to show lines and markers for a large performance improvement.
For this project, the DirectX component has been further extended to enable the use of textured geometry.
Two additional demonstrations are in the current version, showing the potential for textured rectangle object performance.

## Demonstrations
The first demo can be found in the side menu under DxRectangleTextured.
This demo shows 10,000 markers textured with the distinctive DVD logo.
Pressing the *Start Animation* button animates the display by moving every icon on screen diagonally, bouncing off the defined borders.
Pressing the *Stop Animation* button ceases this animation.
Pressing the *Reset Positions* button moves all icons in the display to a new random position.

The second demo is also found on the side menu with the label DxInstancedRectangleTextured.
A major difference is the use of DirectX instancing.
Instancing allows the drawing of a large amounts of similar geometry with a smaller amount of draw calls, for a smaller CPU performance impact.
To demonstrate the power of instancing, the demo shows 100,000 of the same markers.
This demo has the same controls as the textured rectangle textures.

## New Features
*SharpDX9.DataTypes.DXVertex* is a new object that stores a position and texture coordinate for a vertex.
This is useful for defining vertices for textured shapes.

*SharpDX9.DataTypes.DXRectangle* is a helpful structure for defining the points of a rectangle with a given position and width.
It has properties to easily retrieve the coordinates of each rectangle edge.

*SharpDX9.Helpers.SharpDXHelper* contains the functions necessary to create the vertex representation of a rectangle and calculate its texture coordinates.
The MakeRectangle function will return a list of vertices from an initial position, width and height.

*SharpDX9.Helpers.TextureHelper* has helper functions for creating DirectX textures.
TextureFromStream can return a texture from a file stream.
TextureFromFile can return a texture from file.

*SharpDX9.Markers.DxMarkerRectangle* takes a collection of vertices and renders a textured rectangle.

*SharpDX9.Markers.DxMarkerRectangleInstanced* takes a collection of points and vertices and renders textured rectangles using instancing.

*SharpDX9.Shaders.DxRectangleTexturedShader* contains the vertex shader and pixel shader for a textured shape.

*SharpDX9.Shaders.DxRectangleInstancedTexturedShader* contains a vertex shader and pixel shader for textured shapes using instancing.

*SharpDX9.Triangles.DxTriangleStripTextured* gets the transform effects for a textured triangle strip.

*SharpDX9.BaseDxTexturePrimitive* holds the set of functions for defining the transform effect, vertex buffer and geometry source for the device.

*SharpDX9.BaseDxInstancedTexturePrimitive* gets transform effect, vertex buffer and geometry source for instanced geometry.

## Sources
The archive for the original DynamicDataDisplay:
https://archive.codeplex.com/?p=dynamicdatadisplay

The repository for InteractiveDataDisplay for WPF:
https://github.com/microsoft/InteractiveDataDisplay.WPF

The repository for DynamicDataDisplayReloaded:
https://github.com/stewienj/DynamicDataDisplayReloaded

The repository for SharpDX:
https://github.com/sharpdx/SharpDX