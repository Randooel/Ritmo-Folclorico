using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCommands : MonoBehaviour
{
    // READ ME: THIS SCRIPT CHECKS IF THE PLAYER'S INPUTS MATCH A VALID COMMAND IF SO, IT EXECUTES IT

    public bool commandable = true;

    [Header("Player Commands")]
    public List<int> mouseButtonPressed = new List<int>();
}
