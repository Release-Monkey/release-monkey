﻿using Microsoft.AspNetCore.Mvc;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.Server.Services;

namespace ReleaseMonkey.Server.Controller
{

    public record UpdateReleaseTesterRequest
    (
        int Id,
        int State,
        string Comment
    );

    public record CreateReleaseTesterRequest
    (
        int ReleaseId,
        int TesterId
    );

    [ApiController]
    [Route("release-testers")]
    public class ReleaseTestersController(ReleaseTestersService releaseTesters) : ControllerBase
    {
        [HttpGet]
        public IActionResult Fetch()
        {
            Console.WriteLine(0);
            return Ok(releaseTesters.GetAllReleaseTesters());
        }

        [HttpGet("{id:int}", Name = "FetchReleaseTesterById")]
        public IActionResult FetchById(int id)
        {
            try
            {
                return Ok(releaseTesters.GetReleaseTesterById(id));
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet("release/{id:int}", Name = "FetchReleaseTesterByReleaseId")]
        public IActionResult FetchByProjectId(int id)
        {
            return Ok(releaseTesters.GetReleaseTestersByReleaseId(id));
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateReleaseTesterRequest body)
        {
            Console.WriteLine(body.Id);
            Console.WriteLine(0);
            var updatedReleaseTester = await releaseTesters.UpdateReleaseTester(body.Id, body.State, body.Comment);
            return CreatedAtRoute("FetchReleaseTesterById", new { updatedReleaseTester.Id }, updatedReleaseTester);
        }
    }
}
