using UnityEngine.UIElements;

namespace CustomUtils.Editor.Scripts.CustomEditorUtilities
{
    internal sealed class NonReorderableListView : ListView
    {
        internal NonReorderableListView()
        {
            showFoldoutHeader = true;
            showBoundCollectionSize = false;
            virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
        }
    }
}