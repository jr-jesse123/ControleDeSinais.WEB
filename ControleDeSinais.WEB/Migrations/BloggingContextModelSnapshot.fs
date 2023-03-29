﻿// <auto-generated />
namespace ControleDeSinais.WEB.Migrations

open System
open Microsoft.EntityFrameworkCore
open Microsoft.EntityFrameworkCore.Infrastructure
open Microsoft.EntityFrameworkCore.Metadata
open Microsoft.EntityFrameworkCore.Migrations
open Microsoft.EntityFrameworkCore.Storage.ValueConversion

[<DbContext(typeof<InfraEstrutura.Persistencia.BloggingContext>)>]
type BloggingContextModelSnapshot() =
    inherit ModelSnapshot()

    override this.BuildModel(modelBuilder: ModelBuilder) =
        modelBuilder.HasAnnotation("ProductVersion", "6.0.15") |> ignore

        modelBuilder.Entity("InfraEstrutura+Blog", (fun b ->

            b.Property<int>("Id")
                .IsRequired(true)
                .ValueGeneratedOnAdd()
                .HasColumnType("INTEGER")
                |> ignore

            b.Property<string>("Url")
                .IsRequired(true)
                .HasColumnType("TEXT")
                |> ignore

            b.HasKey("Id")
                |> ignore


            b.ToTable("Blogs") |> ignore

        )) |> ignore

