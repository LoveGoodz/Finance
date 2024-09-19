﻿using Finance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StockTransController : ControllerBase
    {
        private readonly FinanceContext _context;

        public StockTransController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/StockTrans/{id} 
        [HttpGet("{id}")]
        public async Task<ActionResult<StockTrans>> GetStockTranById(int id)
        {
            var stockTran = await _context.StockTrans.FindAsync(id);

            if (stockTran == null)
            {
                return NotFound(new { Message = "Stok işlemi kaydı bulunamadı.", Status = 404 });
            }

            return Ok(new { Message = "Stok işlemi başarıyla getirildi.", Data = stockTran });
        }

        // PUT: api/StockTrans/5 
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStockTran(int id, StockTrans stockTran)
        {
            if (id != stockTran.ID)
            {
                return BadRequest(new { Message = "ID parametresi ile StockTrans.ID eşleşmiyor.", Status = 400 });
            }

            
            if (!StockTranExists(id))
            {
                return NotFound(new { Message = "Stok işlemi kaydı bulunamadı.", Status = 404 });
            }

            _context.Entry(stockTran).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Beklenmeyen bir hata oluştu.", Error = ex.Message });
            }

            return Ok(new { Message = "Stok işlemi başarıyla güncellendi." });
        }

        // POST: api/StockTrans 
        [HttpPost]
        public async Task<ActionResult<StockTrans>> PostStockTran(StockTrans stockTran)
        {
            _context.StockTrans.Add(stockTran);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStockTranById), new { id = stockTran.ID }, new { Message = "Stok işlemi başarıyla eklendi.", Data = stockTran });
        }

        // DELETE: api/StockTrans/5 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStockTran(int id)
        {
            var stockTran = await _context.StockTrans.FindAsync(id);
            if (stockTran == null)
            {
                return NotFound(new { Message = "Stok işlemi kaydı bulunamadı.", Status = 404 });
            }

            _context.StockTrans.Remove(stockTran);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Stok işlemi başarıyla silindi." });
        }

        // GET: api/StockTrans 
        [HttpGet]
        public async Task<ActionResult> GetStockTrans(string transactionType = null, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest(new { Message = "PageNumber ve PageSize sıfırdan büyük olmalıdır.", Status = 400 });
            }

            var query = _context.StockTrans.AsQueryable();

            
            if (!string.IsNullOrEmpty(transactionType))
            {
                query = query.Where(st => st.TransactionType.Contains(transactionType));
            }

            
            var totalRecords = await query.CountAsync();

            
            var stockTrans = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { TotalRecords = totalRecords, Data = stockTrans });
        }

        
        private bool StockTranExists(int id)
        {
            return _context.StockTrans.Any(e => e.ID == id);
        }
    }
}
