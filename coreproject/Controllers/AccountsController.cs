using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using coreproject.Attributes;
using coreproject.Model;
using coreproject.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace coreproject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountsController: ControllerBase
    {        
        private readonly IAccountService _accountService;
        private readonly IEncryptonService _encryptionService;

         public AccountsController(IAccountService accountService, IEncryptonService encryptionService)
        {
            _accountService = accountService;
            _encryptionService = encryptionService;
        }

        [HttpGet]
        [Authorize("Admin")]
        public IEnumerable<AccountReturn> Get()
        {
            var accounts = _accountService.GetAccounts();
            return accounts;
            
        }

        [HttpPost]
        [Route("signup")]
        public IActionResult CreateAccount(AccountSignup accountSignup)
        {
            Account account = _accountService.CreateAccount(accountSignup);
            var al = new AccountLogin();
            al.AccountId = account.AccountId;
            al.AccessToken = _encryptionService.EncryptToken(account.AccessToken);
            al.AccessTokenExpiryDate = account.AccessTokenExpiryDate;
            al.Role = account.Role;
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddMinutes(20),
            };

            Response.Cookies.Append("refreshTokenCookie", account.RefreshToken, cookieOptions);       
            return Ok(al);
        }

        // [HttpGet]
        // [Route("GetAccount/{id}")]
        // [Authorize("Admin")]
        // public IActionResult GetAccount(int id)
        // {
        //     var account = _accountService.GetAccount(id);
        //     return Ok(account);
        // }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(AccountLoginRequest accRequest)
        {
            try
            {
                var account = _accountService.GetAccountOnLogin(accRequest.UserName, accRequest.Password);
                var al = new AccountLogin();
                al.AccountId = account.AccountId;
                al.AccessToken = _encryptionService.EncryptToken(account.AccessToken);
                al.AccessTokenExpiryDate = account.AccessTokenExpiryDate;
                al.Role = account.Role;
                var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddMinutes(20),
            };

            Response.Cookies.Append("refreshTokenCookie", account.RefreshToken, cookieOptions);

                return Ok(al);
                
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
            
        }

        [HttpPost]
        [Route("refresh-token")]
        public IActionResult RefreshToken()
        {
            try
            {
                var token = Request.Cookies["refreshTokenCookie"];
                if(token == null){
                    return BadRequest("Refresh token expired");
                }
                
                var account = _accountService.GenerateAccessToken(token);
                var al = new AccountLogin();
                al.AccountId = account.AccountId;
                al.AccessToken = _encryptionService.EncryptToken(account.AccessToken);
                al.AccessTokenExpiryDate = account.AccessTokenExpiryDate;
                al.Role = account.Role;

                return Ok(al);
                
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
            
        }

        [HttpPost]
        [Route("logout")]
        public IActionResult Logout()
        {
            try
            {
                var token = Request.Cookies["refreshTokenCookie"];
                if(token != null){  // expired do nothing
                 _accountService.DeleteTokens(token);
                }
                return Ok();
                
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
            
        }

        [HttpDelete]
        [Authorize("Admin")]
        [Route("account-delete/{accountId}")]
        public ActionResult DeleteAccount(int accountId)
        {
            try
            {
                var MainFolderName = Path.Combine("Resources", "Images");
                var MainFolderPath = Path.Combine(Directory.GetCurrentDirectory(), MainFolderName);
                // check if Account folder exists
                var AccountFolderPath = Path.Combine(MainFolderPath, accountId.ToString());
                if (Directory.Exists(AccountFolderPath))  
                {
                        System.GC.Collect(); 
                        System.GC.WaitForPendingFinalizers(); 
                        Directory.Delete(AccountFolderPath, true);
                }      

                _accountService.DeleteAccount(accountId);
                
                return Ok();
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}