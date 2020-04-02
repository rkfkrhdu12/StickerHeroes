using System;
using GameplayIngredients;
using UnityEngine;

[ManagerDefaultPrefab("GameManager")]
public class GameManager : Manager
{
    #region Show Inspector
   
    [Tooltip("아군인지 적군인지 선택")]
    public eTeam _WinTeam = eTeam.PLAYER;
    
    #endregion

    #region Hide Inspector

    //아이템에 대한 리스트 변수
    public ItemList itemList { get; private set; }

    //팀에 대한 변수
    private Team _playerTeam;
    
    //삭제에 대한 매니저 변수
    private DeleteObjectManager _deleteObjectManager;
    
    #endregion
    
    public int getPlayerUnitCount() => //플레이어의 유닛 개수를 리턴합니다.
        _playerTeam._units.Length;

    //index번째 유닛에 대한 데이터를 리턴합니다.
    public UnitData getPlayerUnit(int index)
    {
        //index 값이 0미만이거나, 배열의 길이를 초과한다면 : null을 반환
        if (index < 0 || index >= _playerTeam._units.Length)
            return null;

        //해당 index번째 유닛을 리턴합니다.
        return _playerTeam._units[index];
    }
    
    private void Awake() =>
        OnAwake();
    
    private void OnAwake()
    {
        //itemList : 초기화
        itemList = new ItemList();
        itemList.Init();
        
        //PlayerTeam : 초기화
        _playerTeam = new Team();
        _playerTeam.InitTest();

        UnitStability.Init();
        _deleteObjectManager = new DeleteObjectManager();
        DontDestroyOnLoad(gameObject);
    }
    
    private void LateUpdate() =>
        objectToDelete();
    
    private void objectToDelete()
    {
        //삭제해야하는 오브젝트 목록이 0이라면, 아래 코드 구문 실행 X
        if (DeleteObjectManager.getCount() <= 0) return;

        //오브젝트를 제거하고, 제거된 오브젝트를 return합니다.
        var deleteObj = DeleteObjectManager.getDequeue();

        //삭제
        Destroy(deleteObj);
    }
}
