using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class IAPRestorePurchasesButton : MonoBehaviour
{
	#region Unity Methods

	private void Start()
	{
		//Restore Purchased Btn Handle      -------Temporery disabled 
		gameObject.SetActive(Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer);
		gameObject.GetComponent<Button>().onClick.AddListener(IAPManager.inst.RestorePurchases);
	}

	#endregion
}