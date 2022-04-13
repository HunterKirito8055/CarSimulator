using UnityEngine;

public class RCC_Teleporter : MonoBehaviour{

	public Transform spawnPoint;
	
	void OnTriggerEnter(Collider col){
		print("aaaaa");
		if (col.isTrigger)
			return;

		RCC_CarControllerV3 carController = col.gameObject.GetComponentInParent<RCC_CarControllerV3> ();

		if (!carController)
			return;

		//RCC.Transport (carController, spawnPoint.position, spawnPoint.rotation);
		
	}

}
