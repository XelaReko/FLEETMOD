﻿using System;
using CodeStage.AntiCheat.ObscuredTypes;
using HarmonyLib;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLShipStats), "CalculateStats")]
	internal class CalculateStats
	{
		public static void Postfix(ref ObscuredFloat ___m_WarpRange)
		{
			if (MyVariables.isrunningmod)
			{
				if (PLServer.Instance != null)
				{
					foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
					{
						if (plshipInfoBase != null && plshipInfoBase.TagID == -23)
						{
							___m_WarpRange = plshipInfoBase.MyStats.WarpRange;
							break;
						}
					}
				}/*    /// STARTED WORKING ON REWRITING WARP RANGE CALCULATION BUT NOT MUCH LUCK. PROBS LOOK INTO THE GAMES IMPLEMENTATION
                bool flag = PLServer.Instance != null;
                if (flag)
                {
                    if (PhotonNetwork.isMasterClient)
                    {
                        float lowestrange = -1;
                        foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
                        {
                            if (plshipInfoBase != null && plshipInfoBase.TagID == -23)
                            {
                                if (lowestrange > plshipInfoBase.MyStats.WarpRange || lowestrange == -1)
                                {
                                    lowestrange = plshipInfoBase.MyStats.WarpRange;
                                }
                            }
                        }
                        ___m_WarpRange = lowestrange;
                        MyVariables.warprange = ___m_WarpRange;
                    }
                    else
                    {
                        ___m_WarpRange = MyVariables.warprange;
                    }
                }*/
            }
		}
	}
}
