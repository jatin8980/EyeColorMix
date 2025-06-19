/*
 * DynamicScrollView.cs
 * 
 * @author mosframe / https://github.com/mosframe
 * 
 */

namespace Mosframe
{

    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System.Collections;
    using System.Linq;
    using System;

    /// <summary>
    /// Dynamic Scroll View
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public abstract class DynamicScrollView : UIBehaviour
    {

        public int totalItemCount = 99;
        public RectTransform itemPrototype = null;
        private int toAddMarginForScrollDown = 0;
        public void scrollToLastPos()
        {

            this.contentAnchoredPosition = this.viewportSize - this.contentSize;
            this.refresh();
        }
        public void scrollByItemIndex(int itemIndex)
        {

            var totalLen = this.contentSize;
            var itemLen = totalLen / this.totalItemCount;
            var pos = itemLen * itemIndex;
            this.contentAnchoredPosition = -pos;
        }
        public void refresh()
        {

            var index = 0;
            if (this.contentAnchoredPosition != 0)
            {
                index = (int)(-this.contentAnchoredPosition / this.itemSize);
            }

            foreach (var itemRect in this.containers)
            {

                // set item position
                var pos = this.itemSize * index;
                itemRect.anchoredPosition = (this.direction == Direction.Vertical) ? new Vector2(0, -pos) : new Vector2(pos, 0);

                this.updateItem(index, itemRect.gameObject);

                ++index;
            }

            this.nextInsertItemNo = index - this.containers.Count;
            this.prevAnchoredPosition = (int)(this.contentAnchoredPosition / this.itemSize) * this.itemSize;
        }


        protected override void Awake()
        {

            if (this.itemPrototype == null)
            {
                Debug.LogError(RichText.Red(new { this.name, this.itemPrototype }));
                return;
            }

            base.Awake();

            this.scrollRect = this.GetComponent<ScrollRect>();
            this.viewportRect = this.scrollRect.viewport;
            this.contentRect = this.scrollRect.content;
        }
        protected override void Start()
        {

            this.prevTotalItemCount = this.totalItemCount;

            this.StartCoroutine(this.onSeedData());
        }

        protected virtual IEnumerator onSeedData()
        {

            yield return null;

            // hide prototype

            this.itemPrototype.gameObject.SetActive(false);

            // instantiate items

            var itemCount = (int)(this.viewportSize / this.itemSize) + 4;
            if (GameManager.activePopUps.Last() == GameManager.Popups.GalleryPopUp)
            {
                float totalSpacingBetweenItems = (GalleryPopUp.Inst.bigItemsPerRow - 1) * GalleryPopUp.Inst.spacing;
                float widthofParent = contentRect.rect.width - (GalleryPopUp.Inst.spacing * 2);
                float widthPerItem = (widthofParent - totalSpacingBetweenItems) / GalleryPopUp.Inst.bigItemsPerRow;
                itemPrototype.sizeDelta = new Vector2(widthofParent, widthPerItem + GalleryPopUp.Inst.spacing);
                GalleryPrototypeItem galleryPrototypeItem = itemPrototype.GetComponent<GalleryPrototypeItem>();
                RectTransform firstChild = itemPrototype.GetChild(0).GetComponent<RectTransform>();
                firstChild.sizeDelta = new(widthPerItem, widthPerItem);
                firstChild.anchoredPosition = new(widthPerItem / 2, widthPerItem / 2);
                for (int i = 1; i < GalleryPopUp.Inst.bigItemsPerRow; i++)
                {
                    RectTransform rt = Instantiate(firstChild, itemPrototype);
                    rt.anchoredPosition = new Vector2((widthPerItem * i) + (GalleryPopUp.Inst.spacing * i) + widthPerItem / 2, widthPerItem / 2);
                    galleryPrototypeItem.galleryItems.Add(rt.GetComponent<GalleryItem>());
                }
            }
            else if (GameManager.activePopUps.Last() == GameManager.Popups.ThemesPopUp)
            {
                int itemsPerRow;
                if (ThemesPopUpController.Inst.selectedIndex is 0 or 1 or 4)
                    itemsPerRow = ThemesPopUpController.Inst.bigItemsPerRow;
                else
                    itemsPerRow = ThemesPopUpController.Inst.smallItemsPerRow;

                float totalSpacingBetweenItems = (itemsPerRow - 1) * ThemesPopUpController.Inst.spacing;
                float widthofParent = contentRect.rect.width - (ThemesPopUpController.Inst.spacing * 2 + 2);
                float widthPerItem = (widthofParent - totalSpacingBetweenItems) / itemsPerRow;
                itemPrototype.sizeDelta = new Vector2(widthofParent, widthPerItem + ThemesPopUpController.Inst.spacing);

                RectTransform firstChild = itemPrototype.GetChild(0).GetComponent<RectTransform>();
                firstChild.sizeDelta = new(widthPerItem, widthPerItem);
                firstChild.anchoredPosition = new(widthPerItem / 2, widthPerItem / 2);

                if (ThemesPopUpController.Inst.selectedIndex == 0)
                {
                    PenPrototypeItem penPrototypeItem = itemPrototype.GetComponent<PenPrototypeItem>();
                    for (int i = 1; i < ThemesPopUpController.Inst.bigItemsPerRow; i++)
                    {
                        penPrototypeItem.penItems.Add(MakeChild(i).GetComponent<PenItem>());
                    }
                }
                else if (ThemesPopUpController.Inst.selectedIndex == 1)
                {
                    LensPrototypeItem lensPrototypeItem = itemPrototype.GetComponent<LensPrototypeItem>();
                    for (int i = 1; i < ThemesPopUpController.Inst.bigItemsPerRow; i++)
                    {
                        lensPrototypeItem.lensItems.Add(MakeChild(i).GetComponent<LensItem>());
                    }
                }
                else if (ThemesPopUpController.Inst.selectedIndex == 2)
                {
                    PupilPrototypeItem pupilPrototypeItem = itemPrototype.GetComponent<PupilPrototypeItem>();
                    for (int i = 1; i < ThemesPopUpController.Inst.smallItemsPerRow; i++)
                    {
                        pupilPrototypeItem.pupilItems.Add(MakeChild(i).GetComponent<PupilShopItem>());
                    }
                }
                else if (ThemesPopUpController.Inst.selectedIndex == 3)
                {
                    PatternPrototypeItem patternPrototypeItem = itemPrototype.GetComponent<PatternPrototypeItem>();
                    for (int i = 1; i < ThemesPopUpController.Inst.smallItemsPerRow; i++)
                    {
                        patternPrototypeItem.patternItems.Add(MakeChild(i).GetComponent<PatternShopItem>());
                    }
                }
                else
                {
                    ClickParticlePrototypeItem particlePrototypeItem = itemPrototype.GetComponent<ClickParticlePrototypeItem>();
                    for (int i = 1; i < ThemesPopUpController.Inst.bigItemsPerRow; i++)
                    {
                        particlePrototypeItem.clickParticleItems.Add(MakeChild(i).GetComponent<ClickParticleItem>());
                    }
                }

                RectTransform MakeChild(int i)
                {
                    RectTransform rt = Instantiate(firstChild, itemPrototype);
                    rt.anchoredPosition = new Vector2((widthPerItem * i) + (ThemesPopUpController.Inst.spacing * i) + widthPerItem / 2, widthPerItem / 2);
                    return rt;
                }
            }

            for (var i = 0; i < itemCount; ++i)
            {

                var itemRect = Instantiate(this.itemPrototype);
                itemRect.SetParent(this.contentRect, false);
                itemRect.name = i.ToString();
                itemRect.anchoredPosition = this.direction == Direction.Vertical ? new Vector2(0, -this.itemSize * i) : new Vector2(this.itemSize * i, 0);
                this.containers.AddLast(itemRect);

                itemRect.gameObject.SetActive(true);

                this.updateItem(i, itemRect.gameObject);
            }


            // resize content

            this.resizeContent();

            if (GameManager.activePopUps.Last() == GameManager.Popups.ThemesPopUp)
            {
                if (ThemesPopUpController.Inst.selectedIndex is 0 or 1 or 4)
                {
                    int index;
                    if (ThemesPopUpController.Inst.selectedIndex == 0)
                        index = Mathf.FloorToInt(GeneralDataManager.SelectedPenIndex / (float)ThemesPopUpController.Inst.bigItemsPerRow);
                    else if (ThemesPopUpController.Inst.selectedIndex == 1)
                        index = Mathf.FloorToInt(GeneralDataManager.SelectedLensIndex / (float)ThemesPopUpController.Inst.bigItemsPerRow);
                    else
                        index = Mathf.FloorToInt((GeneralDataManager.SelectedClickParticleIndex + 1) / (float)ThemesPopUpController.Inst.bigItemsPerRow);
                    if (contentRect.rect.height > viewportSize)
                        contentRect.anchoredPosition = new Vector2(0, Mathf.Min(contentRect.rect.height - viewportSize, index * this.itemSize));
                }
            }
            else if (GameManager.activePopUps.Last() == GameManager.Popups.GalleryPopUp)
            {
                if (contentRect.rect.height > viewportSize)
                    contentRect.anchoredPosition = new Vector2(0, contentRect.rect.height - viewportSize);
            }

            toAddMarginForScrollDown = (int)-this.itemSize;
        }


        private void Update()
        {

            if (this.totalItemCount != this.prevTotalItemCount)
            {

                this.prevTotalItemCount = this.totalItemCount;

                // check scroll bottom

                var isBottom = false;
                if (this.viewportSize - this.contentAnchoredPosition >= this.contentSize - this.itemSize * 0.5f)
                {
                    isBottom = true;
                }

                this.resizeContent();

                // move scroll to bottom

                if (isBottom)
                {
                    this.contentAnchoredPosition = this.viewportSize - this.contentSize;
                }

                this.refresh();
            }


            // [ Scroll up ]

            while (this.contentAnchoredPosition - this.prevAnchoredPosition < -(this.itemSize * 2))
            {

                this.prevAnchoredPosition -= this.itemSize;

                // move a first item to last

                var first = this.containers.First;
                if (first == null) break;
                var item = first.Value;
                this.containers.RemoveFirst();
                this.containers.AddLast(item);

                // set item position

                var pos = this.itemSize * (this.containers.Count + this.nextInsertItemNo);
                item.anchoredPosition = (this.direction == Direction.Vertical) ? new Vector2(0, -pos) : new Vector2(pos, 0);

                // update item

                this.updateItem(this.containers.Count + this.nextInsertItemNo, item.gameObject);

                this.nextInsertItemNo++;
            }

            // [ Scroll down ]

            while ((this.contentAnchoredPosition - this.prevAnchoredPosition) - toAddMarginForScrollDown > 0)
            {

                this.prevAnchoredPosition += this.itemSize;

                // move a last item to first

                var last = this.containers.Last;
                if (last == null) break;
                var item = last.Value;
                this.containers.RemoveLast();
                this.containers.AddFirst(item);

                this.nextInsertItemNo--;

                // set item position

                var pos = this.itemSize * this.nextInsertItemNo;
                item.anchoredPosition = (this.direction == Direction.Vertical) ? new Vector2(0, -pos) : new Vector2(pos, 0);

                // update item

                this.updateItem(this.nextInsertItemNo, item.gameObject);
            }
        }

        private void resizeContent()
        {
            var size = this.contentRect.getSize();
            if (this.direction == Direction.Vertical)
            {
                if (GameManager.activePopUps.Last() == GameManager.Popups.GalleryPopUp)
                    size.y = this.itemSize * this.totalItemCount + GalleryPopUp.Inst.spacing;
                else if (GameManager.activePopUps.Last() == GameManager.Popups.ThemesPopUp)
                    size.y = this.itemSize * this.totalItemCount + ThemesPopUpController.Inst.spacing;
                else
                    size.y = this.itemSize * this.totalItemCount;
            }
            else
            {
                if (GameManager.activePopUps.Last() == GameManager.Popups.ChooseImagePopUp)
                    size.x = this.itemSize * this.totalItemCount + ChooseImagePopUpController.Inst.spacing;
                else
                    size.x = this.itemSize * this.totalItemCount + GameManager.Inst.gamePlayUi.itemSpacing;
            }

            this.contentRect.setSize(size);

        }
        private void updateItem(int index, GameObject itemObj)
        {
            if (index < 0 || index >= this.totalItemCount)
            {

                itemObj.SetActive(false);
            }
            else
            {

                itemObj.SetActive(true);

                var item = itemObj.GetComponent<IDynamicScrollViewItem>();
                if (item != null) item.onUpdateItem(index);
            }
        }



        [ContextMenu("Initialize")]
        public virtual void init()
        {

            // [ cliear ]

            this.clear();

            // [ RectTransform ]

            var rectTransform = this.GetComponent<RectTransform>();
            rectTransform.setFullSize();

            // [ ScrollRect ]

            var scrollRect = this.GetComponent<ScrollRect>();
            scrollRect.horizontal = this.direction == Direction.Horizontal;
            scrollRect.vertical = this.direction == Direction.Vertical;
            scrollRect.scrollSensitivity = 15f;

            // [ ScrollRect / Viewport ]

            var viewportRect = new GameObject("Viewport", typeof(RectTransform), typeof(Mask), typeof(Image)).GetComponent<RectTransform>();
            viewportRect.SetParent(scrollRect.transform, false);
            viewportRect.setFullSize();
            viewportRect.offsetMin = new Vector2(10f, 10f);
            viewportRect.offsetMax = new Vector2(-10f, -10f);
            var viewportImage = viewportRect.GetComponent<Image>();
            //viewportImage.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UIMask.psd");
            viewportImage.type = Image.Type.Sliced;
            var viewportMask = viewportRect.GetComponent<Mask>();
            viewportMask.showMaskGraphic = false;
            scrollRect.viewport = viewportRect;

            // [ ScrollRect / Viewport / Content ]

            var contentRect = new GameObject("Content", typeof(RectTransform)).GetComponent<RectTransform>();
            contentRect.SetParent(viewportRect, false);
            if (this.direction == Direction.Horizontal) contentRect.setSizeFromLeft(1.0f); else contentRect.setSizeFromTop(1.0f);
            var contentRectSize = contentRect.getSize();
            contentRect.setSize(contentRectSize - contentRectSize * 0.06f);
            scrollRect.content = contentRect;

            // [ ScrollRect / Viewport / Content / PrototypeItem ]

            this.resetPrototypeItem(contentRect);


            // [ ScrollRect / Scrollbar ]

            var scrollbarName = this.direction == Direction.Horizontal ? "Scrollbar Horizontal" : "Scrollbar Vertical";
            var scrollbarRect = new GameObject(scrollbarName, typeof(Scrollbar), typeof(Image)).GetComponent<RectTransform>();
            scrollbarRect.SetParent(viewportRect, false);
            if (this.direction == Direction.Horizontal) scrollbarRect.setSizeFromBottom(0.05f); else scrollbarRect.setSizeFromRight(0.05f);
            scrollbarRect.SetParent(scrollRect.transform, true);

            var scrollbar = scrollbarRect.GetComponent<Scrollbar>();
            scrollbar.direction = (this.direction == Direction.Horizontal) ? Scrollbar.Direction.LeftToRight : Scrollbar.Direction.BottomToTop;
            if (this.direction == Direction.Horizontal) scrollRect.horizontalScrollbar = scrollbar; else scrollRect.verticalScrollbar = scrollbar;

            // [ ScrollRect / Scrollbar / Image ]

            var scrollbarImage = scrollbarRect.GetComponent<Image>();
#if UNITY_EDITOR
            scrollbarImage.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
#endif
            scrollbarImage.color = new Color(0.1f, 0.1f, 0.1f, 0.5f);
            scrollbarImage.type = Image.Type.Sliced;

            // [ ScrollRect / Scrollbar / slidingArea ]

            var slidingAreaRect = new GameObject("Sliding Area", typeof(RectTransform)).GetComponent<RectTransform>();
            slidingAreaRect.SetParent(scrollbarRect, false);
            slidingAreaRect.setFullSize();

            // [ ScrollRect / Scrollbar / slidingArea / Handle ]

            var scrollbarHandleRect = new GameObject("Handle", typeof(Image)).GetComponent<RectTransform>();
            scrollbarHandleRect.SetParent(slidingAreaRect, false);
            scrollbarHandleRect.setFullSize();
            var scrollbarHandleImage = scrollbarHandleRect.GetComponent<Image>();
#if UNITY_EDITOR
            scrollbarHandleImage.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
#endif
            scrollbarHandleImage.color = new Color(0.5f, 0.5f, 1.0f, 0.5f);
            scrollbarHandleImage.type = Image.Type.Sliced;
            scrollbar.handleRect = scrollbarHandleRect;

            // [ ScrollRect / ScrollbarHandleSize ]

            var scrollbarHandleSize = scrollRect.GetComponent<ScrollbarHandleSize>();
            if (scrollbarHandleSize == null)
            {
                scrollbarHandleSize = scrollRect.gameObject.AddComponent<ScrollbarHandleSize>();
                scrollbarHandleSize.maxSize = 1.0f;
                scrollbarHandleSize.minSize = 0.1f;
            }

            // [ Layer ]

            this.gameObject.setLayer(this.transform.parent.gameObject.layer, true);
        }
        protected virtual void resetPrototypeItem(RectTransform contentRect)
        {

            // [ ScrollRect / Viewport / Content / PrototypeItem ]

            var prototypeItemRect = new GameObject("Prototype Item", typeof(RectTransform), typeof(Image), typeof(DynamicScrollViewItemExample)).GetComponent<RectTransform>();
            prototypeItemRect.SetParent(contentRect, false);
            if (this.direction == Direction.Horizontal) prototypeItemRect.setSizeFromLeft(0.23f); else prototypeItemRect.setSizeFromTop(0.23f);
            var prototypeItem = prototypeItemRect.GetComponent<DynamicScrollViewItemExample>();
            var prototypeItemBg = prototypeItemRect.GetComponent<Image>();
#if UNITY_EDITOR
            prototypeItemBg.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
#endif
            prototypeItemBg.type = Image.Type.Sliced;
            prototypeItem.background = prototypeItemBg;
            this.itemPrototype = prototypeItemRect;

            // [ ScrollRect / Viewport / Content / PrototypeItem / Title ]

            var prototypeTitleRect = new GameObject("Title", typeof(RectTransform), typeof(Text)).GetComponent<RectTransform>();
            prototypeTitleRect.SetParent(prototypeItemRect, false);
            prototypeTitleRect.setFullSize();
            var prototypeTitleSize = prototypeTitleRect.getSize();
            prototypeTitleRect.setSize(prototypeTitleSize - prototypeTitleSize * 0.1f);
            var title = prototypeTitleRect.GetComponent<Text>();
            title.fontSize = 16;
            title.alignment = TextAnchor.MiddleCenter;
            title.horizontalOverflow = HorizontalWrapMode.Wrap;
            title.verticalOverflow = VerticalWrapMode.Truncate;
            title.color = Color.black;
            title.text = "Name000";
            title.resizeTextForBestFit = true;
            title.resizeTextMinSize = 9;
            title.resizeTextMaxSize = 40;
            prototypeItem.title = title;
        }
        protected virtual void clear()
        {

            while (this.transform.childCount > 0)
            {
                DestroyImmediate(this.transform.GetChild(0).gameObject);
            }
        }


        protected abstract float contentAnchoredPosition { get; set; }
        protected abstract float contentSize { get; }
        protected abstract float viewportSize { get; }
        protected abstract float itemSize { get; }


        protected Direction direction = Direction.Vertical;
        protected LinkedList<RectTransform> containers = new LinkedList<RectTransform>();
        protected float prevAnchoredPosition = 0;
        protected int nextInsertItemNo = 0; // item index from left-top of viewport at next insert
        protected int prevTotalItemCount = 99;
        protected ScrollRect scrollRect = null;
        protected RectTransform viewportRect = null;
        protected RectTransform contentRect = null;




        /// <summary> Scroll Direction </summary>
	    public enum Direction
        {
            Vertical,
            Horizontal,
        }
    }
}
