using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest Data")]
public class QuestData_SO : ScriptableObject
{
    [System.Serializable]
    public class QuestRequire
    {
        public string name;
        public int requireAmount;
        public int currentAmount;
    }

    public string questName;
    [TextArea]
    public string description;

    public bool isStarted;
    public bool isComplete;
    public bool isFinished;

    public List<QuestRequire> questRequires = new List<QuestRequire> ();
    public List<InventoryItem> rewards = new List<InventoryItem> ();
  
    public void CheckQuestProgress()
    {
        var finishRequires = questRequires.Where(r => r.requireAmount <= r.currentAmount);
        isComplete = finishRequires.Count() == questRequires.Count;

        if(isComplete)
        {
            Debug.Log("?????��??");
        }
    }

    public void GiveRewards()
    {
        foreach (var reward in rewards)
        {
            if (reward.amount < 0)//��Ҫ�Ͻ�������Ʒ�����
            {
                int requireCount = Mathf.Abs(reward.amount);

                if (InventoryManager.Instance.QuestItemInBag(reward.itemData) != null)//������������Ҫ������Ʒ
                {
                    if (InventoryManager.Instance.QuestItemInBag(reward.itemData).amount <= requireCount)//����������Ҫ�Ͻ���Ʒ�������պù����߲��������
                    {
                        requireCount -= InventoryManager.Instance.QuestItemInBag(reward.itemData).amount;
                        InventoryManager.Instance.QuestItemInBag(reward.itemData).amount = 0;

                        if (InventoryManager.Instance.QuestItemInAction(reward.itemData) != null)
                            InventoryManager.Instance.QuestItemInAction(reward.itemData).amount -= requireCount;
                    }
                    else//���������Ͻ���Ʒ����������
                    {
                        InventoryManager.Instance.QuestItemInBag(reward.itemData).amount -= requireCount;
                    }
                }
                else//��������û���Ͻ���Ʒ����Action��һ��������������Ʒ������
                {
                    InventoryManager.Instance.QuestItemInAction(reward.itemData).amount -= requireCount;
                }
            }
            else//������õĶ�����Ʒ������ӵ�������
            {
                InventoryManager.Instance.inventoryData.AddItem(reward.itemData, reward.amount);
            }

            InventoryManager.Instance.inventoryUI.RefreshUI();
            InventoryManager.Instance.actionUI.RefreshUI();
        }
	}
		
  public List<string> RequireTargetName()
    {
        List<string> targetNameList = new List<string> ();

        foreach (var require in questRequires)
        {
            targetNameList.Add(require.name);
        }
        return targetNameList;
	}
  }
