using System.Collections.Generic;
using CustomUtils.Editor.CustomEditorUtilities;
using CustomUtils.Editor.Extensions;
using CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Scripts;
using CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Scripts.Attributes;
using CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Scripts.Helpers;
using CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Scripts.Modifiers;
using UnityEditor;
using UnityEditor.UI;
using UnityEditorInternal;
using UnityEngine;

namespace CustomUtils.Editor.UI.CustomComponents.ProceduralUIImage
{
    [CustomEditor(typeof(ProceduralImage), true)]
    [CanEditMultipleObjects]
    public sealed class ProceduralImageEditor : ImageEditor
    {
        private static List<ModifierIDAttribute> _attributes;

        private SerializedProperty _borderWidth;
        private SerializedProperty _falloffDist;
        private SerializedProperty _sprite;

        private int _selectedId;

        private EditorStateControls _editorStateControls;
        private ProceduralImage _proceduralImage;
        private Component _targetComponent;

        protected override void OnEnable()
        {
            base.OnEnable();

            _sprite = serializedObject.FindProperty("m_Sprite");

            _editorStateControls = new EditorStateControls(target, serializedObject);

            _attributes = ModifierUtility.GetAttributeList();

            _borderWidth = serializedObject.FindField(nameof(ProceduralImage.BorderRatio));
            _falloffDist = serializedObject.FindField(nameof(ProceduralImage.FalloffDistance));

            _proceduralImage = (ProceduralImage)target;
            _targetComponent = (Component)target;

            if (_proceduralImage.GetComponent<ProceduralImageModifier>())
                _selectedId = _attributes.IndexOf(((ModifierIDAttribute[])_proceduralImage
                    .GetComponent<ProceduralImageModifier>().GetType()
                    .GetCustomAttributes(typeof(ModifierIDAttribute), false))[0]);

            _selectedId = Mathf.Max(_selectedId, 0);
            EditorApplication.update -= UpdateProceduralImage;
            EditorApplication.update += UpdateProceduralImage;
        }

        private void UpdateProceduralImage()
        {
            if (target)
            {
                _proceduralImage.Update();
                return;
            }

            EditorApplication.update -= UpdateProceduralImage;
        }

        public override void OnInspectorGUI()
        {
            CheckForShaderChannelsGUI();

            serializedObject.Update();

            ProceduralImageSpriteGUI();

            _editorStateControls.PropertyField(m_Color);

            RaycastControlsGUI();

            EditorGUILayout.Space();

            ModifierGUI();

            _editorStateControls.PropertyField(_borderWidth);
            _editorStateControls.PropertyField(_falloffDist);

            serializedObject.ApplyModifiedProperties();
        }

        private void ProceduralImageSpriteGUI()
        {
            if (_sprite.hasMultipleDifferentValues)
            {
                EditorGUILayout.PropertyField(_sprite);
                return;
            }

            var sprite = (Sprite)EditorGUILayout.ObjectField("Sprite",
                EmptySpriteHelper.IsEmptySprite((Sprite)_sprite.objectReferenceValue)
                    ? null
                    : _sprite.objectReferenceValue, typeof(Sprite), false,
                GUILayout.Height(16));

            _sprite.objectReferenceValue = sprite ? sprite : EmptySpriteHelper.GetSprite();
        }

        private void CheckForShaderChannelsGUI()
        {
            var canvas = _targetComponent.GetComponentInParent<Canvas>();

            if (!canvas)
            {
                EditorVisualControls.WarningBox("There is no Canvas in parent of this object.");
                return;
            }

            if ((canvas.additionalShaderChannels
                 | AdditionalCanvasShaderChannels.TexCoord1
                 | AdditionalCanvasShaderChannels.TexCoord2
                 | AdditionalCanvasShaderChannels.TexCoord3) == canvas.additionalShaderChannels)
                return;

            EditorVisualControls.WarningBox(
                "TexCoord1,2,3 are not enabled as an additional shader channel in parent canvas." +
                " Procedural Image will not work properly");

            if (EditorVisualControls.Button("Fix: Enable TexCoord1,2,3 in Canvas: " + canvas.name) is false)
                return;

            Undo.RecordObject(canvas, "enable TexCoord1,2,3 as additional shader channels");
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1 |
                                               AdditionalCanvasShaderChannels.TexCoord2 |
                                               AdditionalCanvasShaderChannels.TexCoord3;
        }

        private void ModifierGUI()
        {
            var con = new GUIContent[_attributes.Count];
            for (var i = 0; i < con.Length; i++)
                con[i] = new GUIContent(_attributes[i].Name);

            var hasMultipleValues = CheckMultipleValues();
            var selectedIndex = hasMultipleValues ? -1 : _selectedId;
            var index = EditorGUILayout.Popup(new GUIContent("Modifier Type"), selectedIndex, con);

            if (index == selectedIndex)
                return;

            UpdateModifierType(index);
        }

        private void UpdateModifierType(int index)
        {
            _selectedId = index;
            foreach (var item in targets)
            {
                var proceduralImage = (ProceduralImage)item;
                var modifierType = ModifierUtility.GetTypeWithId(_attributes[_selectedId].Name);
                proceduralImage.SetModifierType(modifierType);

                var imageModifier = proceduralImage.GetComponent<ProceduralImageModifier>();
                MoveComponentBehind(proceduralImage, imageModifier);
            }

            GUIUtility.ExitGUI();
        }

        private bool CheckMultipleValues()
        {
            if (targets.Length <= 1)
                return false;

            var firstImage = (ProceduralImage)targets[0];
            var firstImageModifier = firstImage.GetComponent<ProceduralImageModifier>().GetType();
            foreach (var item in targets)
            {
                var proceduralImage = (ProceduralImage)item;
                if (proceduralImage.GetComponent<ProceduralImageModifier>().GetType() == firstImageModifier)
                    continue;

                return true;
            }

            return false;
        }

        public override string GetInfoString()
            => $"Modifier: {_attributes[_selectedId].Name}, Line-Weight: {_proceduralImage.BorderRatio.Value}";

        private static void MoveComponentBehind(Component reference, Component componentToMove)
        {
            if (!reference || !componentToMove || reference.gameObject != componentToMove.gameObject)
                return;

            var components = reference.GetComponents<Component>();
            var list = new List<Component>();
            list.AddRange(components);
            var i = list.IndexOf(componentToMove) - list.IndexOf(reference);
            while (i != 1)
            {
                switch (i)
                {
                    case < 1:
                        ComponentUtility.MoveComponentDown(componentToMove);
                        i++;
                        break;
                    case > 1:
                        ComponentUtility.MoveComponentUp(componentToMove);
                        i--;
                        break;
                }
            }
        }
    }
}