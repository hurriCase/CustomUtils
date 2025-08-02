using System;
using UnityEngine;

namespace CustomUtils.Runtime.Downloader
{
    [Serializable]
    public class Sheet
    {
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public long Id { get; set; }
        [field: SerializeField] public TextAsset TextAsset { get; set; }
        [field: SerializeField] public long ContentLength { get; set; }

        public bool HasChanged(long newContentLength) => ContentLength != newContentLength;
    }
}