using UnityEngine;
using DateTime = System.DateTime;

namespace Avfi
{
    static class PathUtil
    {
        public static string TemporaryDirectoryPath
          => Application.platform == RuntimePlatform.IPhonePlayer
               ? Application.temporaryCachePath : ".";

        public static string GetTimestampedFilename()
          => $"Record_{DateTime.Now:MMdd_HHmm_ss}.mp4";

        public static string GetTemporaryFilePath()
          => TemporaryDirectoryPath + "/" + GetTimestampedFilename();
    }
} // namespace Avfi
