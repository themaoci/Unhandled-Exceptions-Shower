using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using EFT;
using System.IO;
using UnhandledExceptionHandler.Functions;

namespace UnhandledExceptionHandler.Functions
{
    class AFunc
    {
        #region DLLImporting + mouse events
        //[DllImport("user32.dll")]
        //static extern bool GetCursorPos(out POINT lpPoint);
        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        private const int MOUSEEVENTF_MOVE = 0x0001;
        private static void Move(int xDelta, int yDelta)
        { mouse_event(MOUSEEVENTF_MOVE, xDelta, yDelta, 0, 0); }
        //private struct POINT { public int X, Y; }
        #endregion
        public static void DPI_Detection(IEnumerable<Player> _ply, Player _lP, int _AimSpeed, float _checkDistance = 200f, double _distance2d = 100)
        {
            if (_ply != null && _lP != null)
            {
                float aimPosX = 0;
                float aimPosY = 0;
                Vector3 velocity = new Vector3(0, 0, 0);
                var enumerator = _ply.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    try
                    {
                        var p = enumerator.Current;
                        if (p != null)
                        {
                            if (!p.IsVisible) continue;
                            float distanceToObject = FMath.FD(Camera.main.transform.position, p.Transform.position);
                            if (distanceToObject < _checkDistance)
                            {
                                if (p.HealthController.IsAlive && p.IsVisible && EPointOfView.FirstPerson != p.PointOfView)
                                {

                                    Vector3 playerHeadVector = Camera.main.WorldToScreenPoint(p.PlayerBones.Head.position);
                                    double screenDist = FMath.FDv2(new Vector2(Screen.width, Screen.height) / 2, new Vector2(playerHeadVector.x, playerHeadVector.y));
                                    if (screenDist < _distance2d)
                                    {
                                        velocity = p.Velocity;
                                        aimPosX = playerHeadVector.x;
                                        aimPosY = playerHeadVector.y;
                                    }
                                    else
                                        continue;
                                }
                                else
                                    continue;
                            }
                            else
                                continue;
                        }
                        else
                            continue;

                    }
                    catch (NullReferenceException ex)
                    {
                    }
                    AimAtPos(aimPosX, aimPosY, velocity, _AimSpeed);
                }
            }
            
        }
        private static void AimAtPos(float x, float y, Vector3 velocity, int _AimSpeed)
        {
            int ScreenCenterX = (Screen.width / 2);
            int ScreenCenterY = (Screen.height / 2);
            float TargetX = 0;
            float TargetY = 0;
            if (Math.Abs(ScreenCenterX - x) < 50 || Math.Abs(ScreenCenterY - y) < 50)
                _AimSpeed = 2;
            if (x != 0 && x != ScreenCenterX)
            {
                if (x > ScreenCenterX)
                {
                    TargetX = -(ScreenCenterX - x);
                    if (TargetX + ScreenCenterX > ScreenCenterX * 2) TargetX = 0;
                }

                if (x < ScreenCenterX)
                {
                    TargetX = x - ScreenCenterX;
                    if (TargetX + ScreenCenterX < 0) TargetX = 0;
                }
            }
            if (y != 0 && y != ScreenCenterY)
            {

                if (y > ScreenCenterY)
                {
                    TargetY = ScreenCenterY - y;
                    if (TargetY + ScreenCenterY > ScreenCenterY * 2) TargetY = 0;
                }

                if (y < ScreenCenterY)
                {
                    TargetY = -(y - ScreenCenterY);
                    if (TargetY + ScreenCenterY < 0) TargetY = 0;
                }
            }
            TargetX /= (int)_AimSpeed;
            TargetY /= (int)_AimSpeed;
            if (Math.Abs(TargetX) < 1)
            {
                if (TargetX > 0)
                {
                    TargetX = 1;
                }
                if (TargetX < 0)
                {
                    TargetX = -1;
                }
            }
            if (Math.Abs(TargetY) < 1)
            {
                if (TargetY > 0)
                {
                    TargetY = 1;
                }
                if (TargetY < 0)
                {
                    TargetY = -1;
                }
            }
            Move((int)TargetX, (int)TargetY);
        }
    }
}
