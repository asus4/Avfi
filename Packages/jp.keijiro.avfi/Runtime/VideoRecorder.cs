using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using IntPtr = System.IntPtr;

namespace Avfi
{
    public sealed class VideoRecorder : System.IDisposable
    {
        #region Editable attributes

        private RenderTexture _source = null;

        public RenderTexture Source
        {
            get => _source;
            set => ChangeSource(value);
        }

        #endregion

        #region Public properties and methods

        public bool IsRecording { get; private set; }

        public VideoRecorder(RenderTexture source)
        {
            ChangeSource(source);
        }

        public void Dispose()
        {
            if (IsRecording)
            {
                EndRecording();
            }
            Object.Destroy(_buffer);
        }

        public void Update(NativeArray<byte> metadata)
        {
            if (!IsRecording) { return; }
            if (!_metaQueue.TryEnqueueNow(metadata)) { return; }

            Graphics.Blit(_source, _buffer, new Vector2(1, -1), new Vector2(0, 1));
            AsyncGPUReadback.Request(_buffer, 0, OnSourceReadback);
        }

        public void StartRecording()
        {
            var path = PathUtil.GetTemporaryFilePath();
            Plugin.StartRecording(path, _source.width, _source.height);

            _metaQueue.Clear();
            IsRecording = true;
        }

        public void EndRecording()
        {
            AsyncGPUReadback.WaitAllRequests();
            Plugin.EndRecording();
            IsRecording = false;
        }

        #endregion

        #region Private objects

        private RenderTexture _buffer;
        private readonly MetaQueue _metaQueue = new();

        private void ChangeSource(RenderTexture rt)
        {
            if (IsRecording)
            {
                Debug.LogError("Can't change the source while recording.");
                return;
            }

            if (_buffer != null)
            {
                Object.Destroy(_buffer);
            }

            _source = rt;
            _buffer = new RenderTexture(rt.width, rt.height, 0);
        }

        #endregion

        #region Async GPU readback

        private unsafe void OnSourceReadback(AsyncGPUReadbackRequest request)
        {
            if (!IsRecording)
            {
                return;
            }

            // Get pixel buffer
            var data = request.GetData<byte>(0);
            var ptr = (IntPtr)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(data);

            var (time, metadata) = _metaQueue.Dequeue();
            if (metadata.IsCreated)
            {
                var metadataPtr = (IntPtr)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(metadata);
                Plugin.AppendFrame(ptr, (uint)data.Length, metadataPtr, (uint)metadata.Length, time);
                metadata.Dispose();
            }
            else
            {
                Plugin.AppendFrame(ptr, (uint)data.Length, IntPtr.Zero, 0, time);
            }
        }

        #endregion
    }

} // namespace Avfi
