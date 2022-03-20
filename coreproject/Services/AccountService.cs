using System;
using System.Collections.Generic;
using System.Linq;
using coreproject.Model;
using Microsoft.EntityFrameworkCore;

namespace coreproject.Services
{
    public interface IAccountService{
        public List<AccountReturn> GetAccounts();
        // public Account GetAccount(int id);
        public Account GetAccountOnLogin(string username, string password);
        public Account GenerateAccessToken(string refreshToken);
        public void DeleteTokens(string refreshToken);
        public Account CreateAccount(AccountSignup accountSignup);
        public void DeleteAccount(int accountId);
    }
    public class AccountService : IAccountService
    {
        
        public List<AccountReturn> GetAccounts()
        {
            try
            {
                using(var db = new ApplicationContext())
                {
                    List<Account> accounts = db.Accounts.Where(d => d.Role != "Admin").ToList();

                    List<AccountReturn> acc = new List<AccountReturn>();

                    foreach(var ac in accounts)
                    {
                        acc.Add(new AccountReturn{ AccountId = ac.AccountId, FirstName = ac.FirstName, LastName = ac.LastName, UserName = ac.UserName });
                    }

                    return acc;

                }
            }
            catch
            {
                throw new Exception("Could not get accounts");
            }
            
        }

        // public Account GetAccount(int id)
        // {
        //     using(var db = new ApplicationContext())
        //     {
        //         return db.Accounts.Where(d => d.AccountId == id).FirstOrDefault();
        //     }
        // }

        public Account GetAccountOnLogin(string username, string password)
        {
            using(var db = new ApplicationContext())
            {
                try
                {
                    var account = db.Accounts.Where(d => d.UserName == username && d.Password == password).FirstOrDefault();
                    var rng = new Random();
                    account.AccessToken = account.AccountId.ToString() + "_" +   rng.Next(1,9999).ToString();
                    account.AccessTokenExpiryDate = DateTime.Now.AddSeconds(60);
                    account.RefreshToken = account.AccountId.ToString() + "_" +   rng.Next(9999,99999).ToString();
                    account.RefreshTokenExpiryDate = DateTime.UtcNow;
                    db.SaveChanges();
                    return account;
                }
                catch
                {
                    throw new Exception("There is no account with this existing username or password");
                }
                
            }
        }

        public Account GenerateAccessToken(string refreshToken)
        {
             using(var db = new ApplicationContext())
            {
                try
                {
                    var account = db.Accounts.Where(d => d.RefreshToken == refreshToken).FirstOrDefault();
                    var rng = new Random();
                    account.AccessToken = account.AccountId.ToString() + "_" +  rng.Next(1,9999).ToString();
                    account.AccessTokenExpiryDate = DateTime.Now.AddSeconds(60);
                    db.SaveChanges();
                    return account;
                }
                catch
                {
                    throw new Exception("There is no account with this refreshToken in database");
                }

                    
                
            }
        }


        public void DeleteTokens(string refreshToken)
        {
            
            using(var db = new ApplicationContext())
            {
                try
                {
                    var account = db.Accounts.Where(d => d.RefreshToken == refreshToken).FirstOrDefault();
                    account.AccessToken = string.Empty;
                    account.RefreshToken = string.Empty;
                    db.SaveChanges();
                }
                catch
                {
                    throw new Exception("There is no account with this refreshToken in database");
                }
                
            }
        }

        public Account CreateAccount(AccountSignup accountSignup)
        {
            
            using(var db = new ApplicationContext())
            {
                try
                {
                    var acc = db.Accounts.Where(d => d.UserName == accountSignup.UserName).FirstOrDefault();
                    if(acc != null)
                    {
                        throw new Exception("Username already exists");
                    }
                    string role = "User";

                    if(this.GetAccounts().Count() == 0)
                    {
                        role = "Admin"; // first user
                    }
                
                    Account account = new Account();
                    account.FirstName = accountSignup.FirstName;
                    account.LastName = accountSignup.LastName;
                    account.UserName = accountSignup.UserName;
                    account.Password = accountSignup.Password;
                    account.Role = role;
                    
                    db.Accounts.Add(account);
                    db.SaveChanges();

                    var rng = new Random();
                    account.AccessToken = account.AccountId.ToString() + "_" +  rng.Next(1,9999).ToString();
                    account.AccessTokenExpiryDate = DateTime.Now.AddSeconds(60);
                    account.RefreshToken = account.AccountId.ToString() + "_" +  rng.Next(9999,99999).ToString();
                    account.RefreshTokenExpiryDate = DateTime.UtcNow;
                    //db.Accounts.Add(account);
                    db.SaveChanges();
                    return account;
                }
                catch (Exception e)
                {
                    if(e.Message == "Username already exists"){
                        throw new Exception(e.Message);
                    }else{
                        throw new Exception("Could not create account");
                    }
                }
               
               }
        }

        public void DeleteAccount(int accountId)
        {
            try
            {
                using(var db = new ApplicationContext())
                {
                    Account acc = db.Accounts.Where(d => d.AccountId == accountId).FirstOrDefault();
                    db.Accounts.Remove(acc);
                    db.SaveChanges();
                }
            }
            catch
            {
                throw new Exception("Could not delete account");
            }
        }

    }
}