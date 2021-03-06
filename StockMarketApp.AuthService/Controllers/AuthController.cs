﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StockMarketApp.AuthService.Repositories;
using StockMarketLibrary;



// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StockMarketApp.AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    

    public class AuthController : ControllerBase
    {
        private IRepository<User> repository;

        public AuthController(IRepository<User> repository)
        {
            this.repository = repository;
        }
        
        [HttpPost("login")]
        public IActionResult Post(LoginData item)
        {
            //SSV
            if (!(string.IsNullOrEmpty(item.userName) && string.IsNullOrEmpty(item.password)))
            {
                try
                {
                    var result = repository.Login(item.userName,item.password);
                    return Ok(result.Item2);
                }
                catch (Exception ex) // internal server error
                {
                    return StatusCode(500, "internal server error");
                }
            }
            return BadRequest("Please pass both username and password");
        }

        // GET api/<AuthController>/5
        [HttpGet("logout")]
        public IActionResult Get()
        {
            repository.Logout();
            return Ok("logged out");
        }

        // POST api/<AuthController>
        // register new user / signup
        [HttpPost]
        public IActionResult Post([FromForm] User user)
        {
            //SSV
            if (ModelState.IsValid)
            {
                //pass to repository
                var isSuccess = repository.Signup(user);
                if (isSuccess)
                {
                    return Ok("User registered successfully");
                }
                return StatusCode(500, "Internal server error");
            }
            return BadRequest(ModelState);
        }

        [HttpGet("profile")]
        [Authorize]
        public IActionResult GetProfile()
        {
            var currentUser = HttpContext.User;
            var result = repository.GetProfile(currentUser);
            return Ok(result);
        }
    }
}
