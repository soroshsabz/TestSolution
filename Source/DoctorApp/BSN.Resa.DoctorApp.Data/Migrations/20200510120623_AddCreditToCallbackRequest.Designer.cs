﻿// <auto-generated />
using System;
using BSN.Resa.DoctorApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BSN.Resa.DoctorApp.Data.Migrations
{
    [DbContext(typeof(DoctorAppContext))]
    [Migration("20200510120623_AddCreditToCallbackRequest")]
    partial class AddCreditToCallbackRequest
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854");

            modelBuilder.Entity("BSN.Resa.DoctorApp.Domain.Models.AppUpdate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("HasNotifiableUpdateLocally");

                    b.Property<bool>("HasUrgentUpdateLocally");

                    b.Property<DateTime?>("LastSynchronizationTime");

                    b.Property<string>("LatestDownloadableAppUpdateUrlLocally");

                    b.Property<string>("LatestDownloadableAppUpdateVersionLocallyInString");

                    b.HasKey("Id");

                    b.ToTable("AppUpdates");
                });

            modelBuilder.Entity("BSN.Resa.DoctorApp.Domain.Models.CallbackRequest", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CallerFullName");

                    b.Property<string>("CallerSubscriberNumber");

                    b.Property<int>("CommunicationAttemptsCount");

                    b.Property<DateTime>("ConsentGivenAt");

                    b.Property<long?>("Credit");

                    b.Property<bool>("IsCallTried");

                    b.Property<bool>("IsEstablishedCallNotified");

                    b.Property<bool>("IsExpired");

                    b.Property<bool>("IsSeen");

                    b.Property<DateTime>("LastCallTriedAt");

                    b.Property<string>("Message");

                    b.Property<string>("ReceiverFullName");

                    b.Property<string>("ReceiverSubscriberNumber");

                    b.Property<bool>("ReturnCallHasBeenEstablished");

                    b.HasKey("Id");

                    b.ToTable("CallbackRequests");
                });

            modelBuilder.Entity("BSN.Resa.DoctorApp.Domain.Models.Contact", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BlockedCount");

                    b.Property<int>("DoctorId");

                    b.Property<bool>("IsAnnouncedToService");

                    b.Property<bool>("IsBlocked");

                    b.Property<bool>("IsResaContact");

                    b.Property<bool>("IsVisible");

                    b.Property<string>("PhoneNumber");

                    b.HasKey("Id");

                    b.HasIndex("DoctorId");

                    b.HasIndex("PhoneNumber")
                        .IsUnique();

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("BSN.Resa.DoctorApp.Domain.Models.Doctor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FirstName");

                    b.Property<bool>("IsLoggedIn");

                    b.Property<string>("LastName");

                    b.Property<string>("Msisdn");

                    b.Property<int>("State");

                    b.HasKey("Id");

                    b.ToTable("Doctors");
                });

            modelBuilder.Entity("BSN.Resa.DoctorApp.Domain.Models.MedicalTest", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreatedAt");

                    b.Property<string>("PatientId");

                    b.Property<string>("PatientPhone");

                    b.Property<string>("Photos");

                    b.Property<int>("Price");

                    b.Property<int>("Status");

                    b.Property<DateTime?>("UpdatedAt");

                    b.HasKey("Id");

                    b.ToTable("MedicalTests");
                });

            modelBuilder.Entity("BSN.Resa.DoctorApp.Domain.Models.Contact", b =>
                {
                    b.HasOne("BSN.Resa.DoctorApp.Domain.Models.Doctor")
                        .WithMany("InternalContacts")
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BSN.Resa.DoctorApp.Domain.Models.Doctor", b =>
                {
                    b.OwnsOne("BSN.Resa.DoctorApp.Commons.ServiceCommunicators.OauthToken", "ServiceCommuncationToken", b1 =>
                        {
                            b1.Property<int>("DoctorId");

                            b1.Property<string>("AccessToken");

                            b1.Property<string>("ExpiresIn");

                            b1.Property<string>("TokenType");

                            b1.HasKey("DoctorId");

                            b1.ToTable("Doctors");

                            b1.HasOne("BSN.Resa.DoctorApp.Domain.Models.Doctor")
                                .WithOne("ServiceCommuncationToken")
                                .HasForeignKey("BSN.Resa.DoctorApp.Commons.ServiceCommunicators.OauthToken", "DoctorId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
