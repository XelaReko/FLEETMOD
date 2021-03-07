﻿using System;
using HarmonyLib;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLUIPlayMenu), "ActuallyJoinRoom")]
	internal class ActuallyJoinRoom
	{
		public static bool Prefix(RoomInfo room)
		{
			bool result;
			if ((int)room.CustomProperties["CurrentPlayersPlusBots"] < (int)room.MaxPlayers)
			{
				if (room.CustomProperties.ContainsKey("SteamServerID"))
				{
					if (!SteamManager.Initialized)
					{
						PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLErrorMessageMenu("Failed to join crew! Can't join Secured game when not logged in to Steam!"));
						return false;
					}
					uint num = (uint)((long)room.CustomProperties["SteamServerID"]);
					PLNetworkManager.Instance.ClearSteamAuthSession(num);
					PLNetworkManager.Instance.SetSteamAuthTicket(num);
				}
				if ((string)room.CustomProperties["Ship_Type"] == Plugin.myversion)
				{
					MyVariables.isrunningmod = true;
					PLNetworkManager.Instance.JoinRoom(room);
					PLNetworkManager.Instance.StartCoroutine("ServerWaitForNetwork");
					PLLoader.Instance.IsWaitingOnNetwork = true;
				}
				if (!room.CustomProperties["Ship_Type"].ToString().Contains("FLEETMOD"))
				{
					MyVariables.isrunningmod = false;
					PLNetworkManager.Instance.JoinRoom(room);
					PLNetworkManager.Instance.StartCoroutine("ServerWaitForNetwork");
					PLLoader.Instance.IsWaitingOnNetwork = true;
				}
				if ((string)room.CustomProperties["Ship_Type"] != Plugin.myversion && room.CustomProperties["Ship_Type"].ToString().Contains("."))
				{
					MyVariables.isrunningmod = false;
					PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLErrorMessageMenu("Sorry, This Server Is Running FleetMod Version " + (string)room.CustomProperties["Ship_Type"] + "\n You Have Version " + Plugin.myversion));
				}
				result = false;
			}
			else
			{
				PLTabMenu.Instance.TimedErrorMsg = "Couldn't join crew! It is full!";
				result = false;
			}
			return result;
		}
	}
}
