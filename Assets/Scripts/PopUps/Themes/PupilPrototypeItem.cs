using Mosframe;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PupilPrototypeItem : UIBehaviour, IDynamicScrollViewItem
{
    public List<PupilShopItem> pupilItems;

    public void onUpdateItem(int index)
    {
        pupilItems[0].SetIndex(index * ThemesPopUpController.Inst.smallItemsPerRow);
        pupilItems[0].SetThis();

        for (int i = 1; i < pupilItems.Count; i++)
        {
            if (index * ThemesPopUpController.Inst.smallItemsPerRow + i < ThemesPopUpController.Inst.totalPupilItems)
            {
                pupilItems[i].SetIndex(index * ThemesPopUpController.Inst.smallItemsPerRow + i);
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
