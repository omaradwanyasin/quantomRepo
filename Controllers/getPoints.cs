﻿using backend_api.Services;
using backend_api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetPointsController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;

        public GetPointsController(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        [HttpGet("getPoints")]
        public async Task<IActionResult> GetPoints([FromQuery] int areaId)
        {
            List<Node> nodes = await _mongoDbService.GetNodesByAreaAsync(areaId);

            // Convert to list of coordinate pairs
            var coordinates = nodes.Select(n => new List<double> { n.X, n.Y }).ToList();

            return Ok(coordinates);
        }
        [HttpGet("getPointsAll")]
        public async Task<IActionResult> CheckAreaStatuses()
        {
            await _mongoDbService.CheckAreaStatusesAsync();
            var pathfinder = new AStarPathfinder();
            var path = pathfinder.FindPath(MongoDbService.nodesMap, 1, 801);
            var coordinates = new List<double[]>();
            foreach (var item in path)
            {
                var x = MongoDbService.nodesMap[item].X;
                var y = MongoDbService.nodesMap[item].Y;
                coordinates.Add(new double[] { x, y });
            }
            return Ok(coordinates);
        }
        [HttpGet("signalr-test")]
        public IActionResult TestSignalRConnection()
        {
            return Ok("SignalR endpoint is active");
        }
    }
}
