using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NodeEditor : EditorWindow {

	private List<BaseNode> windows = new List<BaseNode>();

	Vector2 mousePos;

	BaseNode selectedNode;

	bool makeTransitionNode = false;

	[MenuItem("Window/Node Editor")]
	static void ShowEditor() {
		NodeEditor editor = EditorWindow.GetWindow<NodeEditor>();
	}

	void OnGUI() {
		Event e = Event.current;

		mousePos = e.mousePosition;

		if (e.button == 1 && !makeTransitionNode) {
			if (e.type == EventType.MouseDown) {
				bool clickedOnWindow = false;
				int selectedIndex = -1;

				for (int i = 0; i < windows.Count; i++) {
					if (windows[i].windowRect.Contains(mousePos)) {
						selectedIndex = i;
						clickedOnWindow = true;
						break;
					}
				}

				if (!clickedOnWindow) {
					GenericMenu menu = new GenericMenu();
					menu.AddItem(new GUIContent("Add Input Node"), false, ContextCallback, "inputNode");
					menu.AddItem(new GUIContent("Add Output Node"), false, ContextCallback, "outputNode");
					menu.AddItem(new GUIContent("Add Calculation Node"), false, ContextCallback, "calcNode");

					menu.ShowAsContext();
					e.Use();
				} else {
					GenericMenu menu = new GenericMenu();

					menu.AddItem(new GUIContent("Make Transition"), false, ContextCallback, "makeTransition");
					menu.AddSeparator("");
					menu.AddItem(new GUIContent("Delete Node"), false, ContextCallback, "deleteNode");

					menu.ShowAsContext();
					e.Use();
				}
			}
		} else if(e.button == 0 && e.type == EventType.MouseDown && makeTransitionNode) {
			bool clickedOnWindow = false;
			int selectedIndex = -1;

			for (int i = 0; i < windows.Count; i++) {
				if (windows[i].windowRect.Contains(mousePos)) {
					selectedIndex = i;
					clickedOnWindow = true;
					break;
				}
			}

			if (clickedOnWindow && !windows[selectedIndex].Equals(selectedNode)) {
				windows[selectedIndex].SetInput((BaseInputNode) selectedNode, mousePos);
				makeTransitionNode = false;
				selectedNode = null;
			}

			if (!clickedOnWindow) {
				makeTransitionNode = false;
				selectedNode = null;
			}

			e.Use();
		} else if(e.button == 0 && e.type == EventType.MouseDown & !makeTransitionNode) {
			bool clickedOnWindow = false;
			int selectedIndex = -1;

			for (int i = 0; i < windows.Count; i++) {
				if (windows[i].windowRect.Contains(mousePos)) {
					selectedIndex = i;
					clickedOnWindow = true;
					break;
				}
			}
			if (clickedOnWindow) {
				BaseInputNode nodeToChange = windows[selectedIndex].ClickedOnInput(mousePos);

				if (nodeToChange != null) {
					selectedNode = nodeToChange;
					makeTransitionNode = true;
				}
			}
		}

		if (makeTransitionNode && selectedNode != null) {
			Rect mouseRect = new Rect(e.mousePosition.x, e.mousePosition.y, 10, 10);
			DrawNodeCurve(selectedNode.windowRect, mouseRect);

			Repaint();
		}

		foreach(BaseNode n in windows) {
			n.DrawCurves();
		}

		BeginWindows();

		for (int i = 0; i < windows.Count; i++) {
			windows[i].windowRect = GUI.Window(i, windows[i].windowRect, DrawNodeWindow, windows[i].windowTitle);
		}

		EndWindows();
	}

	void DrawNodeWindow(int id) {
		windows[id].DrawWindow();
		GUI.DragWindow();
	}

	void ContextCallback(object obj) {
		string clb = obj.ToString();

		if (clb.Equals("inputNode")) {
			InputNode inputNode = new InputNode();
			inputNode.windowRect = new Rect(mousePos.x, mousePos.y, 200, 150);

			windows.Add(inputNode);
		} else if (clb.Equals("outputNode")) {
			OutputNode outputNode = new OutputNode();
			outputNode.windowRect = new Rect(mousePos.x, mousePos.y, 200, 150);

			windows.Add(outputNode);
		} else if (clb.Equals("calcNode")) {
			CalcNode calcNode = new CalcNode();
			calcNode.windowRect = new Rect(mousePos.x, mousePos.y, 200, 150);

			windows.Add(calcNode);

		} else if (clb.Equals("makeTransition")) {
			bool clickedOnWindow = false;
			int selectedIndex = -1;

			for (int i = 0; i < windows.Count; i++) {
				if (windows[i].windowRect.Contains(mousePos)) {
					selectedIndex = i;
					clickedOnWindow = true;
					break;
				}
			}

			if (clickedOnWindow) {
				selectedNode = windows[selectedIndex];
				makeTransitionNode = true;
			}
		} else if (clb.Equals("deleteNode")) {
			bool clickedOnWindow = false;
			int selectedIndex = -1;

			for (int i = 0; i < windows.Count; i++) {
				if (windows[i].windowRect.Contains(mousePos)) {
					selectedIndex = i;
					clickedOnWindow = true;
					break;
				}
			}

			if (clickedOnWindow) {
				BaseNode selNode = windows[selectedIndex];
				windows.RemoveAt(selectedIndex);

				foreach(BaseNode n in windows) {
					n.NodeDeleted(selNode);
				}
			}
		}
	}

	public static void DrawNodeCurve(Rect start, Rect end) {
		Vector3 startPos = new Vector3(start.x + start.width /2, start.y + start.height / 2, 0);
		Vector3 endPos = new Vector3(end.x + end.width / 2, end.y + end.height / 2, 0);
		Vector3 startTan = startPos + Vector3.right * 50;
		Vector3 endTan = endPos + Vector3.left * 50;
		Color shadowCol = new Color(0,0,0, 0.06f);

		for (int i = 0; i < 3; i++) {
			Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i+1) * 5);
		}

		Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
	}
}
