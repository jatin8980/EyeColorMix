using Mosframe;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PatternPrototypeItem : UIBehaviour, IDynamicScrollViewItem
{
    public List<PatternShopItem> patternItems;

    public void onUpdateItem(int index)
    {
        patternItems[0].SetIndex(index * ThemesPopUpController.Inst.smallItemsPerRow);
        patternItems[0].SetThis();

        for (int i = 1; i < patternItems.Count; i++)
        {
            if (index * ThemesPopUpController.Inst.smallItemsPerRow + i < ThemesPopUpController.Inst.totalPatternItems)
            {
                patternItems[i].SetIndex(index * ThemesPopUpController.Inst.smallItemsPerRow + i);
                patternItems[i].SetThis();
                patternItems[i].gameObject.SetActive(true);
            }
            else
            {
                patternItems[i].gameObject.SetActive(false);
            }
        }
    }
}
