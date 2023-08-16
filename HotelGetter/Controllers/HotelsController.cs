using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelGetter;
using System.Net.Http;
using Newtonsoft.Json;

namespace HotelGetter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly hotelDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public HotelsController(hotelDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            this._httpClientFactory = httpClientFactory;
        }


        // GET: api/Hotels
        [HttpGet]
        public async Task<IActionResult> GetHotelsFromFirstApi()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync("https://localhost:7033/api/Hotels"); 
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var hotels = JsonConvert.DeserializeObject<List<Hotel>>(content);
                foreach(var hot in hotels)
                {
                    var existingHotel = await _context.Hotels
                .FirstOrDefaultAsync(h => h.Name == hot.Name && h.Address == hot.Address);
                    if (existingHotel == null)
                    {
                        var newHotel = new Hotel
                        {
                            Name = hot.Name,
                            Address = hot.Address,
                            Rating = hot.Rating,
                            CountryId = default
                        };
                        await _context.AddAsync(newHotel);
                    }
    
                }
                await _context.SaveChangesAsync();
                return Ok(hotels);
                

            }
            catch (DbUpdateException ex)
            {
                // Log or display the inner exception details
                var message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, message);
            }
            catch (Exception ex)
            {
                // Handle exception
                return StatusCode(500, ex.Message);
            }
        }

         // GET: api/Hotels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Hotel>> GetHotel(int id)
        {
          if (_context.Hotels == null)
          {
              return NotFound();
          }
            var hotel = await _context.Hotels.FindAsync(id);

            if (hotel == null)
            {
                return NotFound();
            }

            return hotel;
        }

        // PUT: api/Hotels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHotel(int id, Hotel hotel)
        {
            if (id != hotel.Id)
            {
                return BadRequest();
            }

            _context.Entry(hotel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HotelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Hotels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Hotel>> PostHotel(Hotel hotel)
        {
          if (_context.Hotels == null)
          {
              return Problem("Entity set 'hotelDbContext.Hotels'  is null.");
          }
            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
        }

        // DELETE: api/Hotels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            if (_context.Hotels == null)
            {
                return NotFound();
            }
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }

            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HotelExists(int id)
        {
            return (_context.Hotels?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
