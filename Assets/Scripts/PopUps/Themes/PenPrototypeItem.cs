using Mosframe;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PenPrototypeItem : UIBehaviour, IDynamicScrollViewItem
{
    public List<PenItem> penItems;

    public void onUpdateItem(int index)
    {
        penItems[0].index = index * ThemesPopUpController.Inst.bigItemsPerRow;
        penItems[0].SetThis();

        for (int i = 1; i < penItems.Count; i++)
        {
            if (index * ThemesPopUpController.Inst.bigItemsPerRow + i < ThemesPopUpController.Inst.totalPenItems)
            {
                penItems[i].index = index * ThemesPopUpController.Inst.bigItemsPerRow + i;
                penItems[i].SetThis();
                penItems[i].gameObject.SetActive(true);
            }
            else
            {
                penItems[i].gameObject.SetActive(false);
            }
        }
    }
}