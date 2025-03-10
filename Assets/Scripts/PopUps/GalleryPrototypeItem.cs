using Mosframe;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class GalleryPrototypeItem : UIBehaviour, IDynamicScrollViewItem
{
    public List<GalleryItem> galleryItems;

    public void onUpdateItem(int index)
    {
        galleryItems[0].SetThis(index * GalleryPopUp.Inst.bigItemsPerRow);
        for (int i = 1; i < galleryItems.Count; i++)
        {
            if (index * GalleryPopUp.Inst.bigItemsPerRow + i < GalleryPopUp.Inst.listOfCharacterPlayedInLevel.Count)
            {
                galleryItems[i].SetThis(index * GalleryPopUp.Inst.bigItemsPerRow + i);
                galleryItems[i].gameObject.SetActive(true);
            }
            else
            {
                galleryItems[i].gameObject.SetActive(false);
            }
        }
    }
}
