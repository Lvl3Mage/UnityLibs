using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Text;
using UnityEngine.Serialization;

[System.Serializable]
public class NoiseSampler
{
	
	//Source settings
	[Header("Source settings")]
	[SerializeField] float noiseScale;
	[SerializeField] Vector2 scrollSpeed;
	[SerializeField] Vector2 offset;

	[Tooltip("Amount of individual octaves the noise has")]
	[SerializeField] [MinAttribute(1)] int octaves;

	[Tooltip("Speed with witch the frequency of each octave grows")]
	[SerializeField] [MinAttribute(1)] float lacunarity;

	[FormerlySerializedAs("persistance")]
	[Tooltip("How much each the influence of each octave persists with octave amount")]
	[SerializeField] [Range(0,1)] float persistence;

	//
	public enum FilterType{
		None,Exponential,SCurve,Falloff
	}
	[Header("Filter settings")]
	[SerializeField] FilterType filter;
	#if UNITY_EDITOR
	[SelectableField(nameof(filter), (int)FilterType.Exponential)] 
	#endif
	[SerializeField] float exponent;
	#if UNITY_EDITOR
	[SelectableField(nameof(filter), (int)FilterType.SCurve)] 
	#endif
	[SerializeField] float slope;
	#if UNITY_EDITOR
	[SelectableField(nameof(filter), (int)FilterType.Falloff)] 
	#endif
	[SerializeField] float falloff;

