using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CommandsList", menuName = "CommandsList")]
public class CommandCombinations : ScriptableObject
{
    public enum MouseButton
    {
        MouseButton0 = 0,
        MouseButton1 = 1
    }

    [System.Serializable]
    public class CommandCombination
    {
        public string name;
        public List<MouseButton> mouseButtons = new List<MouseButton>();
    }

    public List<CommandCombination> commandCombinations = new List<CommandCombination>();

    public List<CommandCombination> GetCommandCombinations()
    {
        return commandCombinations;
    }
}
