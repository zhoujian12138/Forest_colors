using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemPickUp : MonoBehaviour
{
    public ItemData_SO itemData;
    public AudioClip getObject;


    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            //将物品添加到背包
            AudioManager.instance.PlaySound(getObject, transform.position);
            InventoryManager.Instance.inventoryData.AddItem(itemData, itemData.itemAmount);
            InventoryManager.Instance.inventoryUI.RefreshUI();
            //装备武器
            //  GameManager.Instance.playerStats.EquipWeapon(itemData);

            //检查是否有任务
            QuestManager.Instance.UpdateQuestProgress(itemData.itemName, itemData.itemAmount);
            Destroy(gameObject);
        }
    }       
    
}
