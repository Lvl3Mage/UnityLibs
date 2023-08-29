using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LevelManagerShortcut : MonoBehaviour
{
	public void LoadLevel(int levelIndex){
		LevelManager.LoadScene(levelIndex);
	}
	public void ReloadLevel(){
		LevelManager.ReloadScene();
	}
	public void LoadNextLevel(){
		LevelManager.LoadNextScene();
	}
}