	[Header("Scaling settings")]
	[SerializeField] float minValue;
	[SerializeField] float maxValue;
	float SampleSource(Vector2 position){
		Vector2 baseCoords = (position + scrollSpeed*Time.time)*noiseScale;
		float bounds = 0;
		float rawVal = 0;
		for(int i = 0; i < octaves; i++){
			float frequency = Mathf.Pow(lacunarity, i);
			float amplitude = Mathf.Pow(persistence, i);
			Vector2 noiseCoords = baseCoords*frequency +offset;
			float octaveVal = Mathf.PerlinNoise(noiseCoords.x, noiseCoords.y)*amplitude;

			rawVal += octaveVal;
			bounds += amplitude;
		}
		
		rawVal = MathUtils.TransformRange(rawVal,0,bounds, 0, 1);


		switch (filter){
			case FilterType.None:
				break;
			case FilterType.Exponential:
				rawVal = Mathf.Pow(rawVal,exponent);
				break;
			case FilterType.SCurve:
				rawVal = MathUtils.ValueActivation01(rawVal,slope);
				break;
			case FilterType.Falloff:
				rawVal = MathUtils.ValueFalloff01(rawVal, falloff);
				break;
		}
		return rawVal;
	}
	public float SampleAt(Vector2 position){
		float rawVal = SampleSource(position);
		float val = MathUtils.TransformRange(rawVal,0,1,minValue,maxValue);
		return val;
	}
	#if UNITY_EDITOR
	public float GetSourceAt(Vector2 position){
		return SampleSource(position);
	}
	#endif

}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(NoiseSampler))]
public class NoiseSamplerDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty parentProp, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, parentProp);
		float offset = position.y;

		Rect foldoutRect = LineRect(position.x, position.width, ref offset);
		parentProp.isExpanded = EditorGUI.Foldout(foldoutRect, parentProp.isExpanded, label);


		EditorGUI.indentLevel += 1;
		if(parentProp.isExpanded){
			int depth = parentProp.depth;
			var enumProp = parentProp.Copy();
			var enumerator = enumProp.GetEnumerator();
			
			while (enumerator.MoveNext())
			{
				SerializedProperty curProp = enumerator.Current as SerializedProperty;
				if (curProp == null || curProp.depth > depth + 1) continue;
				var propRect = PropertyRect(curProp,position.x, position.width, ref offset);
				EditorGUI.PropertyField(propRect, curProp, new GUIContent(curProp.displayName), true);
			}
			
			if (GUI.Button(LineRect(position.x + 15, 200, ref offset), "Randomize Offset")){
				var seedProp = parentProp.FindPropertyRelative("offset");
				seedProp.vector2Value = new Vector2(Random.Range(-100f, 100f),Random.Range(-100f, 100f));
			}
			NoiseSampler sampler = EditorHelper.SerializedPropertyToObject<NoiseSampler>(parentProp);
			Texture2D texture = new Texture2D(64,64);
			Color[] clrs = new Color[64*64];
			for(int i = 0; i < 64; i++){
				for(int j = 0; j < 64; j++){
					float val = sampler.GetSourceAt(new Vector2(i,j)/64f);
					clrs[i + j*64] = new Color(val,val,val,1);
				}
			}
			GUI.Label(LineRect(position.x+15, position.width, ref offset), "Preview");
			texture.SetPixels(clrs);
			texture.Apply();
			EditorGUI.DrawPreviewTexture(new Rect(position.x + 15, offset, 100, 100),texture);
		}
		EditorGUI.indentLevel -= 1;
		// var indent = EditorGUI.indentLevel;

		// EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}

	//This will need to be adjusted based on what you are displaying
	public override float GetPropertyHeight(SerializedProperty parentProp, GUIContent label)
	{	
		if(!parentProp.isExpanded){
			return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		}

		int depth = parentProp.depth;
		var prop = parentProp.Copy();

		int additionalFields = 3;//include parentProp field aswell
		float totalHeight = additionalFields*EditorGUIUtility.singleLineHeight + additionalFields*EditorGUIUtility.standardVerticalSpacing;
		var enumProp = parentProp.Copy();
		var enumerator = enumProp.GetEnumerator();
		
		while (enumerator.MoveNext())
		{
			SerializedProperty curProp = enumerator.Current as SerializedProperty;
			if (curProp == null || curProp.depth > depth + 1) continue;
			totalHeight += EditorGUI.GetPropertyHeight(curProp) + EditorGUIUtility.standardVerticalSpacing;
		}
		return totalHeight + 100;
	}

	static Rect PropertyRect(SerializedProperty prop, float x, float width, ref float drawHeight){
		float propHeight = EditorGUI.GetPropertyHeight(prop);
		var propRect = new Rect(x, drawHeight, width, propHeight);
		float propSizeMultiplier = Mathf.Clamp01(Mathf.Ceil(propHeight));
		drawHeight += propRect.height + EditorGUIUtility.standardVerticalSpacing*propSizeMultiplier;
		return propRect;
	}
	Rect LineRect(float x, float width, ref float drawHeight){
		var propRect = new Rect(x, drawHeight, width, EditorGUIUtility.singleLineHeight);
		drawHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		return propRect;
	}
}

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
public class SelectableField : PropertyAttribute//think of a better name bruh
{
	public readonly string enumName;
	public readonly int enumTargetValue;
	public SelectableField(string _enumName, int _enumTargetValue){
		enumName = _enumName;
		enumTargetValue = _enumTargetValue;
	}
}
[CustomPropertyDrawer(typeof(SelectableField))]
public class SelectableFieldDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		bool visible = CheckCondition(property);
		if(visible){
			var propRect = new Rect(position.x, position.y, position.width, EditorGUI.GetPropertyHeight(property));
			EditorGUI.PropertyField(propRect, property, new GUIContent(property.name), true);
		}
	}

	//This will need to be adjusted based on what you are displaying
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{	
		bool visible = CheckCondition(property);
		return visible ? EditorGUI.GetPropertyHeight(property) : 0;
	}
	bool CheckCondition(SerializedProperty property){
		SelectableField attrib = attribute as SelectableField;

		SerializedProperty enumProp = GetSiblingProperty(property, attrib.enumName);
		if(enumProp == null){
			Debug.LogError($"Cannot find property with name {attrib.enumName}");
		}
		return enumProp.enumValueIndex == attrib.enumTargetValue;
	}
	SerializedProperty GetSiblingProperty(SerializedProperty property, string siblingName){
		string propPath = property.propertyPath;
		string[] path = propPath.Split(".");
		StringBuilder pathBuilder = new StringBuilder("");
		for(int i = 0; i < path.Length-1; i++){
			pathBuilder.Append(path[i]);
			if(i != path.Length-1){
				pathBuilder.Append(".");
			}
		}
		pathBuilder.Append(siblingName);
		return property.serializedObject.FindProperty(pathBuilder.ToString());
	}
}
#endif