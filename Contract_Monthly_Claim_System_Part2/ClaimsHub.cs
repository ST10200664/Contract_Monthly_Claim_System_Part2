namespace Contract_Monthly_Claim_System_Part2
{
    using Microsoft.AspNetCore.SignalR;

    public class ClaimsHub : Hub
    {
        public async Task UpdateClaimStatus(int claimId, string status)
        {
            await Clients.All.SendAsync("ReceiveClaimStatusUpdate", claimId, status);
        }
    }

}
