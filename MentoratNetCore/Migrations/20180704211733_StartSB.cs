using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MentoratNetCore.Migrations
{
    public partial class StartSB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetCategorieUser",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetCategorieUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Expertises",
                columns: table => new
                {
                    No_Expertise = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    No_Regroupement_Expertise = table.Column<int>(nullable: true),
                    Nom_Expertise = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expertises", x => x.No_Expertise);
                });

            migrationBuilder.CreateTable(
                name: "Experts",
                columns: table => new
                {
                    No_Expert = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nom_Expert = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Experts", x => x.No_Expert);
                });

            migrationBuilder.CreateTable(
                name: "MentoratCategorie",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Nom = table.Column<string>(maxLength: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MentoratCategorie", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mentors",
                columns: table => new
                {
                    No_Mentor = table.Column<string>(maxLength: 128, nullable: false),
                    Courriel_Mentor = table.Column<string>(maxLength: 50, nullable: false),
                    DateConnexion_Mentor = table.Column<DateTime>(nullable: true),
                    NoTPS_Mentor = table.Column<string>(maxLength: 255, nullable: true),
                    NoTVQ_Mentor = table.Column<string>(maxLength: 255, nullable: true),
                    Nom_Mentor = table.Column<string>(maxLength: 255, nullable: true),
                    Prenom_Mentor = table.Column<string>(maxLength: 255, nullable: true),
                    Taxe_Mentor = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mentors", x => x.No_Mentor);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    IdCategorieUtilisateur = table.Column<int>(nullable: true),
                    IdParent = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NomLong = table.Column<string>(nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoles_AspNetCategorieUser_IdCategorieUtilisateur",
                        column: x => x.IdCategorieUtilisateur,
                        principalTable: "AspNetCategorieUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetRoles_AspNetRoles_IdParent",
                        column: x => x.IdParent,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    ActifUser = table.Column<bool>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    IdCategorieUtilisateur = table.Column<int>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    NomUser = table.Column<string>(nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    PrenomUser = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_AspNetCategorieUser_IdCategorieUtilisateur",
                        column: x => x.IdCategorieUtilisateur,
                        principalTable: "AspNetCategorieUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MentoratCategorieMentors",
                columns: table => new
                {
                    No_Mentor = table.Column<string>(nullable: false),
                    MentoratCategorieId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MentoratCategorieMentors", x => new { x.No_Mentor, x.MentoratCategorieId });
                    table.ForeignKey(
                        name: "FK_MentoratCategorieMentors_MentoratCategorie_MentoratCategorieId",
                        column: x => x.MentoratCategorieId,
                        principalTable: "MentoratCategorie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MentoratCategorieMentors_Mentors_No_Mentor",
                        column: x => x.No_Mentor,
                        principalTable: "Mentors",
                        principalColumn: "No_Mentor",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Mentores",
                columns: table => new
                {
                    No_Mentore = table.Column<string>(maxLength: 128, nullable: false),
                    Cellulaire_Mentore = table.Column<string>(maxLength: 15, nullable: true),
                    Courriel_Mentore = table.Column<string>(maxLength: 50, nullable: false),
                    DateInscription_Mentore = table.Column<DateTime>(nullable: true),
                    MentoratCategorieId = table.Column<int>(nullable: true),
                    MotPasse_Mentore = table.Column<string>(maxLength: 255, nullable: true),
                    No_Expert_Mentore = table.Column<int>(nullable: true),
                    No_Mentor_Mentore = table.Column<string>(maxLength: 128, nullable: true),
                    Nom_Mentore = table.Column<string>(maxLength: 30, nullable: false),
                    Objectifs_Mentore = table.Column<string>(type: "ntext", maxLength: 1000, nullable: true),
                    Organisme_Mentore = table.Column<string>(maxLength: 100, nullable: false),
                    Paye_Mentore = table.Column<bool>(nullable: false),
                    Prenom_Mentore = table.Column<string>(maxLength: 30, nullable: false),
                    Telephone_Mentore = table.Column<string>(maxLength: 15, nullable: false),
                    upsize_ts = table.Column<byte[]>(type: "timestamp", maxLength: 8, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mentores", x => x.No_Mentore);
                    table.ForeignKey(
                        name: "FK_Mentores_MentoratCategorie_MentoratCategorieId",
                        column: x => x.MentoratCategorieId,
                        principalTable: "MentoratCategorie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mentores_Mentors_No_Mentor_Mentore",
                        column: x => x.No_Mentor_Mentore,
                        principalTable: "Mentors",
                        principalColumn: "No_Mentor",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Interventions",
                columns: table => new
                {
                    No_Intervention = table.Column<string>(maxLength: 128, nullable: false),
                    Date_Intervention = table.Column<DateTime>(nullable: true),
                    Description_Intervention = table.Column<string>(maxLength: 500, nullable: true),
                    Duree_Intervention = table.Column<int>(nullable: true),
                    No_Mentor_Intervention = table.Column<string>(maxLength: 128, nullable: true),
                    No_Mentore_Intervention = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interventions", x => x.No_Intervention);
                    table.ForeignKey(
                        name: "FK_Interventions_Mentors_No_Mentor_Intervention",
                        column: x => x.No_Mentor_Intervention,
                        principalTable: "Mentors",
                        principalColumn: "No_Mentor",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Interventions_Mentores_No_Mentore_Intervention",
                        column: x => x.No_Mentore_Intervention,
                        principalTable: "Mentores",
                        principalColumn: "No_Mentore",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MentoratInscription",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    APaye = table.Column<bool>(nullable: false),
                    Annee = table.Column<int>(nullable: false),
                    DateDebut = table.Column<DateTime>(nullable: false),
                    DateFin = table.Column<DateTime>(nullable: false),
                    DateInscription = table.Column<DateTime>(nullable: false),
                    MentorNoMentor = table.Column<string>(nullable: true),
                    MentoratCategorieId = table.Column<int>(nullable: true),
                    MentoreNo_Mentore = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MentoratInscription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MentoratInscription_Mentors_MentorNoMentor",
                        column: x => x.MentorNoMentor,
                        principalTable: "Mentors",
                        principalColumn: "No_Mentor",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MentoratInscription_MentoratCategorie_MentoratCategorieId",
                        column: x => x.MentoratCategorieId,
                        principalTable: "MentoratCategorie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MentoratInscription_Mentores_MentoreNo_Mentore",
                        column: x => x.MentoreNo_Mentore,
                        principalTable: "Mentores",
                        principalColumn: "No_Mentore",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MentoresExpertises",
                columns: table => new
                {
                    No_Expertise = table.Column<int>(nullable: false),
                    No_Mentore = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MentoresExpertises", x => new { x.No_Expertise, x.No_Mentore });
                    table.ForeignKey(
                        name: "FK_MentoresExpertises_Expertises_No_Expertise",
                        column: x => x.No_Expertise,
                        principalTable: "Expertises",
                        principalColumn: "No_Expertise",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MentoresExpertises_Mentores_No_Mentore",
                        column: x => x.No_Mentore,
                        principalTable: "Mentores",
                        principalColumn: "No_Mentore",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlanAction",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Actions = table.Column<string>(nullable: true),
                    Echeancier = table.Column<string>(nullable: true),
                    Evaluation = table.Column<string>(nullable: true),
                    Indicateurs = table.Column<string>(nullable: true),
                    InscriptionId = table.Column<string>(nullable: false),
                    Objectifs = table.Column<string>(nullable: true),
                    Ordre = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanAction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanAction_MentoratInscription_InscriptionId",
                        column: x => x.InscriptionId,
                        principalTable: "MentoratInscription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_IdCategorieUtilisateur",
                table: "AspNetRoles",
                column: "IdCategorieUtilisateur");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_IdParent",
                table: "AspNetRoles",
                column: "IdParent");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IdCategorieUtilisateur",
                table: "AspNetUsers",
                column: "IdCategorieUtilisateur");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_No_Mentor_Intervention",
                table: "Interventions",
                column: "No_Mentor_Intervention");

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_No_Mentore_Intervention",
                table: "Interventions",
                column: "No_Mentore_Intervention");

            migrationBuilder.CreateIndex(
                name: "IX_MentoratCategorieMentors_MentoratCategorieId",
                table: "MentoratCategorieMentors",
                column: "MentoratCategorieId");

            migrationBuilder.CreateIndex(
                name: "IX_MentoratInscription_MentorNoMentor",
                table: "MentoratInscription",
                column: "MentorNoMentor");

            migrationBuilder.CreateIndex(
                name: "IX_MentoratInscription_MentoratCategorieId",
                table: "MentoratInscription",
                column: "MentoratCategorieId");

            migrationBuilder.CreateIndex(
                name: "IX_MentoratInscription_MentoreNo_Mentore",
                table: "MentoratInscription",
                column: "MentoreNo_Mentore");

            migrationBuilder.CreateIndex(
                name: "IX_Mentores_MentoratCategorieId",
                table: "Mentores",
                column: "MentoratCategorieId");

            migrationBuilder.CreateIndex(
                name: "IX_Mentores_No_Mentor_Mentore",
                table: "Mentores",
                column: "No_Mentor_Mentore");

            migrationBuilder.CreateIndex(
                name: "IX_MentoresExpertises_No_Mentore",
                table: "MentoresExpertises",
                column: "No_Mentore");

            migrationBuilder.CreateIndex(
                name: "IX_PlanAction_InscriptionId",
                table: "PlanAction",
                column: "InscriptionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Experts");

            migrationBuilder.DropTable(
                name: "Interventions");

            migrationBuilder.DropTable(
                name: "MentoratCategorieMentors");

            migrationBuilder.DropTable(
                name: "MentoresExpertises");

            migrationBuilder.DropTable(
                name: "PlanAction");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Expertises");

            migrationBuilder.DropTable(
                name: "MentoratInscription");

            migrationBuilder.DropTable(
                name: "AspNetCategorieUser");

            migrationBuilder.DropTable(
                name: "Mentores");

            migrationBuilder.DropTable(
                name: "MentoratCategorie");

            migrationBuilder.DropTable(
                name: "Mentors");
        }
    }
}
