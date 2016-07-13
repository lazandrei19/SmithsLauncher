using UnityEngine;
using System.Collections;

public class Tasks : MonoBehaviour
{

	public static void Initialize () {
		GameController.control.SetInt ("currency", 15);
		GameController.control.SetBool ("sold", false);
		Materials.InitializeMaterials ();
		if (Application.platform == RuntimePlatform.Android)
			Screen.SetResolution (1024, 576, false);
	}
}

