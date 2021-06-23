using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class MeshGenerator : EditorWindow {
	[MenuItem("Editor/MeshGenerator")]
	static void Open() {
		GetWindow<MeshGenerator>("Generator");
	}

	//string path_name;

	Mesh referenceMesh;
	int N;
	bool rename;
	string renameStr;
	bool appendBounce;
	Vector3 bounceSize;

	void OnGUI() {
		//path_name = EditorGUILayout.TextField("Path_Name", path_name);
		N = EditorGUILayout.IntField("Num", N);
		rename = EditorGUILayout.Toggle("Rename?", rename);
		if (rename)
			renameStr = EditorGUILayout.TextField("Rename Name", renameStr);
		appendBounce = EditorGUILayout.Toggle("Set Bounce?", appendBounce);
		if (appendBounce)
			bounceSize = EditorGUILayout.Vector3Field("Bounce Size", bounceSize);
		if (GUILayout.Button("Generate Point Polygons")) {
			GeneratePointPolygons(); }
		referenceMesh = (Mesh) EditorGUILayout.ObjectField("Reference Mesh", referenceMesh, typeof(Mesh), false);
		if (GUILayout.Button("Mesh N Copy")) {
			MeshNCopy(); }
		if (GUILayout.Button("Append Bounce")) {
			AppendBounce(); }
	}

	StringBuilder builder = new StringBuilder();
	(bool usable, string dirPath, string filename) GetAssetPath(Object asset)
	{
		var assetPath = AssetDatabase.GetAssetPath(asset);
		var assetPathSplit = assetPath.Split('/');
		var assetPathSplitRev = assetPathSplit.Reverse();
		var dirPathSplit = assetPathSplitRev.Skip(1).Reverse();
		var fileNameSplit = assetPathSplitRev.Take(1);
		builder.Clear();
		foreach (var str in dirPathSplit)
		{
			builder.Append(str);
			builder.Append('/');
		}
		builder.Remove(builder.Length - 1, 1);
		var dirPath = builder.ToString();
		var fileName = fileNameSplit.Any() ? fileNameSplit.First() : "";
		return (dirPathSplit.FirstOrDefault() == "Asset", dirPath, fileName);
	}

	void AppendBounce(Mesh mesh)
	{
		var vertices = new List<Vector3>();
		var indices = new List<int>();
		vertices.AddRange(mesh.vertices);
		indices.AddRange(mesh.GetIndices(0));
		for (int i = 0; i < 8; i++)
		{
			var x = (i % 2) - 0.5f;
			var y = ((i / 2) % 2) - 0.5f;
			var z = (i / 4) - 0.5f;
			vertices.Add(Vector3.Scale(bounceSize, new Vector3(x, y, z)));
			switch (mesh.GetTopology(0)) {
				case MeshTopology.Points:
					indices.Add(vertices.Count-1); break;
				case MeshTopology.Triangles:
					indices.Add(vertices.Count-1); 
					indices.Add(vertices.Count-1); 
					indices.Add(vertices.Count-1); break;
			};
		}
		mesh.vertices = vertices.ToArray();
		mesh.SetIndices(indices.ToArray(), mesh.GetTopology(0), 0);
	}

	void GeneratePointPolygons()
	{
    var vertices = new List<Vector3>();
    var uv = new List<Vector4>();
		var indices = new List<int>();
		for (int i = 0; i < N; i++)
		{
			vertices.Add(new Vector3(0,0,0));
			indices.Add(i);
			uv.Add(new Vector4(0,0,i,0));
		}
		var mesh = new Mesh();
		mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		mesh.vertices = vertices.ToArray();
		mesh.SetUVs(0, uv);
		mesh.SetIndices(indices.ToArray(), MeshTopology.Points, 0);
		if (appendBounce) AppendBounce(mesh);
		string directory = "";
		bool assetPathUsable = false;
		if (referenceMesh != null)
		{
			var (usable, dirPath, filename) = GetAssetPath(referenceMesh); 
			directory = dirPath;
			assetPathUsable = usable;
		}
		if (!assetPathUsable) {
			var projectWindowUtilType = typeof(ProjectWindowUtil);
			var getActiveFolderPathMethod = projectWindowUtilType.GetMethod("GetActiveFolderPath", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			string pathToCurrentFolder = getActiveFolderPathMethod.Invoke(null, new object[0]).ToString();
			directory = pathToCurrentFolder;
		}
    string path = string.Format($"{directory}/{N}Polygons{(appendBounce ? $"_BX{bounceSize.x}Y{bounceSize.y}Z{bounceSize.z}" : "")}.asset");
    AssetDatabase.CreateAsset(mesh, path);
    AssetDatabase.SaveAssets();
	}

	void MeshNCopy()
	{
		var vertices = new List<Vector3>();
		var uv = new List<Vector4>();
		var uv2 = new List<Vector4>();
		var uv3 = new List<Vector4>();
		var uv4 = new List<Vector4>();
		var triangles = new List<int>();
		var normals = new List<Vector3>();
		var tangents = new List<Vector4>();
		var colors = new List<Color>();
		for (int i = 0; i < N; i++)
		{
			vertices.AddRange(referenceMesh.vertices);
			uv.AddRange(referenceMesh.uv.Select(v => new Vector4(v.x, v.y, i, 0)));
			uv2.AddRange(referenceMesh.uv2.Select(v => new Vector4(v.x, v.y, 0, 0)));
			uv3.AddRange(referenceMesh.uv3.Select(v => new Vector4(v.x, v.y, 0, 0)));
			uv4.AddRange(referenceMesh.uv4.Select(v => new Vector4(v.x, v.y, 0, 0)));
			triangles.AddRange(referenceMesh.triangles
				.Select(index => index + i * referenceMesh.vertexCount));
			normals.AddRange(referenceMesh.normals);
			tangents.AddRange(referenceMesh.tangents);
			colors.AddRange(referenceMesh.colors);
		}
		var mesh = new Mesh();
		mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		mesh.vertices = vertices.ToArray();
		mesh.SetUVs(0, uv);
		mesh.SetUVs(1, uv2);
		mesh.SetUVs(2, uv3);
		mesh.SetUVs(3, uv4);
		mesh.triangles = triangles.ToArray();
		mesh.normals = normals.ToArray();
		mesh.tangents = tangents.ToArray();
		mesh.colors = colors.ToArray();
		if (appendBounce) AppendBounce(mesh);
		var (assetPathUsable, dirPath, filename) = GetAssetPath(referenceMesh);
		var directory = dirPath;
		if (!assetPathUsable) {
			var projectWindowUtilType = typeof(ProjectWindowUtil);
			var getActiveFolderPathMethod = projectWindowUtilType.GetMethod("GetActiveFolderPath", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			string pathToCurrentFolder = getActiveFolderPathMethod.Invoke(null, new object[0]).ToString();
			directory = pathToCurrentFolder;
		}
		var name = rename ? renameStr : referenceMesh.name;
    string path = string.Format($"{directory}/{name}x{N}Polygons{(appendBounce ? $"_BX{bounceSize.x}Y{bounceSize.y}Z{bounceSize.z}" : "")}.asset");
		AssetDatabase.CreateAsset(mesh, path);
		AssetDatabase.SaveAssets();
	}

	void AppendBounce()
	{
		var mesh = new Mesh();
		mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		mesh.vertices = referenceMesh.vertices;
		mesh.SetUVs(0, referenceMesh.uv.ToList());
		mesh.SetUVs(1, referenceMesh.uv2.ToList());
		mesh.SetUVs(2, referenceMesh.uv3.ToList());
		mesh.SetUVs(3, referenceMesh.uv4.ToList());
		mesh.triangles = referenceMesh.triangles;
		mesh.normals = referenceMesh.normals;
		mesh.tangents = referenceMesh.tangents;
		mesh.colors = referenceMesh.colors;
		if (appendBounce) AppendBounce(mesh);
		var (assetPathUsable, dirPath, filename) = GetAssetPath(referenceMesh);
		var directory = dirPath;
		if (!assetPathUsable) {
			var projectWindowUtilType = typeof(ProjectWindowUtil);
			var getActiveFolderPathMethod = projectWindowUtilType.GetMethod("GetActiveFolderPath", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			string pathToCurrentFolder = getActiveFolderPathMethod.Invoke(null, new object[0]).ToString();
			directory = pathToCurrentFolder;
		}
		var name = rename ? renameStr : referenceMesh.name;
    string path = string.Format($"{directory}/{name}{(appendBounce ? $"_BX{bounceSize.x}Y{bounceSize.y}Z{bounceSize.z}" : "")}.asset");
		AssetDatabase.CreateAsset(mesh, path);
		AssetDatabase.SaveAssets();
	}
}
#endif
