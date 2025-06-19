using Mosframe;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class LensPrototypeItem : UIBehaviour, IDynamicScrollViewItem
{
    public List<LensItem> lensItems;

    public void onUpdateItem(int index)
    {
        lensItems[0].index = index * ThemesPopUpController.Inst.bigItemsPerRow;
        lensItems[0].SetThis();

        for (int i = 1; i < lensItems.Count; i++)
        {
            if (index * ThemesPopUpController.Inst.bigItemsPerRow + i < ThemesPopUpController.Inst.totalLensItems)
            {
                lensItems[i].index = index * ThemesPopUpController.Inst.bigItemsPerRow + i;
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
