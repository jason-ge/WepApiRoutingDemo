using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiRoutingDemo.API.Models;
using System.Collections.Generic;
using System.Linq;

namespace WebApiRoutingDemo.Controllers
{
    [EnableCors("SiteCorsPolicy")]
    [ApiController]
    [ApiVersion("1")]
    [Route("v{version:apiVersion}/usersettings")]
    public class UserSettingsController : ControllerBase
    {
        private List<UserSettingModel> settings;
        public UserSettingsController()
        {
            settings = new List<UserSettingModel>()
            {
                new UserSettingModel()
                {
                    UserId = "testuser1",
                    SettingKey = "setingkey1",
                    SettingValue = "settingvalue1",
                    UserSettingId = 1
                },
                 new UserSettingModel()
                {
                    UserId = "testuser1",
                    SettingKey = "setingkey2",
                    SettingValue = "settingvalue2",
                    UserSettingId = 2
                },
                new UserSettingModel()
                {
                    UserId = "testuser2",
                    SettingKey = "setingkey1",
                    SettingValue = "settingvalue1",
                    UserSettingId = 3
                },
           };
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddUserSetting([FromBody] UserSettingModel userSetting)
        {
            var setting = settings.FirstOrDefault(s => s.UserSettingId == userSetting.UserSettingId);
            if (setting != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "User setting already exists");
            }
            else
            {
                userSetting.UserId = HttpContext.User.Identity.Name;
                settings.Add(userSetting);
                return Ok(userSetting);
            }
        }

        [HttpPut]
        [Authorize]
        public IActionResult UpdateUserSetting([FromBody] UserSettingModel userSetting)
        {
            var setting = settings.FirstOrDefault(s => s.UserSettingId == userSetting.UserSettingId && s.UserId == HttpContext.User.Identity.Name);
            if (setting != null)
            {
                setting.SettingKey = userSetting.SettingKey;
                setting.SettingValue = userSetting.SettingValue;
                return Ok(userSetting);
            }
            else
            {
                return StatusCode(StatusCodes.Status404NotFound, "User setting does not exist.");
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public IActionResult DeleteUserSetting([FromRoute]int id)
        {
            int count = settings.RemoveAll(s => s.UserId == HttpContext.User.Identity.Name && s.UserSettingId == id);
            return Ok(count);
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetUserSettings()
        {
            return Ok(settings.Where(s => s.UserId == HttpContext.User.Identity.Name).ToList());
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public IActionResult GetUserSetting([FromRoute]int id)
        {
            return Ok(settings.FirstOrDefault(s => s.UserId == HttpContext.User.Identity.Name && s.UserSettingId == id));
        }
    }
}