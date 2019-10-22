using System;
using System.Collections.Generic;
//using System.Runtime.InteropServices;
using UnityEngine;
using EFT;
using System.IO;
using UnityEngine.SceneManagement;
using UnhandledExceptionHandler.Functions;

using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using EFT.AssetsManager;
using EFT.Ballistics;
using EFT.InventoryLogic;
using EFT.Quests;
using JetBrains.Annotations;
using Systems.Effects;

namespace UnhandledExceptionHandler
{
    class ErrorLog
    {
        private static void Start()
        {
            new UnhandledException().Load();
        }
    }
    public class UnhandledException : MonoBehaviour
    {
        public UnhandledException() { }
        #region VARS Basic Assigns
        private GameObject GameObjectHolder;
        private Scene m_Scen;
        private string m_Scen_name;
        private IEnumerable<Player> _ply;
        private Player _lP;
        private float _plyR = 0f, _scanSceneR = 0f, _lpR = 0f;// _secTime = 0f;
        #endregion
        #region VARS Switches
        private bool _pInfor, _rec = false, _cH, AllowEFunc = true, _aim = false, _aimA = false;
        #endregion
        #region VARS Variables - Changable
        protected float __plyUpdate = 5f, __lPUpdate = 5f;
        private float _viewdistance = 1000f;
        private int _AimSpeed = 1;
        private Vector3 saveVector = new Vector3(0f,0f,0f);
        #endregion

        #region Awake
        private void Awake()
        {
            Debug.logger.logEnabled = true;
        }
        #endregion
        #region Load
        public void Load()
        {
            GameObjectHolder = new GameObject();
            GameObjectHolder.AddComponent<UnhandledException>();
            DontDestroyOnLoad(GameObjectHolder);
        }
        #endregion
        #region Clear
        private void Clear()
        {
            _ply = null;
            _lP = null;
            _plyR = 0;
            _scanSceneR = 0;
            _lpR = 0;
        }
        #endregion
        #region Unload
        public void Unload()
        {
            Clear();
            GC.Collect();
            Destroy(GameObjectHolder);
            Resources.UnloadUnusedAssets();
            //if (File.Exists(@"D:\iksu.txt"))
            //{
            //    File.Delete(@"D:\iksu.txt");
            //}
            Destroy(this);
        }
        #endregion

        #region Update
       // private GInterface163 test;
        private void Update()
        {
           #region Capture scene name for checks
            if (Time.time >= _scanSceneR)
            {
                _scanSceneR = Time.time + 5f;
                this.m_Scen = SceneManager.GetActiveScene();
                this.m_Scen_name = m_Scen.name;
            }
            #endregion
            #region Hotkeys
            //if (Input.GetKeyDown(KeyCode.End))
            //{ Unload(); } // never unload

            if (Input.GetKeyDown(KeyCode.F8))
            {
                if (saveVector.y == 0f)
                {
                    saveVector = MovementState.G;
                }
                
                if(MovementState.G_y == 0.00001f)
                {
                    MovementState.G = saveVector;
                    MovementState.G_y = saveVector.y;
                }
                else
                {
                    MovementState.G = new Vector3(0.00001f, 0.00001f, 0.00001f);
                    MovementState.G_y = 0.00001f;
                }
                
               // test.imethod_saveProgress();
               //    DUMP.DUMPER();
            }

            if (Input.GetKeyDown(KeyCode.F11))
            { Debug.logger.logEnabled = !Debug.logger.logEnabled; }
            if (Input.GetKeyDown(KeyCode.Home))
            { _pInfor = !_pInfor; }
            if (Input.GetKeyDown(KeyCode.Insert))
            { _cH = !_cH; }
            #region Porting
            if (Input.GetKeyDown(KeyCode.F10) && _lP != null)
            {
                //if (Time.time >= _secTime) {
                   // _secTime = Time.time + 1f;
                    _lP.Transform.position = _lP.Transform.position + Camera.main.transform.forward * 1f;
                //}
            }
            if (Input.GetKeyDown(KeyCode.Keypad2) && _lP != null)
            { _lP.Transform.position = new Vector3(_lP.Transform.position.x, _lP.Transform.position.y - 1f, _lP.Transform.position.z); }
            if (Input.GetKeyDown(KeyCode.Keypad8) && _lP != null)
            { _lP.Transform.position = new Vector3(_lP.Transform.position.x, _lP.Transform.position.y + 1f, _lP.Transform.position.z); }
            #endregion
            if (Input.GetKeyDown(KeyCode.F9))
            { _rec = !_rec; }
            #endregion
            #region mouse events capture for AFunc
            if (Input.GetMouseButtonDown(1))
                _aim = true;
            if (Input.GetMouseButtonUp(1))
                _aim = false;
            if (Input.GetKeyDown(KeyCode.F5))
                _aimA = !_aimA;
            #endregion
            #region AFunc activator with preventions
            if (_aim && _aimA && m_Scen_name != "EnvironmentUIScene" && m_Scen_name != "MenuUIScene")
            { AFunc.DPI_Detection(_ply, _lP, _AimSpeed, 200f, 50); }
            #endregion
            
        }
#endregion

