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
            string displayText = modifiedDisplayText;

            if (node.displayText.ToString().Contains("[scanForItems]"))
            {
                List<ItemCount> listedItems = new List<ItemCount>();
                GrabbableObject[] items = UnityEngine.Object.FindObjectsOfType<GrabbableObject>();

                System.Random random = new System.Random(StartOfRound.Instance.randomMapSeed + 91);
                int totalItems = 0;
                int approximateTotal = 0;
                GrabbableObject[] array = UnityEngine.Object.FindObjectsOfType<GrabbableObject>();

                for (int num5 = 0; num5 < array.Length; num5++)
                {
                    GrabbableObject item = array[num5];

                    if (item.itemProperties.isScrap && !item.isInShipRoom && !item.isInElevator)
                    {
                        int approximateItemValue = Mathf.Clamp(random.Next(item.itemProperties.minValue, item.itemProperties.maxValue), item.scrapValue - 6 * num5, item.scrapValue + 9 * num5);
                        approximateTotal += approximateItemValue;
                        totalItems++;

                        bool itemAlreadyListed = listedItems.Exists(listedItem => listedItem.itemName == item.itemProperties.itemName);

                        if (itemAlreadyListed)
                        {
                            int listedItemIndex = listedItems.FindIndex(listedItem => listedItem.itemName == item.itemProperties.itemName);

                            if (listedItemIndex >= 0)
                            {
                                ItemCount listedItemToEdit = listedItems[listedItemIndex];

                                listedItemToEdit.count++;
                                listedItemToEdit.approximateValue += approximateItemValue;

                                listedItems[listedItemIndex] = listedItemToEdit;
                            }
                        }
                        else
                        {
                            ItemCount itemCount = new ItemCount
                            {
                                count = 1,
                                itemName = item.itemProperties.itemName,
                                approximateValue = approximateItemValue
                            };

                            listedItems.Add(itemCount);
                        }
                    }
                }

                if (listedItems.Count() > 0)
                {
                    displayText = $"\n\n\n\nTotal items found: {totalItems}";
                    displayText += $"\nTotal approximate value: ${approximateTotal}";
                    displayText += "\n\nItems:";

                    listedItems.ForEach(item => {
                        displayText += $"\n  {item.itemName}  x{item.count}  (${item.approximateValue})";
                    });
                }
                else
                {
                    displayText = $"\n\n\n\nNo items found.";
                }

                __instance.screenText.text = displayText;
                __instance.currentText = displayText;
            }
        }
    }
}
