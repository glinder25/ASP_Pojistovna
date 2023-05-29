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
    public class PojistnaUdalostController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PojistnaUdalostController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int cisloStrany = 1)
        {
            // Získání seznamu pojištění a stránkování
            int zaznamuNaStranu = 5;
            var udalosti = await _context.PojistneUdalosti
                .Skip((cisloStrany - 1) * zaznamuNaStranu)
                .Take(zaznamuNaStranu)
                .ToListAsync();

            // Vypočítání celkového počtu stránek
            int zaznamuCelkem = await _context.PojistneUdalosti.CountAsync();
            int pocetStran = (int)Math.Ceiling((double)zaznamuCelkem / zaznamuNaStranu);

            // Předání dat do view
            ViewData["Udalosti"] = udalosti;
            ViewData["CisloStrany"] = cisloStrany;
            ViewData["PocetStran"] = pocetStran;

            // Zobrazení seznamu pojištění
            return View(udalosti);
        }

        // GET: Pojistenec/Details/5
        // Metoda pro zobrazení detailu pojistné události
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PojistneUdalosti == null) // Pokud id není specifikováno nebo je PojistnaUdalost v DB null, vrátí se chybová stránka
            {
                return NotFound();
            }

            var udalost = await _context.PojistneUdalosti // Hledá se pojistná událost s daným id v DB. Pokud se nenajde, vrátí se chybová stránka
                .FirstOrDefaultAsync(m => m.Id == id);
            if (udalost == null)
            {
                return NotFound();
            }

            return View(udalost); // Pokud se podaří najít pojitnou událost v DB, vrátí se jeho detailní pohled.
        }

        // GET: PojistnaUdalost/Create
        // Metoda pro zobrazení formuláře pro vytvoření nové pojistné události
        // s předvyplněním id pojištění
        public IActionResult Create(int pojisteniId)
        {
            var pojisteni = _context.Pojisteni.Find(pojisteniId); // Najdeme pojištění s danym id v databázi
            if (pojisteni == null) // Pokud není nalezeno, vrátí se chybová stránka
            {
                return NotFound();
            }
            // Uložíme do ViewBag potřebné údaje o pojistnikovi pro předání Id
            ViewBag.PojisteniId = pojisteni.Id;

            return View(); // Vrátíme View s formulářem pro vytvoření nové pojistné události
        }

        // POST: Pojistenec/Create
        // Metoda pro zpracování formuláře pro vytvoření pojistné události
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Datum,Popis,CastkaPoskozeni,PojisteniId")] PojistnaUdalost pojistnaUdalost, int pojisteniId)
        {
            if (ModelState.IsValid) // Zkontrolujeme, zda je stav modelu platný
            {
                pojistnaUdalost.PojisteniId = pojisteniId; // nastavení ID pojištění
                _context.Add(pojistnaUdalost); // Přidání nové pojistné události do databáze
                await _context.SaveChangesAsync(); // Uložení změn v databázi
                TempData["AlertClass"] = "alert-success"; // nastavení barvy alertu
                TempData["AlertMessage"] = "Pojistná událost byla úspěšně uložena."; // nastavení zprávy alertu
                return RedirectToAction("Details", "Pojisteni", new { id = pojisteniId }); // Přesměrování na detail pojištění
            }
            return View(pojistnaUdalost); // Návrat na formulář pro vytvoření nového pojistné události v případě neúspěchu validace
        }

        // GET: Pojisteni/Edit/5
        // Metoda pro zobrazení formuláře pro editaci konkrétní pojistné události s daným ID
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PojistneUdalosti == null) // Pokud id neexistuje nebo databáze Pojistné události neexistuje, vrátí se chybová stránka
            {
                return NotFound();
            }

            var pojistnaUdalost = await _context.PojistneUdalosti.FindAsync(id); // Najde pojistnou událost s daným id
            if (pojistnaUdalost == null) // Pokud není nalezen žádný záznam, vrátí se chybová stránka
            {
                return NotFound();
            }
            return View(pojistnaUdalost); // Jinak se zobrazí View s daty tohoto pojisteni
        }

        // POST: Pojisteni/Edit/5
        // Metoda pro zpracování formuláře pro editaci konkrétní pojistné události s daným ID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Datum,Popis,CastkaPoskozeni,PojisteniId")] PojistnaUdalost pojistnaUdalost)
        {
            if (id != pojistnaUdalost.Id) // zkontrolujeme, zda-li existuje pojistná událost s daným id, pokud ne vrátí se chybová stránka
            {
                return NotFound();
            }

            if (ModelState.IsValid) // Zkontrolujeme, zda je stav modelu platný
            {
                try
                {
                    _context.Update(pojistnaUdalost); // Aktualizace dat pojistné události v DB
                    await _context.SaveChangesAsync(); // Uložení změn v databázi
                    TempData["AlertClass"] = "alert-warning"; // nastavení barvy alertu
                    TempData["AlertMessage"] = "Změny byly uloženy."; // nastavení zprávy alertu
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PojistnaUdalostExists(pojistnaUdalost.Id)) // Kontrola, zda záznam v DB existuje a v případě neúspěchu se vrátí chybová stránka
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Pojisteni", new { id = pojistnaUdalost.PojisteniId }); // Přesměrování na detail pojištění
            }
            return View(pojistnaUdalost);
        }

        // GET: Pojistenec/Delete/5
        // Metoda pro zpracování formuláře pro smazání konkrétní pojistné události s daným ID
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PojistneUdalosti == null) // Pokud ID neexistuje nebo databáze není inicializována, vrátí se chybová stránka
            {
                return NotFound();
            }

            var udalost = await _context.PojistneUdalosti // Hledá se Pojistná událost z databáze s konkrétním ID
                .FirstOrDefaultAsync(m => m.Id == id);
            if (udalost == null) // Pokud se Pojistná událost v databázi nenajde, vrátí se chybová stránka
            {
                return NotFound();
            }

            return View(udalost); // Jinak se vrátí View s Pojistnou událostí k smazání
        }

        // POST: Pojisteni/Delete/5
        // Metoda pro zpracovaní potvrzeného formuláře pro smazání
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PojistneUdalosti == null) // Pokud není databáze inicializována, vrátí se chybová stránka
            {
                return Problem("Entity set 'ApplicationDbContext.Pojisteni'  is null.");
            }
            var udalost = await _context.PojistneUdalosti.FindAsync(id); // Vyhledání záznamu v databázi
            if (udalost != null) // Pokud byl záznam nalezen, tak se z databáze odstraní
            {
                _context.PojistneUdalosti.Remove(udalost);
            }

            await _context.SaveChangesAsync(); // Uložení změn v databázi
            TempData["AlertClass"] = "alert-danger"; // nastavení barvy alertu
            TempData["AlertMessage"] = "Pojistná událost byla odstraněna."; // nastavení zprávy alertu
            return RedirectToAction("Details", "Pojisteni", new { id = udalost.PojisteniId }); // Přesměrování na detail pojištění
        }

        // Metoda kontrolující, zda záznam v databázi existuje
        // Pokud je seznam Pojistnych událostí prázdný, vrátí se false
        // V opačném případě se vrátí, zda seznam obsahuje záznam s předaným id
        private bool PojistnaUdalostExists(int id)
        {
          return (_context.PojistneUdalosti?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
