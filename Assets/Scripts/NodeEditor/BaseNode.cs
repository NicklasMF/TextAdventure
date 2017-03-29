using System.Collections;
using UnityEngine;
using UnityEditor;

public abstract class BaseNode : ScriptableObject {

	public Rect windowRect;
	public bool hasInputs = false;
	public string windowTitle = "";

	public virtual void DrawWindow() {
		windowTitle = EditorGUILayout.TextField("Title", windowTitle);
	}

	public abstract void DrawCurves();

	public virtual void SetInput(BaseInputNode node, Vector2 clickPos) {
		
	}

	public virtual void NodeDeleted(BaseNode node) {
		
	}

	public virtual BaseInputNode ClickedOnInput(Vector2 pos) {
		return null;
	}
}
