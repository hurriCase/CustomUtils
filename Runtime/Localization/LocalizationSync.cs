using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using ZLinq;
using Object = UnityEngine.Object;

namespace CustomUtils.Runtime.Localization
{
    /// <summary>
    ///     Downloads sheets from Google Sheet and saves them to Resources.
    /// </summary>
    [ExecuteInEditMode]
    internal sealed class LocalizationSync : MonoBehaviour
    {
        /// <summary>
        ///     Table Id on Google Sheets.
        ///     Let's say your table has the following URL
        ///     https://docs.google.com/spreadsheets/d/1RvKY3VE_y5FPhEECCa5dv4F7REJ7rBtGzQg9Z_B_DE4/edit#gid=331980525
        ///     In this case, Table Id is "1RvKY3VE_y5FPhEECCa5dv4F7REJ7rBtGzQg9Z_B_DE4" and Sheet Id is "331980525" (the gid
        ///     parameter).
        /// </summary>
        internal string TableId { get; private set; }

        /// <summary>
        ///     Table sheet contains sheet name and id. First sheet has always zero id. Sheet name is used when saving.
        /// </summary>
        internal Sheet[] Sheets { get; private set; }

        /// <summary>
        ///     Folder to save spreadsheets. Must be inside Resources folder.
        /// </summary>
        internal Object SaveFolder { get; private set; }

        private const string UrlPattern = "https://docs.google.com/spreadsheets/d/{0}/export?format=csv&gid={1}";

#if UNITY_EDITOR

        private void Awake()
        {
            Debug.LogWarning(
                "This script has been deprecated. Use LocalizationSettingsWindow or LocalizationSettings (Scriptable Object).");
        }

        /// <summary>
        ///     Sync spreadsheets.
        /// </summary>
        internal void Sync()
        {
            if (EditorUtility.DisplayDialog("Message",
                    "LocalizationSync is obsolete, please use Window/SimpleLocalization or LocalizationSettings (Scriptable Object).",
                    "Continue", "Abort") is false) return;

            StopAllCoroutines();
            StartCoroutine(SyncCoroutine());
        }

        private IEnumerator SyncCoroutine()
        {
            var folder = AssetDatabase.GetAssetPath(SaveFolder);

            Debug.Log("<color=yellow>Localization sync started...</color>");

            var dict = new Dictionary<string, UnityWebRequest>();

            foreach (var sheet in Sheets)
            {
                var url = string.Format(UrlPattern, TableId, sheet.Id);

                Debug.Log($"Downloading: {url}...");

                dict.Add(url, UnityWebRequest.Get(url));
            }

            foreach (var (url, request) in dict)
            {
                if (request.isDone is false)
                    yield return request.SendWebRequest();

                if (request.error == null)
                {
                    var sheet = Sheets.AsValueEnumerable()
                        .Single(sheet => url == string.Format(UrlPattern, TableId, sheet.Id));
                    var path = Path.Combine(folder, sheet.Name + ".csv");

                    File.WriteAllBytes(path, request.downloadHandler.data);
                    Debug.LogFormat("Sheet {0} downloaded to <color=grey>{1}</color>", sheet.Id, path);
                }
                else
                    throw new Exception(request.error);
            }

            AssetDatabase.Refresh();

            Debug.Log("<color=yellow>Localization sync completed!</color>");
        }

#endif
    }
}