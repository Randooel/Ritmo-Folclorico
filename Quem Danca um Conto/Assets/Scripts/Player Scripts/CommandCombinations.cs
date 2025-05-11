using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CommandsList", menuName = "CommandsList")]
public class CommandCombinations : ScriptableObject
{
    [System.Serializable]
    public class CommandCombination
    {
        public string commandName;
        public List<int> commandSequence = new List<int>();
    }

    public List<CommandCombination> commandSets = new List<CommandCombination>();

    public List<CommandCombination> GetCommandCombinations()
    {
        return commandSets;
    }
}
