/* 
Scene Loader Window is a simple Editor Tool that provides you with easy access to all the scenes in your project. 
You can choose between OpenScene and OpenSceneAdditive, and the tool will open your scene accordingly. 
NOTES: 
- Make sure this script is in the Editor folder.
- The scenes do not need to be added in Build Settings.
*/
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneLoaderWindow : EditorWindow {

	string[] allSceneGUIDs; // Array to store the GUIDs of all the scene assets found.
	GUIStyle guiStyle; // GUIStyle to customize the look of the LabelField
	GUIContent guiButtonContent; // GUIContent to customize the content of Buttons
	Vector2 scrollPosition; // Position of the scroll view
	int OpenMode; // OpenSceneMode as a number. 0 = Single, 1 = Additive.

	[MenuItem("My Tools/Scene Loader")]
	public static void ShowLevelLoaderWindow()
	{
		EditorWindow.GetWindow(typeof(SceneLoaderWindow), false, "Scene Loader");
	}

	void OnEnable()
	{
		// Setting up the GUIStyle to be used later.
		guiStyle = new GUIStyle();
		guiStyle.alignment = TextAnchor.MiddleCenter;
		guiStyle.fontSize = 18;
		guiStyle.fontStyle = FontStyle.Bold;

		// Setting up the GUIContent to be used later.
		guiButtonContent = new GUIContent();

		scrollPosition = new Vector2(0,0);
	}

	void OnGUI()
	{
		EditorGUILayout.LabelField("All Scenes", guiStyle); // Label saying "All Scenes", just for aesthetics
		
		// Radio Buttons allowing user to choose between OpenScene and OpenSceneAdditive
		OpenMode = GUILayout.SelectionGrid(OpenMode, 
			new string[2] {"OpenScene","OpenSceneAdditive"}, 2, EditorStyles.toggle);
		
		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition); // Begin scroll view
		CreateAllButtons();
		EditorGUILayout.EndScrollView(); // End scroll view
		Repaint(); // Repaint Window so that any changes (adding, deleting or renaming scenes) that have been made show up.	
	}

	void CreateAllButtons()
	{
		// Find all the Scene Assets in Project and store their GUIDs. This line can be put in OnEnable() if you don't
		// want the script to look for assets each time OnGUI is called. I put this line here so that if a scene is 
		// added, deleted or renamed in the project, the changes show up without having to reopen the SceneLoaderWindow.
		allSceneGUIDs = AssetDatabase.FindAssets("t:scene"); 
		
		char[] separators = {'/','.'}; // Array to store separators that will be used later for string splitting

		// For each SceneGUID in allSceneGUIDs
		for(int i = 0; i < allSceneGUIDs.Length; i++)
		{
			string scenePath = AssetDatabase.GUIDToAssetPath(allSceneGUIDs[i]); // Convert GUID to asset path
			string[] separatedAssetPath = scenePath.Split(separators[0]); // Split path into multiple strings based on the '/' separator
			
			// Read last cell of separatedAssetPath array to get scene name with file extension. Then split the scene name and the file
			// extension, and store the scene name into sceneName variable.
			string sceneName = separatedAssetPath[separatedAssetPath.Length-1].Split(separators[1])[0];

			guiButtonContent.text = sceneName; // Scene's name
			guiButtonContent.tooltip = "Path: " + scenePath; // Full path starting from Asset's folder

			if(GUILayout.Button(guiButtonContent)) EditorSceneManager.OpenScene(scenePath,
				(OpenMode == 0) ? OpenSceneMode.Single : OpenSceneMode.Additive);
		}
	}
}
