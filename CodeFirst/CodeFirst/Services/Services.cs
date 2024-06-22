using System.Xml;
using CodeFirst.Data;
using CodeFirst.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeFirst.Services
{
    public class PrescriptionService : IServices
    {
        private readonly ApplicationContext _context;

        public PrescriptionService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<bool> DoctorExists(int id)
        {
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.IdDoctor ==id);

            return doctor != null;
        }
        
        
        public async Task<bool> CorrectMedicaments(PrescriptionRequestDto request)
        {
            var medicamentIds = request.Medicaments.Select(m => m.IdMedicament).ToList();
            var existingMedicaments = await _context.Medicaments
                .Where(m => medicamentIds.Contains(m.IdMedicament))
                .ToListAsync();

            return existingMedicaments.Count == medicamentIds.Count;
        }
        public async Task<String> AddPrescriptionAsync(PrescriptionRequestDto request)
        {


                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.IdDoctor == request.IdDoctor);

                Patient patient;

                if(await PatientExists(request.Patient.IdPatient))
                {
                    Console.WriteLine("FOUND PATIENT WITH ID: " + request.Patient.IdPatient);
                    patient = await _context.Patients
                        .FirstOrDefaultAsync(p => p.IdPatient == request.Patient.IdPatient);
                }
                else
                {
                    Console.WriteLine("MAKING A NEW PATIENT WITH ID: " + request.Patient.IdPatient);
                    patient = new Patient
                    {
                        IdPatient = request.Patient.IdPatient,
                        FirstName = request.Patient.FirstName,
                        LastName = request.Patient.LastName,
                        Birthdate = request.Patient.BirthDate
                    };
                }
                

                var prescription = new Prescription
                {
                    Date = request.Date,
                    DueDate = request.DueDate,
                    IdPatient = request.Patient.IdPatient,
                    IdDoctor = request.IdDoctor
                };
                
                _context.Prescriptions.Add(prescription);

                await _context.SaveChangesAsync();

                var prescriptionMedicaments = request.Medicaments.Select(m => new PrescriptionMedicament
                {
                    IdPrescription = prescription.IdPrescription,
                    IdMedicament = m.IdMedicament,
                    Dose = m.Dose,
                    Details = m.Details
                }).ToList();

                doctor.Prescriptions.Add(prescription);
                _context.Doctors.Update(doctor);

                _context.PrescriptionMedicaments.AddRange(prescriptionMedicaments);

                patient.Prescriptions.Add(prescription);

                
                
                if(await PatientExists(request.Patient.IdPatient))
                {
                    _context.Patients.Update(patient);
                    await _context.SaveChangesAsync();
                    
                    return "Prescription added successfully";
                }
                else
                {
                    _context.Patients.Add(patient);
                    await _context.SaveChangesAsync();
                    
                    Console.WriteLine(await PatientExists(request.Patient.IdPatient));
                    
                    return "Prescription and patient added successfully";
                }
            

        }


        public async Task<bool> PatientExists(int id)
        {
            var patient = await _context.Patients.Where(p => p.IdPatient == id).FirstOrDefaultAsync();

            return patient != null;
        }
        
        public async Task<PatientResponseDTO> GetPatientAsync(int id)
        {

                var patient = await _context.Patients.Where(p => p.IdPatient == id).FirstOrDefaultAsync();
                
                var patientPrescription = await _context.Prescriptions.Where(p => p.IdPatient == id).ToListAsync();

                List<PrescriptionResponseDTO> prescriptions = new List<PrescriptionResponseDTO>();

                foreach (var prescription in patientPrescription)
                {
                    var doctorId = await _context.Prescriptions
                        .Where(p => p.IdPrescription == prescription.IdPrescription).Select(d => d.IdDoctor)
                        .FirstOrDefaultAsync();
                    var doctor = await _context.Doctors.Where(d => d.IdDoctor == doctorId).FirstOrDefaultAsync();

                    var medicaments = await _context.PrescriptionMedicaments
                        .Where(p => p.IdPrescription == prescription.IdPrescription).ToListAsync();

                    List<MedicamentResponseDTO> medicamentResponseDtos = new List<MedicamentResponseDTO>();

                    foreach (var med in medicaments)
                    {
                        medicamentResponseDtos.Add(new MedicamentResponseDTO()
                        {
                            Description = await _context.Medicaments.Where(m => m.IdMedicament == med.IdMedicament)
                                .Select(m => m.Description).FirstOrDefaultAsync(),
                            Dose = med.Dose,
                            IdMedicament = med.IdMedicament,
                            Name = await _context.Medicaments.Where(m => m.IdMedicament == med.IdMedicament)
                                .Select(m => m.Name).FirstOrDefaultAsync()
                            
                        });
                    }
                    
                    prescriptions.Add(new PrescriptionResponseDTO()
                    {
                        IdPrescription = prescription.IdPrescription,
                        Date = prescription.Date,
                        DueDate = prescription.DueDate,
                        Medicaments = medicamentResponseDtos,
                        Doctor = new DoctorResponseDTO()
                        {
                            IdDoctor = doctor.IdDoctor,
                            FirstName = doctor.FirstName
                        }
                    });
                }
                
                
                var patientResponse = new PatientResponseDTO
                {
                    IdPatient = patient.IdPatient,
                    FirstName = patient.FirstName,
                    LastName = patient.LastName,
                    Prescriptions = prescriptions
                };

                return patientResponse;
        }

        
    }
}
