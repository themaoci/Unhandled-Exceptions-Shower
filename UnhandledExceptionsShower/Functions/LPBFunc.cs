using System;
using System.Collections.Generic;
using EFT;
using System.IO;

namespace UnhandledExceptionHandler.Functions
{
    class LPBFunc
    {
        public static void GLPB(IEnumerable<Player> _ply, ref Player _lP)
        {
            if (_ply != null)
            {
                var e = _ply.GetEnumerator();
                while (e.MoveNext())
                {
                    try
                    {
                        var player = e.Current;
                        if (player != null)
                            if (EPointOfView.FirstPerson == player.PointOfView)
                                _lP = player;
                    }
                    catch (NullReferenceException ex)
                    {
                    }
                }
            }
        }
    }
}
