using Microsoft.EntityFrameworkCore;
using ReactionLab.Data.Chemistry;
using ReactionLab.Data.Models;
using ReactionLab.Data.Reactions;

namespace ReactionLab.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<Reaction> Reactions => Set<Reaction>();
    public DbSet<Badge> Badges => Set<Badge>();
    public DbSet<UserBadge> UserBadges => Set<UserBadge>();
    public DbSet<UserProgress> UserProgress => Set<UserProgress>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
        });

        // UserBadge (many-to-many)
        modelBuilder.Entity<UserBadge>(entity =>
        {
            entity.HasKey(ub => new { ub.UserId, ub.BadgeId });

            entity.HasOne(ub => ub.User)
                .WithMany(u => u.UserBadges)
                .HasForeignKey(ub => ub.UserId);

            entity.HasOne(ub => ub.Badge)
                .WithMany(b => b.UserBadges)
                .HasForeignKey(ub => ub.BadgeId);
        });

        // UserProgress
        modelBuilder.Entity<UserProgress>(entity =>
        {
            entity.HasIndex(up => new { up.UserId, up.TopicId }).IsUnique();
        });

        // Reaction
        modelBuilder.Entity<Reaction>(entity =>
        {
            entity.HasOne(r => r.Topic)
                .WithMany(t => t.Reactions)
                .HasForeignKey(r => r.TopicId);
        });

        // Seed initial data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed badges
        modelBuilder.Entity<Badge>().HasData(
            new Badge { Id = 1, Name = "Early Bird",        Description = "Join ReactionLab" },
            new Badge { Id = 2, Name = "Iron Alchemist",    Description = "Complete the Iron Sulfide Formation reaction" },
            new Badge { Id = 3, Name = "Rising Chemist",    Description = "Complete your second topic" },
            new Badge { Id = 4, Name = "Lab Veteran",       Description = "Complete your third topic" },
            new Badge { Id = 5, Name = "Chemistry Master",  Description = "Complete your fourth topic" },
            new Badge { Id = 6, Name = "Molecular Vision",  Description = "Switch visualization mode for the first time" },
            new Badge { Id = 7, Name = "Full Circuit",      Description = "Reach the final step of any reaction" },
            new Badge { Id = 8, Name = "Completionist",     Description = "Complete all topics in the lab" }
        );

        // Seed topics
        modelBuilder.Entity<Topic>().HasData(
            new Topic { Id = 1, Title = "Iron sulfide formation",       Description = "Witness two substances fuse into a hot glow to learn how heat transforms separate elements into a single compound.", Order = 1 },
            new Topic { Id = 2, Title = "Production of hydrogen gas",   Description = "Witness a metal dissolve into air and sends a frantic stream of bubbles of explosive gas.", Order = 2 },
            new Topic { Id = 3, Title = "Glowing splint test",          Description = "Decompose hydrogen peroxide with a catalyst and test the released oxygen using the glowing splint technique.", Order = 3 },
            new Topic { Id = 4, Title = "Sodium-water reaction",        Description = "Watch sodium react with water, releasing hydrogen gas and forming sodium hydroxide solution.", Order = 4 }
        );

        // Seed reactions
        modelBuilder.Entity<Reaction>().HasData(
            new Reaction
            {
                Id = 1,
                TopicId = 1,
                Title = "Iron sulfide formation",
                Equation = "8 Fe + 8 S → 8 FeS",
                MoleculeData = "[]",
                StepsData = StepJsonBuilder.Build(IronSulfideReaction.GetSpec())
            },
            new Reaction
            {
                Id = 2,
                TopicId = 2,
                Title = "Production of hydrogen gas",
                Equation = "Fe + 2HCl → FeCl₂ + H₂",
                MoleculeData = "[]",
                StepsData = StepJsonBuilder.Build(HydrogenGasReaction.GetSpec())
            },
            new Reaction
            {
                Id = 3,
                TopicId = 3,
                Title = "Glowing splint test",
                Equation = "2H₂O₂ → 2H₂O + O₂",
                MoleculeData = "[]",
                StepsData = StepJsonBuilder.Build(GlowingSplintReaction.GetSpec())
            },
            new Reaction
            {
                Id = 4,
                TopicId = 4,
                Title = "Sodium-water reaction",
                Equation = "2Na + 2H₂O → 2NaOH + H₂",
                MoleculeData = "[]",
                StepsData = StepJsonBuilder.Build(SodiumWaterReaction.GetSpec())
            }
        );
    }
}

