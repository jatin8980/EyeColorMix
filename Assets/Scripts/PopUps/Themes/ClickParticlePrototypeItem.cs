using Mosframe;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickParticlePrototypeItem : UIBehaviour, IDynamicScrollViewItem
{
    public List<ClickParticleItem> clickParticleItems;

    public void onUpdateItem(int index)
    {
        clickParticleItems[0].index = (index * ThemesPopUpController.Inst.bigItemsPerRow) - 1;
        clickParticleItems[0].SetThis();

        for (int i = 1; i < clickParticleItems.Count; i++)
        {
            if (index * ThemesPopUpController.Inst.bigItemsPerRow + i < ThemesPopUpController.Inst.totalParticleItems)
            {
                clickParticleItems[i].index = index * ThemesPopUpController.Inst.bigItemsPerRow + i - 1;
                clickParticleItems[i].SetThis();
                clickParticleItems[i].gameObject.SetActive(true);
            }
            else
            {
                clickParticleItems[i].gameObject.SetActive(false);
            }
        }
    }
}
