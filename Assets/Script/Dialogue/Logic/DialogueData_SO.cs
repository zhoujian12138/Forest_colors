using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Dialogue", menuName = "Dialogue/DialogueData")]
public class DialogueData_SO : ScriptableObject
{
    public List<DialoguePiece> dialoguePieces = new List<DialoguePiece>();
}
