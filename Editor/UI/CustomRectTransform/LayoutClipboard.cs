﻿#if IS_RECTTRANSFORM_EXTENDED_ENABLED
using System;
using MemoryPack;
using UnityEngine;

namespace CustomUtils.Editor.UI.CustomRectTransform
{
    internal static class LayoutClipboard
    {
        private static byte[] _clipboardData;

        internal static bool HasData { get; private set; }

        internal static void Copy(LayoutData layoutData)
        {
            try
            {
                _clipboardData = MemoryPackSerializer.Serialize(layoutData);
                HasData = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[LayoutClipboard::Copy] Failed to copy layout data: {e.Message}");
                HasData = false;
            }
        }

        internal static bool TryPaste(out LayoutData layoutData)
        {
            layoutData = default;

            if (HasData is false || _clipboardData == null)
                return false;

            try
            {
                layoutData = MemoryPackSerializer.Deserialize<LayoutData>(_clipboardData);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to paste layout data: {e.Message}");
                return false;
            }
        }
    }
}
#endif