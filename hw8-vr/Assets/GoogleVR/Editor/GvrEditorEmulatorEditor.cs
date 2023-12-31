//-----------------------------------------------------------------------
// <copyright file="GvrEditorEmulatorEditor.cs" company="Google Inc.">
// Copyright 2017 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

/// <summary>A custom editor for the `GvrEditorEmulator` script.</summary>
/// <remarks>It adds an info panel describing the camera controls.</remarks>
[CustomEditor(typeof(GvrEditorEmulator)), CanEditMultipleObjects]
public class GvrEditorEmulatorEditor : UnityEditor.Editor
{
    private const string INFO_TEXT = "Camera Controls:\n" +
                                     "   • Alt + Move Mouse = Change Yaw/Pitch\n" +
                                     "   • Ctrl + Move Mouse = Change Roll";

    private const int NUM_INFO_LINES = 3;

    private float infoHeight;

    /// <summary>A builtin method of the `Editor` class.</summary>
    /// <remarks>Implement this function to make a custom inspector.</remarks>
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Rect rect = EditorGUILayout.GetControlRect(false, infoHeight);
        GvrInfoDrawer.Draw(rect, INFO_TEXT, MessageType.None);
    }

    private void OnEnable()
    {
        infoHeight = GvrInfoDrawer.GetHeightForLines(NUM_INFO_LINES);
    }
}
