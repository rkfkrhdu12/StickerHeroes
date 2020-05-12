﻿using System.Collections.Generic;
using GameplayIngredients;
using UnityEngine;

using GameItem;

public class ItemInventorySystem : MonoBehaviour
{
    #region Variable
    public GameObject _contentObject;

    public GameObject _itemPrefab = null;

    private UnitPhoto _unitPhoto;

    [SerializeField]
    private TeamManager _teamManager = null;

    [SerializeField]
    private GameObject _unitModelUI = null;

    public eCodeType Type
    {
        get => _curType;
        set
        {
            if (_curType == value) return;

            _curType = value;
            UpdateInventory();
        }
    }

    /// <summary>
    /// 왼쪽은 true[3] 오른쪽은 false[4]
    /// </summary>
    private bool _isLeftWeapon = true;

    #region Hide Inspector

    private eCodeType _curType = eCodeType.HELMET;
    private readonly List<int>[] _inventory = new List<int>[3];

    private ItemList _itemList;

    private int[] _equipedItems = new int[4];

    #endregion

    #endregion

    #region Monobehaviour Function

    private void Awake()
    {
        _equipedItems[0] = 1;
        _equipedItems[1] = 2;

        _unitPhoto = _teamManager.GetUnitPhoto();
        if (null == _unitPhoto) { Debug.LogError("ItemInventorySystem : _unitPhoto is null"); }

        _itemList = Manager.Get<GameManager>().itemList;

        _inventory[(int)eCodeType.HELMET] = new List<int>();

        _inventory[(int)eCodeType.BODYARMOUR] = new List<int>();

        _inventory[(int)eCodeType.WEAPON] = new List<int>();
    }

    private void Start()
    {
        #region Test

        const int _completeModelCount = 6;
        string[] itemNameList = new string[_completeModelCount * 2];

        int itemIndex = 0;
        itemNameList[itemIndex++] = "일반 머리";
        itemNameList[itemIndex++] = "일반 옷";
        itemNameList[itemIndex++] = "견습 기사의 투구";
        itemNameList[itemIndex++] = "견습 기사의 갑옷";
        itemNameList[itemIndex++] = "셔우드 숲의 모자";
        itemNameList[itemIndex++] = "셔우드 숲의 코트";
        itemNameList[itemIndex++] = "하얀 눈의 모자";
        itemNameList[itemIndex++] = "하얀 눈의 옷";
        itemNameList[itemIndex++] = "A.I의 머리 파츠";
        itemNameList[itemIndex++] = "A.I의 몸통 파츠";
        itemNameList[itemIndex++] = "제국의 헬멧";
        itemNameList[itemIndex++] = "제국의 슈트";

        eCodeType[] codes = new eCodeType[2];
        codes[0] = eCodeType.HELMET;
        codes[1] = eCodeType.BODYARMOUR;

        for (int i = 0; i < itemNameList.Length; ++i)
        {
            int num = _itemList.CodeSearch(codes[i % 2],itemNameList[i]);

            _inventory[(int)codes[i % 2]].Add(num);
        }

        //TypeItemAllSet(eCodeType.HELMET);
        //TypeItemAllSet(eCodeType.BODYARMOUR);
        //TypeItemAllSet(eCodeType.WEAPON);

        #endregion

        Type = eCodeType.HELMET;

        UpdateInventory();
    }

    //매번 UI가 활성화 될 때, 호출됩니다.
    private void OnEnable()
    {
        Type = eCodeType.HELMET;

        UpdateInventory();
    } 
    #endregion

    // Test
    private void TypeItemAllSet(eCodeType codeType)
    {
        //아이템 개수 반환.
        int count = _itemList.ItemCount(codeType);

        //아이템 개수만큼 반복,
        for (int i = 0; i < count; ++i)
        {
            int num = _itemList.CodeSearch(codeType, i);

            _inventory[(int)codeType].Add(num);
        }
    }

    /// <summary>
    /// 타입을 헤멧을 만듭니다.
    /// </summary>
    public void TypeHelmet() => Type = eCodeType.HELMET;

    /// <summary>
    /// 타입을 아머로 만듭니다.
    /// </summary>
    public void TypeBodyArmour() => Type = eCodeType.BODYARMOUR;

