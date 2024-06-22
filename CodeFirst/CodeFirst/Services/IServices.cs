using CodeFirst.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodeFirst.Services;

public interface IServices
{
    
    public Task<bool> DoctorExists(int id);
    
    public Task<bool> CorrectMedicaments(PrescriptionRequestDto request);
    
    
    public Task<String> AddPrescriptionAsync(PrescriptionRequestDto request);
    
    public Task<bool> PatientExists(int id);
    
    public Task<PatientResponseDTO> GetPatientAsync(int id);
}