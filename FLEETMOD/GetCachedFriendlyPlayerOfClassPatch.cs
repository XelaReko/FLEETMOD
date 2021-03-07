﻿using System;
using HarmonyLib;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLServer), "GetCachedFriendlyPlayerOfClass", new Type[]
	{
		typeof(int)
	})]
	internal class GetCachedFriendlyPlayerOfClassPatch
	{
		public static bool Prefix(PLServer __instance, ref int inClass)
		{
			bool result;
			if (!MyVariables.isrunningmod)
			{
				result = true;
			}
			else
			{
				if (inClass == 0)
				{
					result = PLServer.Instance.GetPlayerFromPlayerID(0);
				}
				else
				{
					foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
					{
						if (plplayer != null && !plplayer.IsBot && PLNetworkManager.Instance.LocalPlayer != null && PLNetworkManager.Instance.LocalPlayer.StartingShip != null && plplayer.StartingShip == PLNetworkManager.Instance.LocalPlayer.StartingShip && plplayer.StartingShip != null && plplayer.GetClassID() == inClass && inClass != 0 && __instance != null)
						{
							return plplayer;
						}
					}
					result = false;
				}
			}
			return result;
		}
	}
}
