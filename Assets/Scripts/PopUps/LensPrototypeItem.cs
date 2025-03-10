using Mosframe;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class LensPrototypeItem : UIBehaviour, IDynamicScrollViewItem
{
    public List<LensItem> lensItems;

    public void onUpdateItem(int index)
    {
        lensItems[0].index = index * ShopPopUpController.Inst.bigItemsPerRow;
        lensItems[0].SetThis();

        for (int i = 1; i < lensItems.Count; i++)
        {
            if (index * ShopPopUpController.Inst.bigItemsPerRow + i < ShopPopUpController.Inst.totalLensItems)
            {
                lensItems[i].index = index * ShopPopUpController.Inst.bigItemsPerRow + i;
                lensItems[i].SetThis();
                lensItems[i].gameObject.SetActive(true);
            }
            else
            {
                lensItems[i].gameObject.SetActive(false);
            }
        }
    }
}