        #region OnGui
        private void OnGUI()
        {
            #region On GUI Loaded `Home activated`
            if (_pInfor)
            {
                if (m_Scen != null)
                {
                    GUI.Label(new Rect(1f, 1f, 100f, 20f), ".");
                    if (m_Scen.isLoaded)
                    {
                        if (m_Scen_name != "EnvironmentUIScene" && m_Scen_name != "MenuUIScene")
                        {
                            if (AllowEFunc == false)
                                AllowEFunc = true;
                            GUI.color = Color.red;
                            EDS.P(new Vector2(1f, 1f), Color.red, 1f);
                                // Update Main Struct
                            if (AllowEFunc == true && Time.time >= _plyR) 
                            {
                                if (_ply != FindObjectsOfType<Player>())
                                {
                                    _ply = null;
                                    _ply = FindObjectsOfType<Player>();
                                }
                                _plyR = Time.time + __plyUpdate;
                            }
                                // Update GLPB
                            if (AllowEFunc == true && _ply != null && Time.time >= _lpR)
                            {
                                LPBFunc.GLPB(_ply, ref _lP);
                                _lpR = Time.time + __lPUpdate;
                            }
                                // recoil controller draw blue if ON
                            if (_rec && _lP != null)
                            {
                                GUI.color = Color.blue;
                                EDS.P(new Vector2(2f, 1f), Color.blue, 1f);
                                _lP.ProceduralWeaponAnimation.Shootingg.Intensity = 0.5f;
                            }
                            else
                                _lP.ProceduralWeaponAnimation.Shootingg.Intensity = 1.0f;
                                // draw yellow dotter if in ads
                            if (_cH && _lP != null)
                            {
                                if (!_lP.ProceduralWeaponAnimation.IsAiming)
                                {
                                    GUI.color = Color.yellow;
                                    EDS.P(new Vector2(Screen.width / 2f - 1f, Screen.height / 2f - 1f), Color.yellow, 2f);
                                }
                            }
                            EFunc.DrawError(_ply, _lP, _viewdistance);
                        }
                        else
                        {
                            if (AllowEFunc == true)
                            {
                                _pInfor = false;
                                _lP.ProceduralWeaponAnimation.Shootingg.Intensity = 1.0f;
                                Clear();
                                AllowEFunc = false;
                            }
                        }
                    }
                }
            }
            else
            {
                GUI.color = Color.white;
                EDS.P(new Vector2(1f, 1f), Color.white, 1f);
            }
            #endregion
            #region Log is off? FLAG
            if(Debug.logger.logEnabled == false)
                GUI.Label(new Rect(1f, 5f, 100f, 20f), "off");
            #endregion
        }
        #endregion
    }
}