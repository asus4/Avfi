Avfi
====

![gif](https://user-images.githubusercontent.com/343936/142568294-4cef6937-654c-4cfd-9d9e-032a4c84674e.gif)
![gif](https://user-images.githubusercontent.com/343936/142568320-d6285d24-adeb-4997-8ae6-5038288f4ce8.gif)

**Avfi** is a Unity plugin allowing an application to record video clips with
a simple operation.

Avfi uses AVFoundation as a video encoding backend, so it only runs on the
Apple platforms (macOS/iOS).

[AVFoundation]: https://developer.apple.com/av-foundation/

How To Install
--------------

This package uses the [scoped registry] feature to resolve package
dependencies. Add the following lines to the manifest file
(`Packages/manifest.json`).

[scoped registry]: https://docs.unity3d.com/Manual/upm-scoped.html

To the `scopedRegistries` section:

```
{
  "name": "Keijiro",
  "url": "https://registry.npmjs.com",
  "scopes": [ "jp.keijiro" ]
}
```

To the `dependencies` section:

```
"jp.keijiro.avfi": "1.0.3"
```

After the changes, the manifest file should look like:

```
{
  "scopedRegistries": [
    {
      "name": "Keijiro",
      "url": "https://registry.npmjs.com",
      "scopes": [ "jp.keijiro" ]
    }
  ],
  "dependencies": {
    "jp.keijiro.avfi": "1.0.3",
    ...
```
