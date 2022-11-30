
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using powerr.Interfaces;
using powerr.Models;
using powerr.Models.Dtos;

namespace powerr.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletRepository _walletRepository;

        public WalletController(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }


        [Route("getWalletByUserId")]
        [HttpGet]
        public async Task<IActionResult> GetWalletByUserId([FromQuery] string userId)
        {
            try
            {
                var wallet = await _walletRepository.FindByConditionAsync(m => m.UserId == userId);

                if(wallet != null)
                {
                    return StatusCode(StatusCodes.Status200OK, new {statusCode = 200, message = "success", data = wallet});
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new { statusCode = 204, message = "wallet not found",  });


            }
            catch (Exception err)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { statusCode = 500, message = err.Message });

            }
        }

        [Route("fundWallet")]
        [HttpPost]
        public async Task<IActionResult> FundWallet([FromBody] FundWalletDto fundDto)
        {
            try
            {
                
                var wallet = await _walletRepository.FindByConditionAsync(m => m.Id == fundDto.WalletId);

                if(wallet != null)
                {
                    wallet.Balance = wallet.Balance + fundDto.FundAmount;

                    _walletRepository.Update(wallet);
                    _walletRepository.Save();


                    return StatusCode(StatusCodes.Status200OK, new { statusCode = 200, message = "wallet funded" });

                }

                return StatusCode(StatusCodes.Status204NoContent, new { statusCode = 204, message = "wallet not found" });

            }
            catch (Exception err)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { statusCode = 500, message = err.Message});

            }

        }
    }
}
