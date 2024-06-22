using CodeFirst.Models;
using CodeFirst.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CodeFirst.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionsController : ControllerBase
    {
        private readonly IServices _services;
        private readonly IAuthService _authService;

        public PrescriptionsController(IServices services, IAuthService authService)
        {
            _services = services;
            _authService = authService;
        }
        

        [HttpPost]
        public async Task<IActionResult> AddPrescription(PrescriptionRequestDto request)
        {
            if (request.DueDate < request.Date)
            {
                return BadRequest("DueDate must be greater than or equal to Date");
            }
            
            if (request.Medicaments.Count > 10)
            {
                return BadRequest("A prescription can include a maximum of 10 medicaments");
            }

            var doctorExistance = await _services.DoctorExists(request.IdDoctor);
            
            if (!doctorExistance)
            {
                return NotFound("Doctor not found");
            }
            
            var MedicamentCorrect = await _services.CorrectMedicaments(request);
            
            if (!MedicamentCorrect)
            {
                return BadRequest("One or more incorrect medicaments");
            }


            var result = await _services.AddPrescriptionAsync(request);

            Console.WriteLine(await _services.PatientExists(request.Patient.IdPatient));
            
            return Ok(result);
            
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatient(int id)
        {
            var patientExists = await _services.PatientExists(id);

            if (!patientExists)
            {
                return NotFound("Patient not found");
            }
            
            
            var result = await _services.GetPatientAsync(id);


            return Ok(result);
            

        }
        
        
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto request)
        {
            var result = await _authService.RegisterAsync(request);
            if (!result.Success)
            {
                return BadRequest(result.Errors);
            }
            return Ok(result);
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDto request)
        {
            var result = await _authService.LoginAsync(request);
            if (!result.Success)
            {
                return BadRequest(result.Errors);
            }
            return Ok(new { AccessToken = result.AccessToken, RefreshToken = result.RefreshToken });
        }
        
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(string refreshToken)
        {
            var result = await _authService.RefreshTokenAsync(refreshToken);
            if (!result.Success)
            {
                return BadRequest(result.Errors);
            }
            return Ok(new { AccessToken = result.AccessToken });
        }

    }
}