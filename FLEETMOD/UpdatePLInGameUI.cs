﻿using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace FLEETMOD
{
	// Token: 0x02000016 RID: 22
	[HarmonyPatch(typeof(PLInGameUI), "Update")]
	internal class UpdatePLInGameUI
	{
		// Token: 0x06000028 RID: 40 RVA: 0x000050E8 File Offset: 0x000032E8
		public static void Postfix(PLInGameUI __instance, ref List<PLPlayer> ___relevantPlayersForCrewStatus, ref PLCachedFormatString<string> ___cSkipWarpLabel, ref Text[] ___CrewStatusSlots_HPs, ref Text[] ___CrewStatusSlots_Names, ref Image[] ___CrewStatusSlots_BGs, ref Image[] ___CrewStatusSlots_Fills, ref Image[] ___CrewStatusSlots_TalkingImages, ref Image[] ___CrewStatusSlots_SlowFills)
		{
			if (MyVariables.isrunningmod)
			{
				PLInGameUI.Instance.ControlsText.enabled = true;
				if (PLServer.Instance != null && PLNetworkManager.Instance.LocalPlayer != null && PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted())
				{
					if (PLServer.Instance != null && ___relevantPlayersForCrewStatus != null)
					{
						___relevantPlayersForCrewStatus.Clear();
						foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
						{
							if (plplayer != null && plplayer.GetClassID() != -1 && plplayer != PLNetworkManager.Instance.LocalPlayer && plplayer.TeamID == 0 && plplayer.GetHasStarted() && plplayer.StartingShip == PLNetworkManager.Instance.LocalPlayer.StartingShip)
							{
								___relevantPlayersForCrewStatus.Add(plplayer);
							}
						}
						___relevantPlayersForCrewStatus.Sort((PLPlayer a, PLPlayer b) => a.GetClassID().CompareTo(b.GetClassID()));
					}
					else if (___relevantPlayersForCrewStatus.Count > 0)
					{
						___relevantPlayersForCrewStatus.Clear();
					}
					PLGlobal.SafeGameObjectSetActive(__instance.CrewStatusRoot, !__instance.UIIsHidden && PLServer.Instance != null && !PLServer.Instance.PlayerShipIsDestroyed && PLEncounterManager.Instance.PlayerShip != null && PLXMLOptionsIO.Instance.CurrentOptions.GetStringValue("ShowCrewHealth") == "1");
					if (__instance.CrewStatusRoot.activeSelf)
					{
						int num = 0;
						foreach (GameObject gameObject in __instance.CrewStatusSlots)
						{
							PLPlayer plplayer2 = null;
							if (___relevantPlayersForCrewStatus.Count > num)
							{
								plplayer2 = ___relevantPlayersForCrewStatus[num];
							}
							PLGlobal.SafeGameObjectSetActive(gameObject, plplayer2 != null && plplayer2.GetClassID() != -1);
							if (gameObject.activeSelf)
							{
								if (plplayer2.GetPawn() == null || plplayer2.GetPawn().IsDead)
								{
									___CrewStatusSlots_HPs[num].text = "Dead";
								}
								else
								{
									___CrewStatusSlots_HPs[num].text = plplayer2.GetPawn().Health.ToString("0") + "/" + plplayer2.GetPawn().MaxHealth.ToString("0");
								}
								___CrewStatusSlots_Names[num].text = PLReadableStringManager.Instance.GetFormattedResultFromInputString(plplayer2.GetPlayerName(false));
								if (plplayer2.GetPawn() != null && Time.time - plplayer2.GetPawn().LastDamageTakenTime < 0.33f && Time.time * 18f % 1f < 0.5f)
								{
									___CrewStatusSlots_BGs[num].color = Color.red * 0.8f;
									___CrewStatusSlots_Fills[num].color = Color.red;
									___CrewStatusSlots_Names[num].color = Color.red;
									___CrewStatusSlots_TalkingImages[num].color = Color.red;
								}
								else
								{
									___CrewStatusSlots_BGs[num].color = PLInGameUI.FromAlpha(PLGlobal.Instance.ClassColors[plplayer2.GetClassID()] * 0.35f, 1f);
									___CrewStatusSlots_Fills[num].color = PLInGameUI.FromAlpha(PLGlobal.Instance.ClassColors[plplayer2.GetClassID()] * 0.7f, 1f);
									___CrewStatusSlots_Names[num].color = ___CrewStatusSlots_Fills[num].color;
									___CrewStatusSlots_TalkingImages[num].color = ___CrewStatusSlots_Fills[num].color;
									if (plplayer2.GetClassID() == 1)
									{
										___CrewStatusSlots_HPs[num].color = Color.black;
									}
									else
									{
										___CrewStatusSlots_HPs[num].color = Color.white;
									}
								}
								___CrewStatusSlots_TalkingImages[num].enabled = (plplayer2.TS_ValidClientID && plplayer2.TS_IsTalking);
								if (plplayer2.GetPawn() == null || plplayer2.GetPawn().IsDead)
								{
									___CrewStatusSlots_Fills[num].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
									___CrewStatusSlots_SlowFills[num].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
								}
								else
								{
									___CrewStatusSlots_Fills[num].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Clamp01(plplayer2.GetPawn().Health / plplayer2.GetPawn().MaxHealth) * 100f);
									___CrewStatusSlots_SlowFills[num].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Clamp01(plplayer2.GetPawn().SlowHealth / plplayer2.GetPawn().MaxHealth) * 99f);
								}
							}
							num++;
						}
					}
					if (PLNetworkManager.Instance != null && PLNetworkManager.Instance.LocalPlayer != null)
					{
						if (PLServer.Instance.GetPlayerFromPlayerID(0).GetPhotonPlayer().NickName == "skipwarp")
						{
							PLGlobal.SafeLabelSetText(PLInGameUI.Instance.SkipWarpLabel, ___cSkipWarpLabel.ToString(PLInput.Instance.GetPrimaryKeyStringForAction(PLInputBase.EInputActionName.skip_warp, true)));
							PLInGameUI.Instance.SkipWarpLabel.enabled = true;
						}
						else
						{
							PLInGameUI.Instance.SkipWarpLabel.enabled = false;
						}
					}
					else
					{
						PLInGameUI.Instance.SkipWarpLabel.enabled = false;
					}
					string a2 = "";
					if (PLCameraSystem.Instance != null)
					{
						a2 = PLCameraSystem.Instance.GetModeString();
					}
					if (UnityEngine.Random.Range(0, 20) == 0 || PLServer.Instance == null)
					{
						string text = "";
						if (PLInGameUI.Instance.ControlsText.enabled)
						{
							if (a2 == "LocalPawn")
							{
								if (PLServer.Instance.GetPlayerFromPlayerID(0).GetPhotonPlayer().NickName == "skipwarp")
								{
									text = text + "<color=#AAAAAA><color=#ffff00>" + PLInput.Instance.GetPrimaryKeyStringForAction(PLInputBase.EInputActionName.skip_warp, true) + "</color> Skip Warp</color>\n";
								}
							}
							else
							{
								text = "";
							}
						}
						PLGlobal.SafeLabelSetText(PLInGameUI.Instance.ControlsText, text);
					}
				}
			}
		}
	}
}
