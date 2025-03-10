using Mosframe;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PupilPrototypeItem : UIBehaviour, IDynamicScrollViewItem
{
    public List<PupilShopItem> pupilItems;

    public void onUpdateItem(int index)
    {
        pupilItems[0].SetIndex(index * ShopPopUpController.Inst.smallItemsPerRow);
        pupilItems[0].SetThis();

        for (int i = 1; i < pupilItems.Count; i++)
        {
            if (index * ShopPopUpController.Inst.smallItemsPerRow + i < ShopPopUpController.Inst.totalPupilItems)
            {
                pupilItems[i].SetIndex(index * ShopPopUpController.Inst.smallItemsPerRow + i);
                pupilItems[i].SetThis();
                pupilItems[i].gameObject.SetActive(true);
            }
            else
            {
                pupilItems[i].gameObject.SetActive(false);
            }
        }
    }
}
