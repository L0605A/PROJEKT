using CodeFirst.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CodeFirst.Data
{
    public class ApplicationContext : DbContext
    {
        protected ApplicationContext()
        {
        }

        public ApplicationContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Software> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Medicament> Medicaments { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
        
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Software>().HasData(new List<Software>()
            {
                new Software
                {
                    IdDoctor = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com"
                },
                new Software
                {
                    IdDoctor = 2,			
                    FirstName = "Emily",
                    LastName = "Smith",
                    Email = "emily.smith@example.com"
                }
            });

            modelBuilder.Entity<Medicament>().HasData(new List<Medicament>()
            {
                new Medicament
                {
                    IdMedicament = 1,
                    Name = "Tylenol",
                    Description = "Pain Reliever",
                    Type = "Tablets"
                },
                new Medicament
                {
                    IdMedicament = 2,
                    Name = "Advil",
                    Description = "Anti-inflammatory",
                    Type = "Tablets"
                },
                new Medicament
                {
                    IdMedicament = 3,
                    Name = "Benadryl",
                    Description = "Antihistamine",
                    Type = "Tablets"
                },
                new Medicament
                {
                    IdMedicament = 4,
                    Name = "Zyrtec",
                    Description = "Allergy Relief",
                    Type = "Tablets"
                }
            });

            modelBuilder.Entity<Patient>().HasData(new List<Patient>()
            {
                new Patient
                {
                    IdPatient = 1,
                    FirstName = "Michael",
                    LastName = "Johnson",
                    Birthdate = new DateTime(1985, 2, 10)
                },
                new Patient
                {
                    IdPatient = 2,
                    FirstName = "Sarah",
                    LastName = "Brown",
                    Birthdate = new DateTime(1992, 6, 25)
                },
                new Patient
                {
                    IdPatient = 3,
                    FirstName = "David",
                    LastName = "Wilson",
                    Birthdate = new DateTime(1980, 9, 15)
                },
                new Patient
                {
                    IdPatient = 4,
                    FirstName = "Emma",
                    LastName = "Davis",
                    Birthdate = new DateTime(2001, 3, 30)
                }
            });

            modelBuilder.Entity<Prescription>().HasData(new List<Prescription>()
            {
                new Prescription
                {
                    IdPrescription = 1,
                    Date = new DateTime(2023, 1, 10),
                    DueDate = new DateTime(2023, 1, 24),
                    IdPatient = 1,
                    IdDoctor = 1
                },
                new Prescription
                {
                    IdPrescription = 2,
                    Date = new DateTime(2023, 2, 12),
                    DueDate = new DateTime(2023, 2, 26),
                    IdPatient = 2,
                    IdDoctor = 2
                },
                new Prescription
                {
                    IdPrescription = 3,
                    Date = new DateTime(2023, 3, 5),
                    DueDate = new DateTime(2023, 3, 19),
                    IdPatient = 3,
                    IdDoctor = 1
                },
                new Prescription
                {
                    IdPrescription = 4,
                    Date = new DateTime(2023, 4, 18),
                    DueDate = new DateTime(2023, 5, 2),
                    IdPatient = 4,
                    IdDoctor = 2
                }
            });

            modelBuilder.Entity<PrescriptionMedicament>().HasData(new List<PrescriptionMedicament>()
            {
                new PrescriptionMedicament
                {
                    IdMedicament = 1,
                    IdPrescription = 1,
                    Dose = 2,
                    Details = "Take before meals"
                },
                new PrescriptionMedicament
                {
                    IdMedicament = 2,
                    IdPrescription = 2,
                    Dose = 1,
                    Details = "Take after meals"
                },
                new PrescriptionMedicament
                {
                    IdMedicament = 3,
                    IdPrescription = 3,
                    Dose = 3,
                    Details = "Take at bedtime"
                },
                new PrescriptionMedicament
                {
                    IdMedicament = 4,
                    IdPrescription = 4,
                    Dose = 1,
                    Details = "Take before meals"
                }
            });
            
            modelBuilder.Entity<User>().HasData(new List<User>
            {
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    RefreshToken = "",
                    RefreshTokenExpiryTime = DateTime.MinValue
                }
            });
        }
    }
}
