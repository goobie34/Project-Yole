using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class StateMachineBuilder : MonoBehaviour
{
    readonly State root;

    public StateMachineBuilder(State root)
    {
        this.root = root;
    }

    public StateMachine Build()
    {
        var m = new StateMachine(root);
        Wire(root, m, new HashSet<State>());
        return m;
    }

    void Wire(State s, StateMachine m, HashSet<State> visited)
    {
        if (s == null) return;
        if (!visited.Add(s)) return; // State is already wired

        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
        var machineField = typeof(State).GetField("Machine", flags);
        if (machineField != null) machineField.SetValue(s, m);

        foreach (var fld in s.GetType().GetFields())
        {
            if (!typeof(State).IsAssignableFrom(fld.FieldType)) continue; // Only consider fields that are State
            if (fld.Name == "Parent") continue; // Skip back-edge to parent

            var child = (State)fld.GetValue(s);
            if (child == null) continue;
            if (!ReferenceEquals(child.Parent, s)) continue; // Ensure it's actually our direct child

            Wire(child, m, visited); // Recurse into the child
        }
    }
}
