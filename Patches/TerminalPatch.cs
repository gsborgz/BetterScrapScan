using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BetterScrapScan.Patches
{

    [HarmonyPatch(typeof(Terminal))]
    internal class TerminalPatch
    {
        [HarmonyPatch(typeof(Terminal), "TextPostProcess")]
        [HarmonyPostfix]
        static void ScanForItemsPatch(ref string modifiedDisplayText, ref TerminalNode node, Terminal __instance)
        {
            string modifiedText = modifiedDisplayText;
            ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("TextPostProcess");

            if (node.displayText.ToString().Contains("[scanForItems]"))
            {
                List<ItemCount> listedItems = new List<ItemCount>();
                GrabbableObject[] items = UnityEngine.Object.FindObjectsOfType<GrabbableObject>();

                for (int index = 0; index < items.Length; index++)
                {
                    GrabbableObject item = items[index];

                    if (item.itemProperties.isScrap && !item.isInShipRoom && !items[index].isInElevator)
                    {
                        bool itemAlreadyListed = listedItems.Exists(listedItem => listedItem.itemName == item.itemProperties.itemName);

                        if (itemAlreadyListed)
                        {
                            int listedItemIndex = listedItems.FindIndex(listedItem => listedItem.itemName == item.itemProperties.itemName);

                            if (listedItemIndex >= 0)
                            {
                                ItemCount listedItemToEdit = listedItems[listedItemIndex];

                                listedItemToEdit.count++;

                                listedItems[listedItemIndex] = listedItemToEdit;
                            }
                        } else
                        {
                            ItemCount itemCount = new ItemCount
                            {
                                count = 1,
                                itemName = item.itemProperties.itemName,
                                minValue = item.itemProperties.minValue,
                                maxValue = item.itemProperties.maxValue,
                            };

                            listedItems.Add(itemCount);
                        }
                    }
                }

                modifiedText += "\n\n Items:";

                listedItems.ForEach(item => {
                    modifiedText += $"\n {item.itemName} x{item.count} (${item.minValue} ~ ${item.maxValue})";
                });

                __instance.screenText.text = modifiedText;
                __instance.currentText = modifiedText;
            }
        }
    }
}
