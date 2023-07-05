using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/**
 * 
 * Request state authority and then request the input authority
 * Relevant in shared topology only
 * 
 **/

public static class SharedAuthorityExtensions
{
    public static async Task<bool> RequestAllAuthority(this NetworkObject o, float maxWaitTime = 8)
    {
        float waitStartTime = Time.time;
        o.RequestStateAuthority();
        while (!o.HasStateAuthority && (Time.time - waitStartTime) < maxWaitTime)
        {
            await System.Threading.Tasks.Task.Delay(1);
        }
        if (!o.HasStateAuthority)
        {
            return false;
        }
        o.AssignInputAuthority(o.Runner.LocalPlayer);
        while (!o.HasInputAuthority && (Time.time - waitStartTime) < maxWaitTime)
        {
            await System.Threading.Tasks.Task.Delay(1);
        }
        return o.HasStateAuthority && o.HasInputAuthority;
    }

    public static async void EnsureAuthorityIsAttributed(this NetworkObject o)
    {
        await EnsureAuthorityIsAttributedAsync(o);
    }
    /**
     * In shared mode, when the state authority authority of an object disconnects,
     *  it my end with no state authority at all.
     *  If required, this method ensure that a player has the state (and input) authority on this NetworkObject, by giving the authorities for this object to the player with the lowest PlayerId
     **/
    public static async Task EnsureAuthorityIsAttributedAsync(this NetworkObject o)
    {
        if (o.StateAuthority == PlayerRef.None)
        {
            // Ensure we have an authority
            int minPlayerId = int.MaxValue;
            foreach (var activePlayer in o.Runner.ActivePlayers)
            {
                if (activePlayer.PlayerId < minPlayerId) minPlayerId = activePlayer.PlayerId;
            }

            if (o.Runner.LocalPlayer.PlayerId == minPlayerId)
            {
                // If we are the player with the lowest id (to haver unicity) in the list, we take the authority
                await o.RequestAllAuthority();
            }
        }
    }
}
