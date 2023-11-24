using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModifierNameSpace
{
    [CreateAssetMenu(fileName = "Modifier", menuName = "Modifiers/Classic", order = 1)]
    public abstract class Modifier : ScriptableObject
    {
        [Header("MAIN PARAMETERS")]
        public string modifierName;
        [TextArea] public string modifierDescription;
        public ModifierType type;

        public abstract void ApplyModifier();
    }

    public enum ModifierType
    {
        VALUE,
        METHOD
    }   
}
