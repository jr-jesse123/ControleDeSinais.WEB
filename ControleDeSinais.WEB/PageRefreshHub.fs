namespace ControleDeSinais.WEB
open Microsoft.AspNetCore.SignalR
open System.Threading.Tasks

type RefreshHub () =
    inherit Hub()
    
    member this.Refresh () =
        this.Clients.All.SendAsync("refresh").Wait()