/*  GRAVEYARD — old raw JSON, kept for reference only
    private static string _OldBuildIronSulfideSteps() => """
    [
      {
        "number": 1,
        "title": "Ambient Mixture",
        "description": "Iron (Fe) appears as grey magnetic particles arranged in a metallic lattice. Sulfur (S) forms S8 crown rings — 8 yellow atoms bonded in a circle. The two substances are physically separate; a magnet would still pull the grey particles away.",
        "temperatureRange": { "start": 20, "end": 115 },
        "background": "default",
        "atoms": [
          { "id": "fe1", "element": "Fe", "x": -3.2, "y":  0.0, "z": -0.7 },
          { "id": "fe2", "element": "Fe", "x": -1.8, "y":  0.0, "z": -0.7 },
          { "id": "fe3", "element": "Fe", "x": -3.2, "y":  0.0, "z":  0.7 },
          { "id": "fe4", "element": "Fe", "x": -1.8, "y":  0.0, "z":  0.7 },
          { "id": "s1",  "element": "S",  "x":  3.7,  "y":  0.45, "z":  0.0  },
          { "id": "s2",  "element": "S",  "x":  3.35, "y": -0.45, "z":  0.85 },
          { "id": "s3",  "element": "S",  "x":  2.5,  "y":  0.45, "z":  1.2  },
          { "id": "s4",  "element": "S",  "x":  1.65, "y": -0.45, "z":  0.85 },
          { "id": "s5",  "element": "S",  "x":  1.3,  "y":  0.45, "z":  0.0  },
          { "id": "s6",  "element": "S",  "x":  1.65, "y": -0.45, "z": -0.85 },
          { "id": "s7",  "element": "S",  "x":  2.5,  "y":  0.45, "z": -1.2  },
          { "id": "s8",  "element": "S",  "x":  3.35, "y": -0.45, "z": -0.85 }
        ],
        "bonds": [
          { "from": "s1", "to": "s2" }, { "from": "s2", "to": "s3" },
          { "from": "s3", "to": "s4" }, { "from": "s4", "to": "s5" },
          { "from": "s5", "to": "s6" }, { "from": "s6", "to": "s7" },
          { "from": "s7", "to": "s8" }, { "from": "s8", "to": "s1" }
        ],
        "electronTransfers": [],
        "atomOverrides": {}
      },
      {
        "number": 2,
        "title": "Activation and Liquefaction",
        "description": "At 119°C the S8 rings break apart. Sulfur becomes fluid, surrounding and coating the iron particles. The yellow crowns dissolve into individual atoms that flow around the stationary iron blocks — the wetting phase.",
        "temperatureRange": { "start": 119, "end": 445 },
        "background": "default",
        "atoms": [
          { "id": "fe1", "element": "Fe", "x": -0.7, "y":  0.0, "z": -0.7 },
          { "id": "fe2", "element": "Fe", "x":  0.7, "y":  0.0, "z": -0.7 },
          { "id": "fe3", "element": "Fe", "x": -0.7, "y":  0.0, "z":  0.7 },
          { "id": "fe4", "element": "Fe", "x":  0.7, "y":  0.0, "z":  0.7 },
          { "id": "s1",  "element": "S",  "x":  0.0, "y":  2.2, "z":  0.0 },
          { "id": "s2",  "element": "S",  "x":  2.2, "y":  0.0, "z":  0.0 },
          { "id": "s3",  "element": "S",  "x":  0.0, "y": -2.2, "z":  0.0 },
          { "id": "s4",  "element": "S",  "x": -2.2, "y":  0.0, "z":  0.0 },
          { "id": "s5",  "element": "S",  "x":  1.6, "y":  1.0, "z":  1.6 },
          { "id": "s6",  "element": "S",  "x": -1.6, "y":  1.0, "z": -1.6 },
          { "id": "s7",  "element": "S",  "x":  0.0, "y":  0.0, "z":  2.5 },
          { "id": "s8",  "element": "S",  "x":  0.0, "y":  0.0, "z": -2.5 }
        ],
        "bonds": [],
        "electronTransfers": [],
        "atomOverrides": {}
      },
      {
        "number": 3,
        "title": "Exothermic Fusion",
        "description": "The high-energy moment: electrons leap from each iron atom to a sulfur neighbor. Iron shrinks as it becomes Fe2+ and sulfur expands as it becomes S2-. The reaction releases 100 kJ/mol — a fierce orange-red incandescence fills the scene.",
        "temperatureRange": { "start": 600, "end": 900 },
        "background": "glow",
        "atoms": [
          { "id": "fe1", "element": "Fe", "x": -0.7, "y":  0.0, "z": -0.7 },
          { "id": "fe2", "element": "Fe", "x":  0.7, "y":  0.0, "z": -0.7 },
          { "id": "fe3", "element": "Fe", "x": -0.7, "y":  0.0, "z":  0.7 },
          { "id": "fe4", "element": "Fe", "x":  0.7, "y":  0.0, "z":  0.7 },
          { "id": "s1",  "element": "S",  "x":  0.0, "y":  2.0, "z":  0.0 },
          { "id": "s2",  "element": "S",  "x":  2.0, "y":  0.0, "z":  0.0 },
          { "id": "s3",  "element": "S",  "x":  0.0, "y": -2.0, "z":  0.0 },
          { "id": "s4",  "element": "S",  "x": -2.0, "y":  0.0, "z":  0.0 },
          { "id": "s5",  "element": "S",  "x":  1.4, "y":  0.9, "z":  1.4 },
          { "id": "s6",  "element": "S",  "x": -1.4, "y":  0.9, "z": -1.4 },
          { "id": "s7",  "element": "S",  "x":  0.0, "y":  0.0, "z":  2.2 },
          { "id": "s8",  "element": "S",  "x":  0.0, "y":  0.0, "z": -2.2 }
        ],
        "bonds": [],
        "electronTransfers": [
          { "from": "fe1", "to": "s4" },
          { "from": "fe2", "to": "s2" },
          { "from": "fe3", "to": "s7" },
          { "from": "fe4", "to": "s5" }
        ],
        "atomOverrides": {
          "fe1": { "radiusScale": 0.8, "dimmed": true },
          "fe2": { "radiusScale": 0.8, "dimmed": true },
          "fe3": { "radiusScale": 0.8, "dimmed": true },
          "fe4": { "radiusScale": 0.8, "dimmed": true },
          "s1":  { "radiusScale": 1.2, "glowing": true },
          "s2":  { "radiusScale": 1.2, "glowing": true },
          "s3":  { "radiusScale": 1.2, "glowing": true },
          "s4":  { "radiusScale": 1.2, "glowing": true },
          "s5":  { "radiusScale": 1.2, "glowing": true },
          "s6":  { "radiusScale": 1.2, "glowing": true },
          "s7":  { "radiusScale": 1.2, "glowing": true },
          "s8":  { "radiusScale": 1.2, "glowing": true }
        }
      },
      {
        "number": 4,
        "title": "Lattice Solidification",
        "description": "The chaos stops. Fe2+ and S2- ions snap into a rigid NiAs-type crystal lattice — each iron atom locked between sulfur neighbors above and below. The glow fades into a dull charcoal-black solid: non-magnetic, brittle, and complete.",
        "temperatureRange": { "start": 900, "end": 25 },
        "background": "dark",
        "atoms": [
          { "id": "fe1", "element": "Fe", "x": -1.0, "y":  0.0, "z": -1.0 },
          { "id": "fe2", "element": "Fe", "x":  1.0, "y":  0.0, "z": -1.0 },
          { "id": "fe3", "element": "Fe", "x": -1.0, "y":  0.0, "z":  1.0 },
          { "id": "fe4", "element": "Fe", "x":  1.0, "y":  0.0, "z":  1.0 },
          { "id": "s1",  "element": "S",  "x": -1.0, "y":  1.7, "z": -1.0 },
          { "id": "s2",  "element": "S",  "x":  1.0, "y":  1.7, "z": -1.0 },
          { "id": "s3",  "element": "S",  "x": -1.0, "y":  1.7, "z":  1.0 },
          { "id": "s4",  "element": "S",  "x":  1.0, "y":  1.7, "z":  1.0 },
          { "id": "s5",  "element": "S",  "x": -1.0, "y": -1.7, "z": -1.0 },
          { "id": "s6",  "element": "S",  "x":  1.0, "y": -1.7, "z": -1.0 },
          { "id": "s7",  "element": "S",  "x": -1.0, "y": -1.7, "z":  1.0 },
          { "id": "s8",  "element": "S",  "x":  1.0, "y": -1.7, "z":  1.0 }
        ],
        "bonds": [
          { "from": "fe1", "to": "s1" }, { "from": "fe1", "to": "s5" },
          { "from": "fe2", "to": "s2" }, { "from": "fe2", "to": "s6" },
          { "from": "fe3", "to": "s3" }, { "from": "fe3", "to": "s7" },
          { "from": "fe4", "to": "s4" }, { "from": "fe4", "to": "s8" }
        ],
        "electronTransfers": [],
        "atomOverrides": {
          "fe1": { "radiusScale": 0.8, "darkened": true },
          "fe2": { "radiusScale": 0.8, "darkened": true },
          "fe3": { "radiusScale": 0.8, "darkened": true },
          "fe4": { "radiusScale": 0.8, "darkened": true },
          "s1":  { "radiusScale": 1.2, "darkened": true },
          "s2":  { "radiusScale": 1.2, "darkened": true },
          "s3":  { "radiusScale": 1.2, "darkened": true },
          "s4":  { "radiusScale": 1.2, "darkened": true },
          "s5":  { "radiusScale": 1.2, "darkened": true },
          "s6":  { "radiusScale": 1.2, "darkened": true },
          "s7":  { "radiusScale": 1.2, "darkened": true },
          "s8":  { "radiusScale": 1.2, "darkened": true }
        }
      }
    ]
    """;
*/
