namespace CodeFirst.Models;

public class PrescriptionRequestDto
{
    
    public int IdDoctor { get; set; }
    public PatientDto Patient { get; set; }
    public List<MedicamentDto> Medicaments { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }

}

public class PatientDto
{
    public int IdPatient { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
}



public class MedicamentDto
{
    public int IdMedicament { get; set; }
    public int Dose { get; set; }
    public string Details { get; set; }
}

public class PatientResponseDTO
{
    public int IdPatient { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public List<PrescriptionResponseDTO> Prescriptions { get; set; }
}


public class PrescriptionResponseDTO
{
    public int IdPrescription { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    
    public List<MedicamentResponseDTO> Medicaments { get; set; }
    
    public DoctorResponseDTO Doctor { get; set; }
}

public class MedicamentResponseDTO
{
    public int IdMedicament { get; set; }
    public String Name { get; set; }
    public int? Dose { get; set; }
    public String Description { get; set; }
}

public class DoctorResponseDTO
{
    public int IdDoctor { get; set; }
    public String FirstName { get; set; }
}