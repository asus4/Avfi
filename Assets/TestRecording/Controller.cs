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

    [SerializeField] Text _buttonLabel = null;

    Avfi.VideoRecorder _recorder = null;

    NativeArray<byte> _metadata;


    void Start()
    {
        Application.targetFrameRate = 60;
        _recorder = GetComponent<Avfi.VideoRecorder>();

        _metadata = new NativeArray<byte>(UnsafeUtility.SizeOf<Metadata>(), Allocator.Persistent);
        _recorder.Metadata = _metadata;
    }

    void OnDestroy()
    {
        _metadata.Dispose();
    }

    unsafe void Update()
    {
        // Convert struct to native byte array
        var metadata = new Metadata
        {
            time = Time.time,
            mouseX = Input.mousePosition.x / Screen.width,
            mouseY = Input.mousePosition.y / Screen.height,
        };
        UnsafeUtility.CopyStructureToPtr(ref metadata, _metadata.GetUnsafePtr());

        // Get struct from native byte array
        // var metadata2 = (Metadata*)_metadata.GetUnsafePtr();
        // Debug.Log($"{metadata2->time} {metadata2->mouseX} {metadata2->mouseY}");
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
