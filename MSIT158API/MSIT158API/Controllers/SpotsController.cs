﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSIT158API.Models;
using MSIT158API.Models.DTO;

namespace MSIT158API.Controllers
{
    //https://localhost/api/spots
    [Route("api/[controller]")]
    [ApiController]
    public class SpotsController : ControllerBase
    {
        private readonly MyDBContext _context;
        public SpotsController(MyDBContext context)
        {
            _context = context;
        }
        //GET  https://localhost/api/spots
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpotImagesSpot>>> GetSpots()
        {
            return await _context.SpotImagesSpots.ToListAsync();
        }
        [HttpGet]
        [Route("search")]
        public async Task<ActionResult<IEnumerable<SpotImagesSpot>>> GetSpots1(string keyword)
        {
            return await _context.SpotImagesSpots.Where(s => s.SpotTitle.Contains(keyword)).ToListAsync();
        }

        [HttpGet]
        [Route("title")]
        public async Task<ActionResult<IEnumerable<String>>> GetSpotssTitle(string title)
        {
            var titles = _context.Spots.Where(s => s.SpotTitle.Contains(title)).Select(s => s.SpotTitle).Take(8);
            return await titles.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<SpotsPagingDTO>> GetSpotsBySearch(SearchDTO searchDTO)
        {
            //根據分類編號搜尋景點資料
            var spots = searchDTO.categoryId == 0 ? _context.SpotImagesSpots : _context.SpotImagesSpots.Where(s => s.CategoryId == searchDTO.categoryId);

            //根據關鍵字搜尋景點資料(title、desc)
            if (!string.IsNullOrEmpty(searchDTO.keyword))
            {
                spots = spots.Where(s => s.SpotTitle.Contains(searchDTO.keyword) || s.SpotDescription.Contains(searchDTO.keyword));
            }

            //排序
            switch (searchDTO.sortBy)
            {
                case "spotTitle":
                    spots = searchDTO.sortType == "asc" ? spots.OrderBy(s => s.SpotTitle) : spots.OrderByDescending(s => s.SpotTitle);
                    break;
                case "categoryId":
                    spots = searchDTO.sortType == "asc" ? spots.OrderBy(s => s.CategoryId) : spots.OrderByDescending(s => s.CategoryId);
                    break;
                default:
                    spots = searchDTO.sortType == "asc" ? spots.OrderBy(s => s.SpotId) : spots.OrderByDescending(s => s.SpotId);
                    break;
            }

            //總共有多少筆資料
            int totalCount = spots.Count();
            //每頁要顯示幾筆資料
            int pageSize = searchDTO.pageSize??9;   //searchDTO.pageSize ?? 9;
            //目前第幾頁
            int page = searchDTO.page??1;

            //計算總共有幾頁
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);

            //分頁
            spots = spots.Skip((page - 1) * pageSize).Take(pageSize);


            //包裝要傳給client端的資料
            SpotsPagingDTO spotsPaging = new SpotsPagingDTO();
            spotsPaging.TotalCount = totalCount;
            spotsPaging.TotalPages = totalPages;
            spotsPaging.SpotsResult = await spots.ToListAsync();


            return spotsPaging;
        }


    }
}