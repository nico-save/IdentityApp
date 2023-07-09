using Api.DTOs.Admin;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AdminController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpGet("get-members")]
        public async Task<ActionResult<IEnumerable<MemberViewDto>>> GetMembers()
        {
            var members = await this.userManager.Users
                .Where(x => x.UserName != SD.AdminUserName)
                // this is a projection
                .Select(member => new MemberViewDto
                {
                    Id = member.Id,
                    Username = member.UserName,
                    Firstname = member.FirstName,
                    Lastname = member.LastName,
                    DateCreated = member.DateCreated,
                    IsLocked = this.userManager.IsLockedOutAsync(member).GetAwaiter().GetResult(),
                    Roles = this.userManager.GetRolesAsync(member).GetAwaiter().GetResult()
                }).ToListAsync();

            return Ok(members);
        }

        [HttpPut("lock-member/{id}")]
        public async Task<IActionResult>LockMember(string id)
        {
            var user = await this.userManager.FindByIdAsync(id);
            if (user == null)  return NotFound(); 
            if(IsAdminUserId(id)) return BadRequest(SD.SuperAdminChangeNotAllowed);

            await this.userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(5));
            return NoContent();

        }
        [HttpPut("unlock-member/{id}")]
        public async Task<IActionResult> UnlockMember(string id)
        {
            var user = await this.userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            if (IsAdminUserId(id)) return BadRequest(SD.SuperAdminChangeNotAllowed);

            await this.userManager.SetLockoutEndDateAsync(user, null);
            return NoContent();

        }

        [HttpDelete("delete-member/{id}")]
        public async Task<IActionResult> DeleteMember(string id)
        {
            var user = await this.userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            if (IsAdminUserId(id)) return BadRequest(SD.SuperAdminChangeNotAllowed);

            await this.userManager.DeleteAsync(user);
            return NoContent();

        }

        [HttpGet("get-application-roles")]
        public async Task<ActionResult<string[]>> GetApplicationRoles(string id)
        {
          
            return Ok(await this.roleManager.Roles.Select(x => x.Name).ToListAsync());

        }

        [HttpGet("get-member/{id}")]
        public async Task<ActionResult<MemberAddEditDto>> GetMember(string id)
        {
            var member = await this.userManager.Users.
                Where(x => x.UserName != SD.AdminUserName && x.Id == id)
                .Select(m => new MemberAddEditDto
                {
                    Id= m.Id,
                UserName = m.UserName,
                FirstName = m.FirstName,
                LastName = m.LastName,
                Roles = string.Join(",", this.userManager.GetRolesAsync(m).GetAwaiter().GetResult())
                }).FirstOrDefaultAsync();
            return Ok(member);

        }

        [HttpPost("add-edit-member")]
        public async Task<IActionResult> AddEditMember(MemberAddEditDto model)
        {
            User user;
            if (string.IsNullOrEmpty(model.Id))
            {
                // adding a new membmer
                if(string.IsNullOrEmpty(model.Password) || model.Password.Length < 6)
                {
                    ModelState.AddModelError("errors", "Password must be at least 6 characters");
                    return  BadRequest(ModelState);

                }

                user = new User
                {
                    FirstName = model.FirstName.ToLower(),
                    LastName = model.LastName.ToLower(),
                    UserName = model.UserName.ToLower(),
                    EmailConfirmed = true
                };
                var result = await this.userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded) return BadRequest(result.Errors);
                
            }
            else
            {
                // editing an existing member

                if (!string.IsNullOrEmpty(model.Password))
                {
                    if(model.Password.Length < 6)
                    {
                        ModelState.AddModelError("errors", "Password must be at least 6 characters");
                        return BadRequest(ModelState);
                    }

                }
                if (IsAdminUserId(model.Id)) return BadRequest(SD.SuperAdminChangeNotAllowed);
                user = await this.userManager.FindByIdAsync(model.Id);
                if (user == null) return NotFound();

                user.FirstName = model.FirstName.ToLower();
                user.LastName = model.LastName.ToLower();
                user.UserName = model.UserName.ToLower();

                if(!string.IsNullOrEmpty(model.Password))
                {
                    await this.userManager.RemovePasswordAsync(user);
                    await this.userManager.AddPasswordAsync(user, model.Password);
                }

            }
            var userRoles = await this.userManager.GetRolesAsync(user);

            // removing users' existing role(s)
            await this.userManager.RemoveFromRolesAsync(user, userRoles);
            foreach(var role in model.Roles.Split(',').ToArray())
            {
                var roleToAdd = await this.roleManager.Roles.FirstOrDefaultAsync(r => r.Name == role);
                if(roleToAdd != null)
                {
                    await this.userManager.AddToRoleAsync(user, role);
                }
            }

            if (string.IsNullOrEmpty(model.Id))
            {
                return Ok(new JsonResult(new { title = "Member Created", message = $"{model.UserName} has been created" }));
            } else
            {
                return Ok(new JsonResult(new { title = "Member Edited", message = $"{model.UserName} has been edited" }));
            }
        }





        private bool IsAdminUserId(string userId)
        {
            return this.userManager.FindByIdAsync(userId).GetAwaiter().GetResult().UserName.Equals(SD.AdminUserName);
        }

    }
}
