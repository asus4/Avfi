using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using IntPtr = System.IntPtr;

namespace Avfi
{
    public class MetadataPlayer : System.IDisposable
    {
        NativeArray<byte> _buffer;

        public MetadataPlayer(string path)
        {
            Plugin.LoadMetadata(path);
            uint size = Plugin.GetBufferSize();
            _buffer = new NativeArray<byte>((int)size, Allocator.Persistent);
        }

        public void Dispose()
        {
            Plugin.UnloadMetadata();
            _buffer.Dispose();
        }

        public unsafe NativeSlice<byte> PeekMetadata(double time)
        {
            var ptr = (IntPtr)NativeArrayUnsafeUtility.GetUnsafePtr(_buffer);
            uint size = Plugin.PeekMetadata(time, ptr);
            return new NativeSlice<byte>(_buffer, 0, (int)size);
        }
    }
}
