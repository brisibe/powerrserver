using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using powerr.Enums;
using powerr.Interfaces;
using powerr.Models.Dtos;
using powerr.Models.Entities.MeterToken;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace powerr.Controllers
{
    [Route("api/recharge")]
    [ApiController]
    public class RechargeTokenController : ControllerBase
    {
        private readonly IRechargeTokenRepository _rechargeTokenRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IMeterRepository _meterRepository;

        public RechargeTokenController(IRechargeTokenRepository rechargeTokenRepository, IWalletRepository walletRepository, IMeterRepository meterRepository)
        {
            _rechargeTokenRepository = rechargeTokenRepository;
            _walletRepository = walletRepository;
            _meterRepository = meterRepository;
        }

        [Route("generateToken")]
        [HttpPost]
        public async Task<IActionResult> GenerateRechargeToken([FromBody] GenerateRechargeTokenDto dto)
        {
           
            try
            {
                Random random = new Random();

                var wallet = await _walletRepository.FindByConditionAsync(m => m.Id == dto.WalletId);
                var unitForAmount = dto.Value == 200 ? 20 : dto.Value == 500 ? 50 : dto.Value == 100 ? 120 : 0;

                if (wallet != null)
                {
                    if(wallet.Balance < dto.Value)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, new { statusCode = 400, message = "Insufficient Balance" });
                    }

                    var rechargeObj = new RechargeToken
                    {
                        Token = random.Next(1000000, 8000000),
                        HasBeenUsed = false,
                        Value = unitForAmount
                    };

                     _rechargeTokenRepository.Create(rechargeObj);
                    _rechargeTokenRepository.Save();

                    wallet.Balance = wallet.Balance - dto.Value;

                    _walletRepository.Update(wallet);
                    _walletRepository.Save();
                  
                  return StatusCode(StatusCodes.Status200OK, new { statusCode = 200, data = rechargeObj });
                }

                return StatusCode(StatusCodes.Status400BadRequest, new { statusCode = 400, message ="wallet not found" });


            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [Route("rechargeMeter")]
        [HttpPost]
        public async Task<IActionResult> RechargeMeter([FromBody] RechargeMeterDto dto)
        {

            try
            {
                var doesTokenExist = await _rechargeTokenRepository.FindByConditionAsync(m => m.Token == dto.token);

                if(doesTokenExist == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { statusCode = 400, message = "Invalid token" });
                }
                else
                {
                    if(doesTokenExist.HasBeenUsed == true)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, new { statusCode = 400, message = "Token has been used" });

                    }
                    else
                    {
                        var meter = await _meterRepository.FindByConditionAsync(m => m.Id == dto.meterId);


                        if(meter != null)
                        {
                            meter.AvailableUnit = meter.AvailableUnit + doesTokenExist.Value;
                            meter.Status = ((int)MeterStatusEnum.CONNECTED);

                            doesTokenExist.HasBeenUsed = true;

                            _meterRepository.Update(meter);
                            _meterRepository.Save();

                            _rechargeTokenRepository.Update(doesTokenExist);
                            _rechargeTokenRepository.Save();





                            return StatusCode(StatusCodes.Status200OK, new { statusCode = 200, message = "" });

                        }

                        return StatusCode(StatusCodes.Status400BadRequest, new { statusCode = 400, message = "Token has been used" });

                    }
                }



              

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
