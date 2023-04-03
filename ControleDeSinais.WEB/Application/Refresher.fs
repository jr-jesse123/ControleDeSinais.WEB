module Refresher

open System.IO
open Microsoft.AspNetCore.SignalR
open Microsoft.Extensions.Hosting
open System.Threading.Tasks
open ControleDeSinais.WEB
open System
//TODO: CONFIGURAR PATH PARA SER RELATIVO AO PROJETO
open Microsoft.FSharp.Control


//watcher.Changed.Add (fun _ -> Console.WriteLine "changed")


//TODO: MELHORAR INTEROPERABILIDADE COM TASK
type ViewWatherIHostedService(refresher:IHubContext<RefreshHub>) =
    let watcher = new FileSystemWatcher(@"C:\development\ControleDeSinais\ControleDeSinais.WEB\ControleDeSinais.WEB\Views\", EnableRaisingEvents = true, IncludeSubdirectories = true)
    //do 
    //    watcher.Changed
    //    |> Event.filter (fun e -> e.FullPath.EndsWith(".cshtml"))
    //    |> Event.add (fun _ -> refresher.Clients.All.SendAsync("refresh").Wait())
    //    |> ignore

    //TODO: PENSAR PORQUE EU PRECISO DA INTERFACE HOSTEDSERVICE
    interface IHostedService with
        member this.StartAsync(cancellationToken) =

            watcher.Changed
            //|> Event.filter (fun e -> e.FullPath.Contains("Views")) //TODO: ADICIONAR FILTRO CORRETO
            |> Event.add (fun _ -> refresher.Clients.All.SendAsync("refresh").Wait())
            |> ignore

            Task.CompletedTask   
                


            

         


        member this.StopAsync(cancellationToken) =
            Task.CompletedTask


