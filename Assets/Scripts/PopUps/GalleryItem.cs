using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GalleryItem : MonoBehaviour
{
    private int index;
    [SerializeField] private Image icon, bodyImage;
    [SerializeField] private RectTransform eyeParentRt, bodyRT;


    private void Start()
    {
        eyeParentRt.localScale = Vector3.one * (GetComponent<RectTransform>().rect.width / 372);
    }

    internal void SetThis(int index)
    {
        this.index = index;
        Texture2D tex = new Texture2D(0, 0);
        tex.LoadImage(File.ReadAllBytes(Application.persistentDataPath + "/.Eyes/" + (index + 1) + ".png"));
        icon.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new(0.5f, 0.5f));
        bodyImage.sprite = GalleryPopUp.Inst.rightEyes[GalleryPopUp.Inst.listOfCharacterPlayedInLevel[index]];
        bodyRT.sizeDelta = GalleryPopUp.Inst.characterSizes[GalleryPopUp.Inst.listOfCharacterPlayedInLevel[index]];
        bodyRT.anchoredPosition = GalleryPopUp.Inst.characterPoses[GalleryPopUp.Inst.listOfCharacterPlayedInLevel[index]];
    }

    public void On_Btn_Click()
    {
        GameManager.Inst.Show_Popup(GameManager.Popups.GalleryDetailPopUp, false);
        FindAnyObjectByType<GalleryDetailPopUp>().SetThis(icon.sprite, index);
    }
}
