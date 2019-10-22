using System;
using System.Collections.Generic;
using UnityEngine;
using EFT;
using System.IO;

namespace UnhandledExceptionHandler.Functions
{

    /*class DUMP {
        public static void DUMPER() {

            File.AppendAllText(@"c:\dump2.txt", "Start\n");
            String temp;
            for (int i = 1; i < 1000000; i++)
            {
                temp = Class1864.smethod_0(i);
                if(temp != "" && temp != null)
                    File.AppendAllText(@"c:\dump2.txt", "|" + i.ToString() + "| -> '" + temp + "'\n");
            }

        }

    }*/
    class EFunc
    {
        #region COLORS
        private static Color C_bots = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        private static Color C_player = new Color(1.0f, 0f, 0f, 1.0f);
        private static Color C_group = new Color(0f, 0.8f, 1.0f, 1.0f);
        private static Color C_botsRip = new Color(0.8f, 0.8f, 0.8f, 1.0f);
        private static Color C_playerRip = new Color(0.8f, 0f, 0f, 1.0f);
        private static Color C_groupRip = new Color(0f, 0.6f, 0.8f, 1.0f);
        private static Color C_Chair = new Color(1.0f, 1.0f, 0f, 1.0f);
        private static Color C_b0n3 = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        #endregion
        #region LocalVariables
        private static Color OldGuiColor;
        #endregion
        public static void DrawError(IEnumerable<Player> _ply, Player _lP, float _viewdistance)
        {
            if (_ply != null && _lP != null)
            {
                float deltaDistance = 25f;
                string playerDisplayName = "";
                float devLabel = 1f;
                string Status;
                
                var LabelSize = new GUIStyle { fontSize = 12 };
                Color playerColor;
                var enumerator = _ply.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    try
                    {
                        var p = enumerator.Current;
                        if (p != null)
                        {
                            if (EPointOfView.FirstPerson != p.PointOfView)
                            {
                                float dTO = FMath.FD(Camera.main.transform.position, p.Transform.position);
                                if (dTO > 1f && dTO <= _viewdistance && Camera.main.WorldToScreenPoint(p.Transform.position).z > 0.01f)
                                {
                                    Vector3 pHeadVector = Camera.main.WorldToScreenPoint(p.PlayerBones.Head.position);
                                    float find_sizebox = Math.Abs(pHeadVector.y - Camera.main.WorldToScreenPoint(p.PlayerBones.Neck.position).y) * 1.5f; // size of the head - its not good but its scaling without much maths
                                    find_sizebox = (find_sizebox > 30f) ? 30f : find_sizebox;
                                    float half_sizebox = (find_sizebox > 30f) ? 15f : find_sizebox / 2f;
                                    int FontSize = 12;
                                    FMath.DistSizer(dTO, ref FontSize, ref deltaDistance, ref devLabel);
                                    LabelSize.fontSize = FontSize;

                                    if (p.HealthController.IsAlive)
                                    {
                                        Status = p.HealthStatus.ToString(); // Health here 
                                        OldGuiColor = GUI.color;
                                        #region detect what we have AI / Teamamate / Scav or Player
                                        if (p.Profile.Info.RegistrationDate <= 0)
                                        {
                                            playerDisplayName = "AI";
                                            playerColor = C_bots;
                                            GUI.color = Color.red;
                                            EDS.P(new Vector2(pHeadVector.x - half_sizebox, (float)(Screen.height - pHeadVector.y) - half_sizebox), Color.red, find_sizebox);
                                        }
                                        else if (_lP.Profile.Info.GroupId == p.Profile.Info.GroupId && _lP.Profile.Info.GroupId != "0" && _lP.Profile.Info.GroupId != "" && _lP.Profile.Info.GroupId != null)
                                        {
                                            playerDisplayName = p.Profile.Info.Nickname;
                                            playerColor = C_group;
                                            GUI.color = Color.cyan;
                                            EDS.P(new Vector2(pHeadVector.x - half_sizebox, (float)(Screen.height - pHeadVector.y) - half_sizebox), Color.cyan, find_sizebox);
                                        }
                                        else
                                        {
                                            playerDisplayName = (p.Profile.Info.Side == EPlayerSide.Savage) ? "Scav" : p.Profile.Info.Nickname;
                                            playerColor = C_player;
                                            GUI.color = Color.red;
                                            EDS.P(new Vector2(pHeadVector.x - half_sizebox, (float)(Screen.height - pHeadVector.y) - half_sizebox), Color.red, find_sizebox);
                                        }
                                        #endregion
                                        GUI.color = OldGuiColor;
                                    }
                                    else
                                    {
                                        Status = "";
                                        #region detect what corpse we have AI / Teamamate / Scav or Player
                                        if (p.Profile.Info.RegistrationDate <= 0)
                                        {
                                            playerDisplayName = "AI";
                                            playerColor = C_botsRip;
                                        }
                                        else if (_lP.Profile.Info.GroupId == p.Profile.Info.GroupId && _lP.Profile.Info.GroupId != "0" && _lP.Profile.Info.GroupId != "" && _lP.Profile.Info.GroupId != null)
                                        {
                                            playerDisplayName = p.Profile.Info.Nickname;
                                            playerColor = C_groupRip;
                                        }
                                        else
                                        {
                                            playerDisplayName = (p.Profile.Info.Side == EPlayerSide.Savage) ? "Scav" : p.Profile.Info.Nickname;
                                            playerColor = C_playerRip;
                                        }
                                        #endregion
                                    }
                                    #region scaling things and drawing text above heads
                                    string playerText = $"{playerDisplayName} [{(int)dTO}m] {Status}";
                                    var pTextVector = GUI.skin.GetStyle(playerText).CalcSize(new GUIContent(playerText));
                                    GUI.color = playerColor;
                                    float texWidth = (devLabel == 1f) ? pTextVector.x : (pTextVector.x / devLabel);
                                    LabelSize.normal.textColor = playerColor;
                                    GUI.Label(new Rect(pHeadVector.x - pTextVector.x / 2f, (float)Screen.height - Camera.main.WorldToScreenPoint(p.PlayerBones.Head.position).y - deltaDistance, texWidth, pTextVector.y), playerText, LabelSize);
                                    #endregion
                                }
                            }
                        }
                    }
                    catch (NullReferenceException ex)
                    {
                    }
                }
            }
        }
    }
}
