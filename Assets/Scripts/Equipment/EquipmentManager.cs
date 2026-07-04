using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EquipmentManager : NetworkBehaviour
{

    public List<ItemManager> allEquipment = new List<ItemManager>();
    public List<ItemManager> EquippedEquipment = new List<ItemManager>();
    public ItemManager primaryAugment;
    public ItemManager secondaryAugment;
    public Transform MonocleSlot;
    public Transform armorSlot;
    public Transform leftGauntletSlot;
    public Transform rightGauntletSlot;
    public Transform leftBootSlot;
    public Transform rightBootSlot;

    GameObject currentMonocle;
    GameObject currentArmor;
    GameObject currentGauntletL;
    GameObject currentGauntletR;
    GameObject currentBootL;
    GameObject currentBootR;

    Player player;

    void Start()
    {
        
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Everyone)]
    public void FindItemByIndexRpc(int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= allEquipment.Count) return;

        ItemManager item = allEquipment[itemIndex];
        Player player = GetComponent<Player>();

        EquippedEquipment.Add(item);
        EquipItemRpc(item);
        if (primaryAugment == null)
        {
            primaryAugment = item;
            player.playerBaseMinAP += item.attackBonus;
            player.playerBaseMaxAP += item.attackBonus / 2;
            player.playerBaseDefense += item.defenseBonus;
            player.playerBaseCurrentHealth.Value += item.healthBonus;
            player.playerBaseMaxHealth += item.healthBonus;
            player.baseWalkSpeed += item.speedBonus;
            player.runSpeed += item.speedBonus;
            player.playerBaseSense += item.senseBonus;
            player.skillTrigger = item.animTrigger;
            player.skillCooldown += item.skillCooldown;
            player.uniqueSkillDamage += item.baseDamage;
        }

       else  if (secondaryAugment == null)
        {
            secondaryAugment = item;
            player.playerBaseMinAP += item.attackBonus / 2;
            player.playerBaseMaxAP += item.attackBonus / 4;
            player.playerBaseDefense += item.defenseBonus / 2;
            player.playerBaseCurrentHealth.Value += item.healthBonus;
            player.playerBaseMaxHealth += item.healthBonus / 2;
            player.baseWalkSpeed += item.speedBonus /2;
            player.runSpeed += item.speedBonus /2;
            player.playerBaseSense += item.senseBonus / 2;
            player.healValue += item.healingBonus;
            player.skillCooldown *= (1 - item.cooldownReductionBonus) ;
            player.playerBaseDefense += Mathf.RoundToInt(item.damageReductionBonus * (3f / 2f));
            player.playerBaseMaxHealth += Mathf.RoundToInt(item.damageReductionBonus * (9f/2f));
            player.heavyDamage /= Mathf.RoundToInt(item.damageReductionBonus / 3f);
            player.uniqueSkillDamage /= item.uniqueSkillBonus / 3;
        }
     

            Debug.Log("Equipped: " + primaryAugment + " and " + secondaryAugment);
    }

    void EquipAugmentRpc(int itemIndex, bool isPrimary)
    {

    }

    public void EquipItemRpc(ItemManager item)
    {
        Debug.Log("ITEM TYPE: " + item.equipmentType);
      
        
            switch (item.equipmentType)
            {
                case ItemManager.EquipmentType.Monocle:

                    currentMonocle = Instantiate(item.itemPrefab1, MonocleSlot);

                    currentMonocle.transform.localPosition = Vector3.zero;
                    currentMonocle.transform.localRotation = Quaternion.identity;
                  

                    break;

                case ItemManager.EquipmentType.Armor:
                    Debug.Log("EQUIPPING ARMOR");

                    currentArmor = Instantiate(item.itemPrefab1, armorSlot);

                    currentArmor.transform.localPosition = Vector3.zero;
                    currentArmor.transform.localRotation = Quaternion.identity;
                   

                break;

                case ItemManager.EquipmentType.Weapon:

                    Debug.Log("EQUIPPING GAUNTLETS");




                    currentGauntletL = Instantiate(item.itemPrefab1, leftGauntletSlot);
                    currentGauntletR = Instantiate(item.itemPrefab2, rightGauntletSlot);
                   



                currentGauntletL.transform.localPosition = Vector3.zero;
                    currentGauntletL.transform.localRotation = Quaternion.identity;

                    currentGauntletR.transform.localPosition = Vector3.zero;
                    currentGauntletR.transform.localRotation = Quaternion.identity;
                  


                break;

                case ItemManager.EquipmentType.Boots:
                    Debug.Log("EQUIPPING BOOTS");



                    currentBootL = Instantiate(item.itemPrefab1, leftBootSlot);
                    currentBootR = Instantiate(item.itemPrefab2, rightBootSlot);
                 


                    currentBootL.transform.localPosition = Vector3.zero;
                    currentBootL.transform.localRotation = Quaternion.identity;

                    currentBootR.transform.localPosition = Vector3.zero;
                    currentBootR.transform.localRotation = Quaternion.identity;
                   



                break;
            }
        }
       
    
}