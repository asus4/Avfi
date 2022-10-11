using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

sealed class Controller : MonoBehaviour
{
    public struct Metadata
    {
        public float time;
        public float mouseX;
        public float mouseY;
    }

    [SerializeField]
    RenderTexture _source = null;

    [SerializeField]
    Text _buttonLabel = null;


    Avfi.VideoRecorder _recorder = null;


    void Start()
    {
        Application.targetFrameRate = 60;

        _recorder = new Avfi.VideoRecorder(_source);
    }

    void OnDestroy()
    {
        _recorder?.Dispose();
    }

    unsafe void Update()
    {
        if (!_recorder.IsRecording) { return; }

        // Convert struct to native byte array
        var metadata = new Metadata
        {
            time = Time.time,
            mouseX = Input.mousePosition.x / Screen.width,
            mouseY = Input.mousePosition.y / Screen.height,
        };
        using var buffer = new NativeArray<byte>(UnsafeUtility.SizeOf<Metadata>(), Allocator.Temp);
        UnsafeUtility.CopyStructureToPtr(ref metadata, buffer.GetUnsafePtr());

        _recorder.Update(buffer);
    }

    public void OnPressRecordButton()
    {
        if (_recorder.IsRecording)
            _recorder.EndRecording();
        else
            _recorder.StartRecording();

        _buttonLabel.text = _recorder.IsRecording ? "Stop" : "Record";
        _buttonLabel.color = _recorder.IsRecording ? Color.red : Color.black;
    }
}