    public void TypeRightWeapon() { Type = eCodeType.WEAPON; _isLeftWeapon = false; }
    public void TypeLeftWeapon()  { Type = eCodeType.WEAPON; _isLeftWeapon = true; }

    public void SetEquipedItems(int[] items)
    {
        UnitModelManager.ResetModel(_unitModelUI, _equipedItems);

        // 유닛 아이템들이 유닛1 수정완료(5,6)   유닛2 수정완료(5,6)

        _equipedItems = new int[4];
        _equipedItems = items;

        UpdateInventory();
    }

    public void OnSave()
    {
        _unitPhoto.SaveTexture(_equipedItems);
        _teamManager.SetSelectUnitEquipedItems(_equipedItems);
    }

    public void OnEquiped(ItemSlot curSlot)
    {
        int itemCode = curSlot.ItemNumber;

        Item i = _itemList.ItemSearch(itemCode);
        if (i == null) { Debug.Log("ItemInventorySystem : UpdateEquipedItem i is Error"); return; }

        // 장착한 아이템 업데이트
        {

        }

        // 모델링 업데이트
        {
            int partsNum = 0;
            switch (i.Type)
            {
                case eItemType.NONE: Debug.Log("ItemInventorySystem : UpdateEquipedItem i.Type is Error"); return;
                case eItemType.HELMET: partsNum = 0; break;
                case eItemType.BODYARMOUR: partsNum = 1; break;
                default: partsNum = (_isLeftWeapon ? 3 : 4); break;
            }

            if (partsNum >= _equipedItems.Length) { Debug.Log("ItemInventorySystem : UpdateEquipedItem partsNum is Error"); return; }

            int prevItem = _equipedItems[partsNum];
            _equipedItems[partsNum] = itemCode;

            UpdateModel(prevItem);
        }
    }

    #region Private Function

    private void UpdateInventory()
    {
        // Update Slot
        {
            //현재 타입이 무엇인지 가져옵니다.
            int curType = (int)_curType;

            for (int i = 0; i < _inventory[curType].Count; ++i)
                if (_itemList.ItemSearch(_inventory[curType][i]) == null) _inventory[curType].Remove(i);

            //현재 사용중인 아이템 슬롯의 개수를 가져옵니다.
            int curUseItemSlotCount = _inventory[curType].Count;

            //아이템이 없거나, 오브젝트 컨텐츠가 없을시 : return
            if (_itemPrefab == null || _contentObject == null)
                return;

            //이전 아이템 슬롯의 개수를 가져옵니다.
            int prevUseItemSlotCount = _contentObject.transform.childCount;

            //비어있는 슬롯 개수 = 이전 개수 - 현재 개수
            int emptySlotCount = prevUseItemSlotCount - curUseItemSlotCount;

            //비어있는 슬롯 개수가 있다면, 비어있는 슬롯 개수만큼 제거
            if (emptySlotCount > 0)
                for (int j = 0; j < emptySlotCount; ++j)
                    DeleteObjectSystem.AddDeleteObject(_contentObject.transform.GetChild(0).gameObject);

            //반대로 슬롯 개수가 0 미만이라면, 아이템 프리팹을 추가합니다.
            else if (emptySlotCount < 0)
                for (int j = 0; j > emptySlotCount; --j)
                    Instantiate(_itemPrefab, Vector3.zero, Quaternion.identity, _contentObject.transform).SetActive(true);

            //현재 슬롯 개수만큼 반복하여, 아이템 넘버를 변경한다.
            for (int i = 0; i < curUseItemSlotCount; ++i)
                _contentObject.transform.GetChild(i).GetComponent<ItemSlot>().ItemNumber = _inventory[curType][i];
        }

        UpdateModel();

        // Update Select
        { // 유닛이 끼고잇는 아이템을 표시

        }
    }

    void UpdateModel(in int prevItem = 0)
    {
        if (_equipedItems == null && _unitModelUI == null) { return; }
        if (_equipedItems.Length == 0) { return; }

        UnitModelManager.UpdateModel(_unitModelUI, _equipedItems, prevItem);
    }

    #endregion
}
