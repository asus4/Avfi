using System.IO;
using Avfi;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public sealed class MetadataViewer : MonoBehaviour
{
    [SerializeField] Text _label = null;

    [SerializeField] VideoPlayer _player = null;

    [SerializeField] string _videoPath = "sample.mp4";
    MetadataPlayer _metadataPlayer = null;

    private void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, _videoPath);
        _player.url = path;
        _metadataPlayer = new MetadataPlayer(path);
    }

    private void OnDestroy()
    {
        _metadataPlayer?.Dispose();
    }

    private unsafe void Update()
    {
        var slice = _metadataPlayer.PeekMetadata(_player.time);

        var ptr = slice.GetUnsafeReadOnlyPtr();
        UnsafeUtility.CopyPtrToStructure(ptr, out Controller.Metadata metadata);
        _label.text = $"time:{metadata.time:F2}\nmouse: ({metadata.mouseX:F2}, {metadata.mouseY:F2})";
    }
}
