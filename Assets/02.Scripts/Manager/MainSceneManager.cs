﻿using System.Collections;
using System.Collections.Generic;
using GameplayIngredients;
using UnityEngine;
using UnityEngine.UI;

namespace MainSceneUI
{
    #region UIDataStructure
    public struct UIDataKey
    {
        public const string Lobby = "Lobby";
        public const string SetUnit = "SetUnit";
        public const string Matching = "Match";
        public const string ChoiceUnitTeam = "ChoiceUnitTeam";
    }

    public class UIScreen
    {
        public GameObject myUI;

        //해당 챕터가 꺼져있는지 켜져있는지 체크합니다.
        public bool activeSelf;

        /// <summary>
        /// 해당 UI챕터를 켜거나 끕니다.
        /// </summary>
        /// <param name="active"></param>
        public void setActive(bool active)
        {
            activeSelf = active;

            myUI?.SetActive(active); 
        }
    } 
    #endregion

    public class MainSceneManager : MonoBehaviour
    {
        #region Private Variable

        #region Show Inspector

        [Header("각각의 UI들")]

        [SerializeField]
        private GameObject _tutorialUI = null;

        #region Team UIs
        [SerializeField]
        private GameObject _choiceUnitTeamUI = null;

        [SerializeField, Tooltip("유닛을 세팅할 UI 오브젝트")]
        private GameObject _setUnitUI = null;

        [SerializeField, Tooltip("매칭을 선택할 UI")]
        private GameObject _MatchUI = null; 
        #endregion

        [Space, SerializeField]
        private TeamManager _teamManager = null;

        //[SerializeField]
        //private GameObject _canvas;

        #endregion

        private Team _selectTeam;

        // UIScreenObject 의 이름, List에서의 번호
        private Dictionary<string, UIScreen> nameToSceenUI = new Dictionary<string, UIScreen>();

        private string _curKey = string.Empty;
        #endregion

        #region Monobehaviour Function
        private void Awake()
        {
            //UI키 : UI 오브젝트 형태로 링크해줍니다.
            InitUI(UIDataKey.Lobby, _tutorialUI);
            InitUI(UIDataKey.SetUnit, _setUnitUI);
            InitUI(UIDataKey.Matching, _MatchUI);
            InitUI(UIDataKey.ChoiceUnitTeam, _choiceUnitTeamUI);

            UpdateScreen(UIDataKey.Lobby);
        }

        private void OnEnable()
        {
            StartCoroutine(StartLoading());
        }

        #endregion

        [SerializeField] GameObject _loadingScreenObject = null;
        [SerializeField] Image _loadingBarImage = null;

        WaitForSeconds _loadingScreenTime = new WaitForSeconds(.5f);
        WaitForSeconds _loadingScreenTime1 = new WaitForSeconds(.2f);
        WaitForSeconds _loadingScreenTime2 = new WaitForSeconds(.2f);
        WaitForSeconds _loadingScreenTime3 = new WaitForSeconds(.1f);
        IEnumerator StartLoading()
        {
            if (_loadingScreenObject == null) yield break;

            int i = 0;

            if (!_loadingScreenObject.gameObject.activeSelf)
                _loadingScreenObject.gameObject.SetActive(true);

            _loadingBarImage.fillAmount = 0.0f;

            yield return _loadingScreenTime;
            _loadingBarImage.fillAmount += .4f;

            yield return _loadingScreenTime1;
            _loadingBarImage.fillAmount += .3f;

            yield return _loadingScreenTime2;
            _loadingBarImage.fillAmount += .3f;

            yield return _loadingScreenTime3;

            if (_loadingScreenObject.gameObject.activeSelf)
                _loadingScreenObject.gameObject.SetActive(false);
        }


        #region Private Function
        private void InitUI(string key, GameObject ui = null)
        {
            //객체 생성
            var uiScreen = new UIScreen();

            //인자로 넘어온 UI가 null이 아닐 경우, UI스크린 리스트에 수록
            if (ui != null) uiScreen.myUI = ui;

            //딕셔너리에 해당 키와 UI스크린의 리스트를 생성하고, UI 스크린 객체를 수록한다.
            nameToSceenUI[key] = uiScreen;

            //방금 Add한 오브젝트를 비활성화 시킴
            nameToSceenUI[key].setActive(false);
        }

        #endregion

        public void OnExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
        }

        public void UpdateScreen(string key)
        {
            //인자로 넘어온 키 값이 카운트에 없을 경우 : return
            if (!nameToSceenUI.ContainsKey(key))
                return;

            //현재 활성화된 UI는 비활성화 한다.
            if (!_curKey.Equals(string.Empty))
                nameToSceenUI[_curKey].setActive(false);

            // //새로 들어온 Key의 UI를 활성화 한다.
            nameToSceenUI[key].setActive(true);
            _curKey = key;
        }

        public void UpdateCheckScreen(string key)
        {
            //인자로 넘어온 키 값이 카운트에 없을 경우 : return
            if (!nameToSceenUI.ContainsKey(key))
                return;

            //해당 UI스크린을 가져온다.
            UIScreen ui = nameToSceenUI[key];

            //활성화 <-> 비활성화
            ui.setActive(!ui.activeSelf);
        }

        public void OnGameStart(string scene)
        {
            _selectTeam = _teamManager.GetSelectTeam();

            if(_selectTeam == null) { LogMessage.Log("설정된 팀 또는 팀의 유닛들이 불안정합니다."); return; }

            Manager.Get<GameManager>().SetPlayerUnits(_selectTeam);

            NextGoto(scene);
        }

        public void NextGoto(string scene) =>
            Manager.Get<SceneManagerPro>().LoadScene(scene);
    }
}
