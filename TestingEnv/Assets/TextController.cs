using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TextController : MonoBehaviour
{
	[SerializeField] string prefix = "";
	[SerializeField] string suffix = "";
	string _text;
	public string text 
	{ 
		get
		{
			return _text;
		} 
		set
		{
			_text = value;
			Set(_text);
		}
	}
	TextMeshProUGUI textMesh;
    // Start is called before the first frame update
    void Awake(){
        textMesh = gameObject.GetComponent<TextMeshProUGUI>();
    }


    public void Set(string Text){
        SetString(Text);
    }
    public void Set(int Int){
        SetString(Int.ToString());
    }
    public void Set(Color Color){
    	textMesh.color = Color;
    }
    public void Set(float Float, string format){
    	SetString(Float.ToString(format));
    }
    void SetString(string value){
    	textMesh.text = prefix + value + suffix;
    }
}
