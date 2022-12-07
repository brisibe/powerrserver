using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using powerr.Api.Models.Entities.User;
using powerr.Api.repository;
using powerr.Enums;
using powerr.Interfaces;
using powerr.Models;
using powerr.Models.Dtos;
using powerr.Models.Entities.Meter;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace powerr.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class MeterController : ControllerBase
    {

        //private readonly RepositoryContext _repository;

        private readonly UserManager<AppUser> _userManager;
        private readonly IMeterRequestRepository _meterReqRepository;
        private readonly IMeterRepository _meterRepository;


        public MeterController(IMeterRequestRepository meterRequestRepository, IMeterRepository meterRepository, UserManager<AppUser> userManager)
        {
            _meterReqRepository = meterRequestRepository;
            _userManager = userManager;
            _meterRepository = meterRepository;
        }

        [Route("createMeterRequest")]
        [HttpPost]
        public async Task<IActionResult> CreateMeterRequest([FromBody] MeterRequestDto meterRequestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var checkIfUserExists = await _userManager.FindByIdAsync(meterRequestDto.UserId.ToString());

                if(checkIfUserExists == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new HttpResponseModel
                    {
                        status = "error",
                        message = "User does not exist"
                    });
                }


                //create meter request for user



                //create a meterReq object;
                var MeterReqObj = new MeterRequest()
                {
                    RequestedAt = DateTime.Now,
                    FullName = $"{checkIfUserExists.FirstName} {checkIfUserExists.LastName}",
                    Address = meterRequestDto.Address,
                    LGA = meterRequestDto.LGA,
                    IsApproved = false,
                    UserId = meterRequestDto.UserId,
                    Status = ((int)MeterRequestEnum.New)
                };

                 _meterReqRepository.Create(MeterReqObj);
                _meterReqRepository.Save();



                return StatusCode(StatusCodes.Status201Created, MeterReqObj);

            }
            catch (Exception error)
            {

                return StatusCode(StatusCodes.Status400BadRequest, new { status = "something went wrong", message = error.Message });
            }


        }

        [Route("getMeterRequestByUserId")]
        [HttpGet]
        public async Task<IActionResult> GetMeterRequestByUserId([FromQuery] Guid userId)
        {
            try
            {

                //check if the userId is a valid one
                var checkIfUserExists = await _userManager.FindByIdAsync(userId.ToString());

                if(checkIfUserExists != null)
                {
                    //get meter mapped to user
                    var meter = await _meterReqRepository.FindByConditionAsync(m => m.UserId == userId);

                    if(meter != null)
                    {

                       return StatusCode(StatusCodes.Status200OK, new { statusCode= 200, message="success", data = meter });
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status200OK, new {statusCode = 204, message = "no request found"});

                    }


                }

                return StatusCode(StatusCodes.Status400BadRequest, new { status = "error", message = "user not found" });

            }
            catch (Exception err)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new {status = "error", message = err.Message});
                
            }

        }

        [HttpGet]
        [Route("getAllMeterRequests")]
        public async Task<IActionResult> GetAllMeterRequests()
        {
            try
            {
                var meters = await _meterReqRepository.GetAll();


                return Ok(new {statusCode = 200, message = "success", data = meters});

            }
            catch (Exception err)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        [Route("activateMeterRequest")]
        [HttpPost]
        public async Task<IActionResult> ActivateMeterRequest([FromBody] MeterRequestActivationModel activationModel )
        {
            try
            {
                //get meterRequest with id
                var meterRequest = await _meterReqRepository.FindByConditionAsync(m => m.MeterRequestId == activationModel.MeterRequestId);

                //retrieves meter by id
                var meterToAssign = await _meterRepository.FindByConditionAsync(m => m.Id == activationModel.MeterId);
                
                //if no meter with that Id then return
                if (meterToAssign == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { statusCode = 400, message = $"meter {activationModel.MeterId} does not exist" });
                }

                //if meter is available, check if it has already been activated
                if(meterRequest != null)
                {
                   // if meter is already approved by admin
                    if (meterRequest.IsApproved == true)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, new { statusCode = 400, message = "Meter request has already been approved" });
                    }
                    else
                    {
                        meterRequest.IsApproved = true;
                        meterRequest.Status = ((int)MeterRequestEnum.Approved);

                        meterToAssign.Status = ((int)MeterStatusEnum.DISCONNECTED);
                        meterToAssign.IsAvailable = false;

                        //map meter to request
                        meterRequest.Meters.Add(meterToAssign);


                        _meterReqRepository.Update(meterRequest);
                        _meterReqRepository.Save();
                        _meterRepository.Save();


                    }



                    return StatusCode(StatusCodes.Status201Created, new {StatusCode = 200, message = "Meter Request Approved", data = meterRequest});
                    //}
                }

                return StatusCode(StatusCodes.Status400BadRequest, new { statusCode = 400, message = $"meter request with id: {activationModel.MeterRequestId} does not exist" });

            }
            catch ( Exception err)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        [Route("rejectMeterRequest")]
        [HttpGet]
        public async Task<IActionResult> RejectMeterRequest([FromQuery] int requestId)
        {
            try
            {
                //get meterRequest with id
                var meterRequest = await _meterReqRepository.FindByConditionAsync(m => m.MeterRequestId == requestId);



                //if meter is available, check if it has already been activated
                if (meterRequest != null)
                {
                  
                    
                        meterRequest.IsApproved = false;
                        meterRequest.Status = ((int)MeterRequestEnum.Rejected);
                        

                    //if by anycase a meter has already been assigned to user then clear all.
                       if(meterRequest.Meters.Count > 0)
                    {
                        foreach (var meter in meterRequest.Meters)
                        {
                            meter.IsAvailable = true;
                            meter.AvailableUnit = 0;
                            meter.Status = 1;

                        }
                        meterRequest.Meters.Clear();
                    }

                        _meterReqRepository.Update(meterRequest);
                        _meterReqRepository.Save();

                        return StatusCode(StatusCodes.Status201Created, new { StatusCode = 200, message = "Meter Request Rejected" });

                }

                return StatusCode(StatusCodes.Status400BadRequest, new { statusCode = 400, message = $"meter request with id: {requestId} does not exist" });

            }
            catch (Exception err)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        [HttpPost]
        [Route("createMeter")]
        public  IActionResult CreateMeter([FromBody] MeterDto meterDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var newMeter = new Meter()
                {
                    AvailableUnit = 0,
                    DiscoName = meterDto.DiscoName,
                    Created = DateTime.Now,
                    IsAvailable = true,
                    MeterNumber = new Random().Next(100000, 999999),
                    Status = ((int)MeterStatusEnum.DISCONNECTED),

                };

              _meterRepository.Create(newMeter);
                _meterRepository.Save();
              
                return StatusCode(StatusCodes.Status201Created, newMeter);

            }
            catch (Exception err)
            {

          return StatusCode(StatusCodes.Status500InternalServerError, err.Message);
            }
        }

        [Route("getMeterById")]
        [HttpGet]
        public async Task<IActionResult> GetMeterById([FromQuery] int meterId)
        {
            try
            {

              
                    //get meter mapped to user
                    var meter = await _meterRepository.FindByConditionAsync(m => m.Id == meterId);

                    if (meter != null)
                    {

                        return StatusCode(StatusCodes.Status200OK, new { statusCode = 200, message = "success", data = meter });
                    }
                    

                return StatusCode(StatusCodes.Status400BadRequest, new { status = "error", message = "meter not found" });

            }
            catch (Exception err)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "error", message = err.Message });

            }

        }

        [Route("getAllMeters")]
        [HttpGet]
        public async Task<IActionResult> GetAllMeters()
        {
            try
            {


                //get meter mapped to user
                var meters = await _meterRepository.GetAll();

                if (meters != null)
                {

                    return StatusCode(StatusCodes.Status200OK, new { statusCode = 200, message = "success", data = meters });
                }


                return StatusCode(StatusCodes.Status400BadRequest, new { status = "error", message = "meter not found" });

            }
            catch (Exception err)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "error", message = err.Message });

            }

        }

        [Route("load")]
        public async Task<IActionResult> Load(LoadDto load)
        {
            

            var meter = await _meterRepository.FindByConditionAsync(m => m.Id == load.meterId);

            if(meter != null)
            {
                if(meter.AvailableUnit <= load.unit )
                {
                    meter.AvailableUnit = 0;
                    meter.Status = ((int)MeterStatusEnum.DISCONNECTED);

                    _meterRepository.Update(meter);
                    _meterRepository.Save();

                    return StatusCode(StatusCodes.Status201Created, new { statusCode = 201, message = "You have run out of units." });
                }


                meter.AvailableUnit = meter.AvailableUnit - load.unit;

                _meterRepository.Update(meter);
                _meterRepository.Save();

                return Ok();

            }
            return BadRequest(new { status = 400, message = "something went wrong" }) ;




        }
       
    }
}
