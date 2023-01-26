using Unity.Netcode.Components;
using UnityEngine;

[DisallowMultipleComponent]
public class ClientSyncTransform : NetworkTransform
{
    protected override bool OnIsServerAuthoritative() {
        return false;
    }
}
