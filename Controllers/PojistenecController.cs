using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PojistovnaApp.Data;
using PojistovnaApp.Models;

namespace PojistovnaApp.Controllers
{
    public class PojistenecController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PojistenecController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int cisloStrany = 1)
        {
            // Získání seznamu pojištenců a stránkování
            int zaznamuNaStranu = 5;
            var pojistenci = await _context.Pojistenci
                .OrderBy(p => p.Id)
                .Skip((cisloStrany - 1) * zaznamuNaStranu)
                .Take(zaznamuNaStranu)
                .ToListAsync();

            // Vypočítání celkového počtu stránek
            int totalCount = await _context.Pojistenci.CountAsync();
            int pocetStran = (int)Math.Ceiling((double)totalCount / zaznamuNaStranu);

            // Předání dat do view
            ViewData["Pojistenci"] = pojistenci;
            ViewData["CisloStrany"] = cisloStrany;
            ViewData["PocetStran"] = pocetStran;

            // Zobrazení seznamu pojištěnců
            return View(pojistenci);
        }

        // GET: Pojistenec/Details/5
        // Metoda pro zobrazení detailu pojistence a jeho sjednaných pojištění
        public async Task<IActionResult> Details(int? id, int cisloStrany = 1)
        {
            // Pokud není předáno ID nebo není načtena databáze pojištěnců, vrátí se chybová stránka
            if (id == null || _context.Pojistenci == null)
            {
                return NotFound();
            }

            var pojistenec = await _context.Pojistenci.FirstOrDefaultAsync(m => m.Id == id);
            // Pokud se nepodařilo najít konkrétního pojistence, vrátí se chybová stránka
            if (pojistenec == null)
            {
                return NotFound();
            }

            // Získání seznamu pojištění konkrétního pojistence a stránkování
            int zaznamuNaStranu = 3;
            var pojisteni = _context.Pojisteni
                .Where(p => p.PojistnikId == id)
                .Skip((cisloStrany - 1) * zaznamuNaStranu)
                .Take(zaznamuNaStranu)
                .ToList();

            // Vypočítání celkového počtu stránek
            int zaznamuCelkem = _context.Pojisteni.Count(p => p.PojistnikId == id);
            int pocetStran = (int)Math.Ceiling((double)zaznamuCelkem / zaznamuNaStranu);

            // Předání dat do view
            ViewData["Pojisteni"] = pojisteni;
            ViewData["CisloStrany"] = cisloStrany;
            ViewData["PocetStran"] = pocetStran;

            // Zobrazení detailu pojistence a jeho sjednaných pojištění
            return View(pojistenec);
        }

        // GET: Pojistenec/Create
        // Metoda pro zobrazení formuláře pro vytvoření pojištěnce
        public IActionResult Create()
        {
            return View(); // vrátí pohled Create.cshtml
        }

        // POST: Pojistenec/Create
        // Metoda pro zpracování formuláře pro vytvoření pojištěnce
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Jmeno,Prijmeni,Email,TelefonniCislo,UliceCisloPopisne,Mesto,PSC")] Pojistenec pojistenec)
        {
            if (ModelState.IsValid) // Zkontrolujeme, zda je stav modelu platný
            {
                _context.Add(pojistenec); // Přidáme pojistence do kontextu
                await _context.SaveChangesAsync(); // Provedeme změny v databázi
                TempData["AlertClass"] = "alert-success"; // nastavení barvy alertu
                TempData["AlertMessage"] = "Pojištěnec byl úspěšně uložen."; // nastavení zprávy alertu
                return RedirectToAction(nameof(Index)); // Přesměrujeme na Index stránku
            }
            return View(pojistenec); // Pokud model není platný, zobrazíme zpět formulář Create s aktuálními vstupy
        }

        // GET: Pojistenec/Edit/5
        // Metoda pro zobrazení formuláře pro editaci konkrétního pojištěnce s daným ID
        public async Task<IActionResult> Edit(int? id)
        {
            // Pokud ID neexistuje nebo databáze není inicializována, vrátí se chybová stránka
            if (id == null || _context.Pojistenci == null)
            {
                return NotFound();
            }
            // Pokud se konkrétní pojištěnec s daným ID nenajde, vrátí se chybová stránka
            var pojistenec = await _context.Pojistenci.FindAsync(id);
            if (pojistenec == null)
            {
                return NotFound();
            }
            return View(pojistenec); // Jinak se vrátí zobrazení formuláře pro editaci konkrétního pojištěnce
        }

        // POST: Pojistenec/Edit/5
        // Metoda pro zpracování formuláře pro editaci konkrétního pojištěnce s daným ID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Jmeno,Prijmeni,Email,TelefonniCislo,UliceCisloPopisne,Mesto,PSC")] Pojistenec pojistenec)
        {
            if (id != pojistenec.Id) // zkontrolujeme, zda-li existuje pojištěnec s daným id
            {
                return NotFound();
            }

            if (ModelState.IsValid) // Zkontrolujeme, zda je stav modelu platný
            {
                try
                {
                    _context.Update(pojistenec); // aktualizujeme pojištěnce
                    await _context.SaveChangesAsync(); // Uložení změn v databázi
                    TempData["AlertClass"] = "alert-warning"; // nastavení barvy alertu
                    TempData["AlertMessage"] = "Změny byly uloženy."; // nastavení zprávy alertu
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PojistenecExists(pojistenec.Id)) // pokud došlo k chybě, zkontrolujeme, zda-li pojištěnec stále existuje
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)); // pokud všechno proběhlo v pořádku, přesměrujeme uživatele na Index
            }
            return View(pojistenec); // Jinak se vrátí zobrazení formuláře pro editaci konkrétního pojištěnce
        }

        // GET: Pojistenec/Delete/5
        // Metoda pro zpracování formuláře pro smazání konkrétního pojištěnce s daným ID
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Pojistenci == null) // Pokud ID neexistuje nebo databáze není inicializována, vrátí se chybová stránka
            {
                return NotFound();
            }

            var pojistenec = await _context.Pojistenci // Hledá se Pojistenec z databáze s konkrétním ID
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pojistenec == null) // Pokud se Pojistenec v databázi nenajde, vrátí se chybová stránka
            {
                return NotFound();
            }

            return View(pojistenec); // Jinak se vrátí View s Pojistencem k smazání
        }

        // POST: Pojistenec/Delete/5
        // Metoda pro zpracovaní potvrzeného formuláře pro smazání
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pojistenci == null) // Pokud není databáze inicializována, vrátí se chybová stránka
            {
                return Problem("Entity set 'ApplicationDbContext.Pojistenci'  is null.");
            }
            var pojistenec = await _context.Pojistenci.FindAsync(id); // Vyhledání záznamu v databázi
            if (pojistenec != null) // Pokud byl záznam nalezen, tak se z databáze odstraní
            {
                _context.Pojistenci.Remove(pojistenec);
            }
            
            await _context.SaveChangesAsync(); // Uložení změn v databázi
            TempData["AlertClass"] = "alert-danger"; // nastavení barvy alertu
            TempData["AlertMessage"] = "Pojištěnec byl odstraněn."; // nastavení zprávy alertu
            return RedirectToAction(nameof(Index)); // Přesměrování na Index akci
        }

        // Metoda kontrolující, zda záznam v databázi existuje
        // Pokud je seznam Pojistenci prázdný, vrátí se false
        // V opačném případě se vrátí, zda seznam obsahuje záznam s předaným id
        private bool PojistenecExists(int id)
        {
          return (_context.Pojistenci?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
