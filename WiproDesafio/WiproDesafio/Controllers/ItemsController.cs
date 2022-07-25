using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WiproDesafio.Data;
using WiproDesafio.Models;
using WiproDesafio.Models.Dto;

namespace WiproDesafio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : Controller {
        private readonly DBContext _context;

        public ItemsController(DBContext context) {
            _context = context;
        }

        [HttpGet("Item")]
        public Task<JsonResult> Get() {
            var item = _context.Items.OrderByDescending(a => a.Id).FirstOrDefault();

            if (item == null) {
                return Task.FromResult(new JsonResult(new { error = "Não foi encontrado nenhum registro." }));
            }

            _context.Items.Remove(item);
            _context.SaveChangesAsync();

            return Task.FromResult(new JsonResult(new ItemDto { moeda = item.moeda, data_inicio = item.data_inicio, data_fim = item.data_fim }));
        }

        
        [HttpPost("PostItem")]
        public async Task<JsonResult> Create([Bind("Id,moeda,data_inicio,data_fim")] Item[] itens) {
            foreach (var item in itens) { 
                var isValid = ValidarCamposObrigatorios(item);

                if (isValid) { 
                    _context.Add(item);
                } else { 
                    return await Task.FromResult(new JsonResult(new { Error = "Não foi possível cadastrar nenhum registro. Verifique os campos e tente novamente." }));
                }
                
            }

            await _context.SaveChangesAsync();

            return await Task.FromResult(new JsonResult(new { Sucess = "Os dados foram cadastrados com sucesso." }));
        }

        private bool ValidarCamposObrigatorios(Item item) {
            var moedaPreenchida = !isNullOrEmpty(item.moeda);

            return moedaPreenchida;

            bool isNullOrEmpty(string str) {
                return str == null || str == String.Empty;
            }
        }
    }
}
