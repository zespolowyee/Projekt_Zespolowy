using System.Threading.Tasks;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayController : MonoBehaviour
{
    public Allocation CurrentAllocation { get; set; }

    public async Task CreateAllocation(int maxPlayers)
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
        CurrentAllocation = allocation;
    }

    public async Task<string> GetJoinCode()
    {
        if (CurrentAllocation == null) return "";

        return await RelayService.Instance.GetJoinCodeAsync(CurrentAllocation.AllocationId);
    }
    
    public async Task<JoinAllocation> JoinAllocation(string joinCode)
    {
        return await RelayService.Instance.JoinAllocationAsync(joinCode);
    }
}